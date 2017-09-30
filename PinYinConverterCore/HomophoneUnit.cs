using System;
using System.IO;

namespace Microsoft.International.Converters.PinYinConverter
{
	internal class HomophoneUnit
	{
		internal short Count;

		internal char[] HomophoneList;

		internal static HomophoneUnit Deserialize(BinaryReader binaryReader)
		{
			HomophoneUnit homophoneUnit = new HomophoneUnit();
			homophoneUnit.Count = binaryReader.ReadInt16();
			homophoneUnit.HomophoneList = new char[(int)homophoneUnit.Count];
			for (int i = 0; i < (int)homophoneUnit.Count; i++)
			{
				homophoneUnit.HomophoneList[i] = binaryReader.ReadChar();
			}
			return homophoneUnit;
		}

		internal void Serialize(BinaryWriter binaryWriter)
		{
			binaryWriter.Write(this.Count);
			for (int i = 0; i < (int)this.Count; i++)
			{
				binaryWriter.Write(this.HomophoneList[i]);
			}
		}
	}
}
