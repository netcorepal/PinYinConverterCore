using System;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.International.Converters.PinYinConverter
{
	internal class CharDictionary
	{
		internal int Length;

		internal int Count;

		internal short Offset;

		internal readonly byte[] Reserved = new byte[24];

		internal List<CharUnit> CharUnitTable;

		internal readonly short EndMark = 32767;

		internal void Serialize(BinaryWriter binaryWriter)
		{
			binaryWriter.Write(this.Length);
			binaryWriter.Write(this.Count);
			binaryWriter.Write(this.Offset);
			binaryWriter.Write(this.Reserved);
			for (int i = 0; i < this.Count; i++)
			{
				this.CharUnitTable[i].Serialize(binaryWriter);
			}
			binaryWriter.Write(this.EndMark);
		}

		internal static CharDictionary Deserialize(BinaryReader binaryReader)
		{
			CharDictionary charDictionary = new CharDictionary();
			binaryReader.ReadInt32();
			charDictionary.Length = binaryReader.ReadInt32();
			charDictionary.Count = binaryReader.ReadInt32();
			charDictionary.Offset = binaryReader.ReadInt16();
			binaryReader.ReadBytes(24);
			charDictionary.CharUnitTable = new List<CharUnit>();
			for (int i = 0; i < charDictionary.Count; i++)
			{
				charDictionary.CharUnitTable.Add(CharUnit.Deserialize(binaryReader));
			}
			binaryReader.ReadInt16();
			return charDictionary;
		}

		internal CharUnit GetCharUnit(int index)
		{
			if (index < 0 || index >= this.Count)
			{
				throw new ArgumentOutOfRangeException("index", AssemblyResource.INDEX_OUT_OF_RANGE);
			}
			return this.CharUnitTable[index];
		}

		internal CharUnit GetCharUnit(char ch)
		{
			CharUnitPredicate @object = new CharUnitPredicate(ch);
			return this.CharUnitTable.Find(new Predicate<CharUnit>(@object.Match));
		}
	}
}
