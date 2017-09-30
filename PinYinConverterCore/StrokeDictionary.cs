using System;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.International.Converters.PinYinConverter
{
	internal class StrokeDictionary
	{
		internal const short MaxStrokeNumber = 48;

		internal int Length;

		internal int Count;

		internal short Offset;

		internal readonly byte[] Reserved = new byte[24];

		internal List<StrokeUnit> StrokeUnitTable;

		internal readonly short EndMark = 32767;

		internal void Serialize(BinaryWriter binaryWriter)
		{
			binaryWriter.Write(this.Length);
			binaryWriter.Write(this.Count);
			binaryWriter.Write(this.Offset);
			binaryWriter.Write(this.Reserved);
			for (int i = 0; i < this.Count; i++)
			{
				this.StrokeUnitTable[i].Serialize(binaryWriter);
			}
			binaryWriter.Write(this.EndMark);
		}

		internal static StrokeDictionary Deserialize(BinaryReader binaryReader)
		{
			StrokeDictionary strokeDictionary = new StrokeDictionary();
			binaryReader.ReadInt32();
			strokeDictionary.Length = binaryReader.ReadInt32();
			strokeDictionary.Count = binaryReader.ReadInt32();
			strokeDictionary.Offset = binaryReader.ReadInt16();
			binaryReader.ReadBytes(24);
			strokeDictionary.StrokeUnitTable = new List<StrokeUnit>();
			for (int i = 0; i < strokeDictionary.Count; i++)
			{
				strokeDictionary.StrokeUnitTable.Add(StrokeUnit.Deserialize(binaryReader));
			}
			binaryReader.ReadInt16();
			return strokeDictionary;
		}

		internal StrokeUnit GetStrokeUnitByIndex(int index)
		{
			if (index < 0 || index >= this.Count)
			{
				throw new ArgumentOutOfRangeException("index", AssemblyResource.INDEX_OUT_OF_RANGE);
			}
			return this.StrokeUnitTable[index];
		}

		internal StrokeUnit GetStrokeUnit(int strokeNum)
		{
			if (strokeNum <= 0 || strokeNum > 48)
			{
				throw new ArgumentOutOfRangeException("strokeNum");
			}
			StrokeUnitPredicate @object = new StrokeUnitPredicate(strokeNum);
			return this.StrokeUnitTable.Find(new Predicate<StrokeUnit>(@object.Match));
		}
	}
}
