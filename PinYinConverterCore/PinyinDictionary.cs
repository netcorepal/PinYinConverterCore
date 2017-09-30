using System;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.International.Converters.PinYinConverter
{
	internal class PinyinDictionary
	{
		internal short Length;

		internal short Count;

		internal short Offset;

		internal readonly byte[] Reserved = new byte[8];

		internal List<PinyinUnit> PinyinUnitTable;

		internal readonly short EndMark = 32767;

		internal void Serialize(BinaryWriter binaryWriter)
		{
			binaryWriter.Write(this.Length);
			binaryWriter.Write(this.Count);
			binaryWriter.Write(this.Offset);
			binaryWriter.Write(this.Reserved);
			for (int i = 0; i < (int)this.Count; i++)
			{
				this.PinyinUnitTable[i].Serialize(binaryWriter);
			}
			binaryWriter.Write(this.EndMark);
		}

		internal static PinyinDictionary Deserialize(BinaryReader binaryReader)
		{
			PinyinDictionary pinyinDictionary = new PinyinDictionary();
			binaryReader.ReadInt32();
			pinyinDictionary.Length = binaryReader.ReadInt16();
			pinyinDictionary.Count = binaryReader.ReadInt16();
			pinyinDictionary.Offset = binaryReader.ReadInt16();
			binaryReader.ReadBytes(8);
			pinyinDictionary.PinyinUnitTable = new List<PinyinUnit>();
			for (int i = 0; i < (int)pinyinDictionary.Count; i++)
			{
				pinyinDictionary.PinyinUnitTable.Add(PinyinUnit.Deserialize(binaryReader));
			}
			binaryReader.ReadInt16();
			return pinyinDictionary;
		}

		internal int GetPinYinUnitIndex(string pinyin)
		{
			PinyinUnitPredicate @object = new PinyinUnitPredicate(pinyin);
			return this.PinyinUnitTable.FindIndex(new Predicate<PinyinUnit>(@object.Match));
		}

		internal PinyinUnit GetPinYinUnit(string pinyin)
		{
			PinyinUnitPredicate @object = new PinyinUnitPredicate(pinyin);
			return this.PinyinUnitTable.Find(new Predicate<PinyinUnit>(@object.Match));
		}

		internal PinyinUnit GetPinYinUnitByIndex(int index)
		{
			if (index < 0 || index >= (int)this.Count)
			{
				throw new ArgumentOutOfRangeException("index", AssemblyResource.INDEX_OUT_OF_RANGE);
			}
			return this.PinyinUnitTable[index];
		}
	}
}
