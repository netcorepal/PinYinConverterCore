using System;
using System.IO;

namespace Microsoft.International.Converters.PinYinConverter
{
	internal class StrokeUnit
	{
		internal byte StrokeNumber;

		internal short CharCount;

		internal char[] CharList;

		internal static StrokeUnit Deserialize(BinaryReader binaryReader)
		{
			StrokeUnit strokeUnit = new StrokeUnit();
			strokeUnit.StrokeNumber = binaryReader.ReadByte();
			strokeUnit.CharCount = binaryReader.ReadInt16();
			strokeUnit.CharList = new char[(int)strokeUnit.CharCount];
			for (int i = 0; i < (int)strokeUnit.CharCount; i++)
			{
				strokeUnit.CharList[i] = binaryReader.ReadChar();
			}
			return strokeUnit;
		}

		internal void Serialize(BinaryWriter binaryWriter)
		{
			if (this.CharCount == 0)
			{
				return;
			}
			binaryWriter.Write(this.StrokeNumber);
			binaryWriter.Write(this.CharCount);
			binaryWriter.Write(this.CharList);
		}
	}
}
