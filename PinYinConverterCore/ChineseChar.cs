using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Resources;

namespace Microsoft.International.Converters.PinYinConverter
{
	public class ChineseChar
	{
		private const short MaxPolyphoneNum = 8;

		private static CharDictionary charDictionary;

		private static PinyinDictionary pinyinDictionary;

		private static StrokeDictionary strokeDictionary;

		private static HomophoneDictionary homophoneDictionary;

		private char chineseCharacter;

		private short strokeNumber;

		private bool isPolyphone;

		private short pinyinCount;

		private string[] pinyinList = new string[8];

		public short PinyinCount
		{
			get
			{
				return this.pinyinCount;
			}
		}

		public short StrokeNumber
		{
			get
			{
				return this.strokeNumber;
			}
		}

		public bool IsPolyphone
		{
			get
			{
				return this.isPolyphone;
			}
		}

		public ReadOnlyCollection<string> Pinyins
		{
			get
			{
				return new ReadOnlyCollection<string>(this.pinyinList);
			}
		}

		public char ChineseCharacter
		{
			get
			{
				return this.chineseCharacter;
			}
		}

		static ChineseChar()
		{
			Assembly executingAssembly = Assembly.GetExecutingAssembly();
			using (Stream manifestResourceStream = executingAssembly.GetManifestResourceStream("Microsoft.International.Converters.PinYinConverter.PinyinDictionary.resources"))
			{
				using (ResourceReader resourceReader = new ResourceReader(manifestResourceStream))
				{
					string text;
					byte[] buffer;
					resourceReader.GetResourceData("PinyinDictionary", out text, out buffer);
					using (BinaryReader binaryReader = new BinaryReader(new MemoryStream(buffer)))
					{
						ChineseChar.pinyinDictionary = PinyinDictionary.Deserialize(binaryReader);
					}
				}
			}
			using (Stream manifestResourceStream2 = executingAssembly.GetManifestResourceStream("Microsoft.International.Converters.PinYinConverter.CharDictionary.resources"))
			{
				using (ResourceReader resourceReader2 = new ResourceReader(manifestResourceStream2))
				{
					string text;
					byte[] buffer;
					resourceReader2.GetResourceData("CharDictionary", out text, out buffer);
					using (BinaryReader binaryReader2 = new BinaryReader(new MemoryStream(buffer)))
					{
						ChineseChar.charDictionary = CharDictionary.Deserialize(binaryReader2);
					}
				}
			}
			using (Stream manifestResourceStream3 = executingAssembly.GetManifestResourceStream("Microsoft.International.Converters.PinYinConverter.HomophoneDictionary.resources"))
			{
				using (ResourceReader resourceReader3 = new ResourceReader(manifestResourceStream3))
				{
					string text;
					byte[] buffer;
					resourceReader3.GetResourceData("HomophoneDictionary", out text, out buffer);
					using (BinaryReader binaryReader3 = new BinaryReader(new MemoryStream(buffer)))
					{
						ChineseChar.homophoneDictionary = HomophoneDictionary.Deserialize(binaryReader3);
					}
				}
			}
			using (Stream manifestResourceStream4 = executingAssembly.GetManifestResourceStream("Microsoft.International.Converters.PinYinConverter.StrokeDictionary.resources"))
			{
				using (ResourceReader resourceReader4 = new ResourceReader(manifestResourceStream4))
				{
					string text;
					byte[] buffer;
					resourceReader4.GetResourceData("StrokeDictionary", out text, out buffer);
					using (BinaryReader binaryReader4 = new BinaryReader(new MemoryStream(buffer)))
					{
						ChineseChar.strokeDictionary = StrokeDictionary.Deserialize(binaryReader4);
					}
				}
			}
		}

		public ChineseChar(char ch)
		{
			if (!ChineseChar.IsValidChar(ch))
			{
				throw new NotSupportedException(AssemblyResource.CHARACTER_NOT_SUPPORTED);
			}
			this.chineseCharacter = ch;
			CharUnit charUnit = ChineseChar.charDictionary.GetCharUnit(ch);
			this.strokeNumber = (short)charUnit.StrokeNumber;
			this.pinyinCount = (short)charUnit.PinyinCount;
			this.isPolyphone = (charUnit.PinyinCount > 1);
			for (int i = 0; i < (int)this.pinyinCount; i++)
			{
				PinyinUnit pinYinUnitByIndex = ChineseChar.pinyinDictionary.GetPinYinUnitByIndex((int)charUnit.PinyinIndexList[i]);
				this.pinyinList[i] = pinYinUnitByIndex.Pinyin;
			}
		}

		public bool HasSound(string pinyin)
		{
			if (pinyin == null)
			{
				throw new ArgumentNullException("HasSound_pinyin");
			}
			for (int i = 0; i < (int)this.PinyinCount; i++)
			{
				if (string.Compare(this.Pinyins[i], pinyin, true, CultureInfo.CurrentCulture) == 0)
				{
					return true;
				}
			}
			return false;
		}

		public bool IsHomophone(char ch)
		{
			return ChineseChar.IsHomophone(this.chineseCharacter, ch);
		}

		public static bool IsHomophone(char ch1, char ch2)
		{
			CharUnit charUnit = ChineseChar.charDictionary.GetCharUnit(ch1);
			CharUnit charUnit2 = ChineseChar.charDictionary.GetCharUnit(ch2);
			return ChineseChar.ExistSameElement<short>(charUnit.PinyinIndexList, charUnit2.PinyinIndexList);
		}

		public int CompareStrokeNumber(char ch)
		{
			CharUnit charUnit = ChineseChar.charDictionary.GetCharUnit(ch);
			return (int)(this.StrokeNumber - (short)charUnit.StrokeNumber);
		}

		public static char[] GetChars(string pinyin)
		{
			if (pinyin == null)
			{
				throw new ArgumentNullException("pinyin");
			}
			if (!ChineseChar.IsValidPinyin(pinyin))
			{
				return null;
			}
			HomophoneUnit homophoneUnit = ChineseChar.homophoneDictionary.GetHomophoneUnit(ChineseChar.pinyinDictionary, pinyin);
			return homophoneUnit.HomophoneList;
		}

		public static bool IsValidPinyin(string pinyin)
		{
			if (pinyin == null)
			{
				throw new ArgumentNullException("pinyin");
			}
			return ChineseChar.pinyinDictionary.GetPinYinUnitIndex(pinyin) >= 0;
		}

		public static bool IsValidChar(char ch)
		{
			CharUnit charUnit = ChineseChar.charDictionary.GetCharUnit(ch);
			return charUnit != null;
		}

		public static bool IsValidStrokeNumber(short strokeNumber)
		{
			if (strokeNumber < 0 || strokeNumber > 48)
			{
				return false;
			}
			StrokeUnit strokeUnit = ChineseChar.strokeDictionary.GetStrokeUnit((int)strokeNumber);
			return strokeUnit != null;
		}

		public static short GetHomophoneCount(string pinyin)
		{
			if (pinyin == null)
			{
				throw new ArgumentNullException("pinyin");
			}
			if (!ChineseChar.IsValidPinyin(pinyin))
			{
				return -1;
			}
			return ChineseChar.homophoneDictionary.GetHomophoneUnit(ChineseChar.pinyinDictionary, pinyin).Count;
		}

		public static short GetStrokeNumber(char ch)
		{
			if (!ChineseChar.IsValidChar(ch))
			{
				return -1;
			}
			CharUnit charUnit = ChineseChar.charDictionary.GetCharUnit(ch);
			return (short)charUnit.StrokeNumber;
		}

		public static char[] GetChars(short strokeNumber)
		{
			if (!ChineseChar.IsValidStrokeNumber(strokeNumber))
			{
				return null;
			}
			StrokeUnit strokeUnit = ChineseChar.strokeDictionary.GetStrokeUnit((int)strokeNumber);
			return strokeUnit.CharList;
		}

		public static short GetCharCount(short strokeNumber)
		{
			if (!ChineseChar.IsValidStrokeNumber(strokeNumber))
			{
				return -1;
			}
			return ChineseChar.strokeDictionary.GetStrokeUnit((int)strokeNumber).CharCount;
		}

		private static bool ExistSameElement<T>(T[] array1, T[] array2) where T : IComparable
		{
			int num = 0;
			int num2 = 0;
			while (num < array1.Length && num2 < array2.Length)
			{
				if (array1[num].CompareTo(array2[num2]) < 0)
				{
					num++;
				}
				else
				{
					if (array1[num].CompareTo(array2[num2]) <= 0)
					{
						return true;
					}
					num2++;
				}
			}
			return false;
		}
	}
}
