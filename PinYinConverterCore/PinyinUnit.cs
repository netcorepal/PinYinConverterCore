using System;
using System.IO;
using System.Text;

namespace Microsoft.International.Converters.PinYinConverter
{
	internal class PinyinUnit
	{
		internal string Pinyin;

		internal static PinyinUnit Deserialize(BinaryReader binaryReader)
		{
			PinyinUnit pinyinUnit = new PinyinUnit();
			byte[] bytes = binaryReader.ReadBytes(7);
			pinyinUnit.Pinyin = Encoding.ASCII.GetString(bytes, 0, 7);
			char[] array = new char[1];
			char[] trimChars = array;
			pinyinUnit.Pinyin = pinyinUnit.Pinyin.TrimEnd(trimChars);
			return pinyinUnit;
		}

		internal void Serialize(BinaryWriter binaryWriter)
		{
			byte[] array = new byte[7];
			Encoding.ASCII.GetBytes(this.Pinyin, 0, this.Pinyin.Length, array, 0);
			binaryWriter.Write(array);
		}
	}
}
