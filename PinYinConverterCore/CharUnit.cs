using System;
using System.IO;

namespace Microsoft.International.Converters.PinYinConverter
{
	internal class CharUnit
	{
		internal char Char;

		internal byte StrokeNumber;

		internal byte PinyinCount;

		internal short[] PinyinIndexList;

		internal static CharUnit Deserialize(BinaryReader binaryReader)
		{
			CharUnit charUnit = new CharUnit();
			charUnit.Char = binaryReader.ReadChar();
			charUnit.StrokeNumber = binaryReader.ReadByte();
			charUnit.PinyinCount = binaryReader.ReadByte();
			charUnit.PinyinIndexList = new short[(int)charUnit.PinyinCount];
			for (int i = 0; i < (int)charUnit.PinyinCount; i++)
			{
				charUnit.PinyinIndexList[i] = binaryReader.ReadInt16();
			}
			return charUnit;
		}

		internal void Serialize(BinaryWriter binaryWriter)
		{
			binaryWriter.Write(this.Char);
			binaryWriter.Write(this.StrokeNumber);
			binaryWriter.Write(this.PinyinCount);
			for (int i = 0; i < (int)this.PinyinCount; i++)
			{
				binaryWriter.Write(this.PinyinIndexList[i]);
			}
		}
	}
}
