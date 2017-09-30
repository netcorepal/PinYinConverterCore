using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Resources;

namespace Microsoft.International.Converters.PinYinConverter
{
    /// <summary>
    /// 封装了简体中文的读音和笔画等基本信息。
    /// </summary>
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
        /// <summary>
        /// 获取这个字符的拼音个数。
        /// </summary>
		public short PinyinCount
		{
			get
			{
				return this.pinyinCount;
			}
		}
        /// <summary>
        /// 获取这个字符的笔画数。
        /// </summary>
		public short StrokeNumber
		{
			get
			{
				return this.strokeNumber;
			}
		}
        /// <summary>
        /// 获取这个字符是否是多音字。
        /// </summary>
		public bool IsPolyphone
		{
			get
			{
				return this.isPolyphone;
			}
		}
        /// <summary>
        /// 获取这个字符的拼音。该集合长度不能表示实际拼音个数，实际拼音个数请使用PinyinCount字段
        /// </summary>
		public ReadOnlyCollection<string> Pinyins
		{
			get
			{
				return new ReadOnlyCollection<string>(this.pinyinList);
			}
		}
        /// <summary>
        /// 获取这个汉字字符。
        /// </summary>
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
        /// <summary>
        /// ChineseChar类的构造函数。
        /// </summary>
        /// <param name="ch">指定的汉字字符。</param>
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
        /// <summary>
        /// 识别字符是否有指定的读音。
        /// </summary>
        /// <param name="pinyin">指定的需要被识别的拼音</param>
        /// <returns>如果指定的拼音字符串在实例字符的拼音集合中则返回ture，否则返回false。 </returns>
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
        /// <summary>
        /// 识别给出的字符是否是实例字符的同音字。 
        /// </summary>
        /// <param name="ch">指出需要识别的字符。</param>
        /// <returns>如果给出的实例字符是同音字则返回ture，否则返回false。</returns>
		public bool IsHomophone(char ch)
		{
			return ChineseChar.IsHomophone(this.chineseCharacter, ch);
        }
        /// <summary>
        /// 识别给出的两个字符是否是同音字。
        /// </summary>
        /// <param name="ch1">指出第一个字符</param>
        /// <param name="ch2">指出第二个字符</param>
        /// <returns>如果给出的字符是同音字返回ture，否则返回false。</returns>
		public static bool IsHomophone(char ch1, char ch2)
		{
			CharUnit charUnit = ChineseChar.charDictionary.GetCharUnit(ch1);
			CharUnit charUnit2 = ChineseChar.charDictionary.GetCharUnit(ch2);
			return ChineseChar.ExistSameElement<short>(charUnit.PinyinIndexList, charUnit2.PinyinIndexList);
        }
        /// <summary>
        /// 将给出的字符和实例字符的笔画数进行比较。
        /// </summary>
        /// <param name="ch">显示给出的字符</param>
        /// <returns>说明比较操作的结果。 如果给出字符和实例字符的笔画数相同，返回值为 0。 如果实例字符比给出字符的笔画多，返回值为> 0。 如果实例字符比给出字符的笔画少，返回值为 &lt; 0。 </returns>
		public int CompareStrokeNumber(char ch)
		{
			CharUnit charUnit = ChineseChar.charDictionary.GetCharUnit(ch);
			return (int)(this.StrokeNumber - (short)charUnit.StrokeNumber);
        }
        /// <summary>
        /// 获取给定拼音的所有同音字。
        /// </summary>
        /// <param name="pinyin">指出读音。</param>
        /// <returns>返回具有相同的指定拼音的字符串列表。 如果拼音不是有效值则返回空。 </returns>
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
        /// <summary>
        /// 识别给出的拼音是否是一个有效的拼音字符串。
        /// </summary>
        /// <param name="pinyin">指出需要识别的字符串。</param>
        /// <returns>如果指定的字符串是一个有效的拼音字符串则返回ture，否则返回false。</returns>
		public static bool IsValidPinyin(string pinyin)
		{
			if (pinyin == null)
			{
				throw new ArgumentNullException("pinyin");
			}
			return ChineseChar.pinyinDictionary.GetPinYinUnitIndex(pinyin) >= 0;
        }
        /// <summary>
        /// 识别给出的字符串是否是一个有效的汉字字符。
        /// </summary>
        /// <param name="ch">指出需要识别的字符。</param>
        /// <returns>如果指定的字符是一个有效的汉字字符则返回ture，否则返回false。</returns>
		public static bool IsValidChar(char ch)
		{
			CharUnit charUnit = ChineseChar.charDictionary.GetCharUnit(ch);
			return charUnit != null;
		}
        /// <summary>
        /// 识别给出的笔画数是否是一个有效的笔画数。
        /// </summary>
        /// <param name="strokeNumber">指出需要识别的笔画数。</param>
        /// <returns>如果指定的笔画数是一个有效的笔画数则返回ture，否则返回false。</returns>
		public static bool IsValidStrokeNumber(short strokeNumber)
		{
			if (strokeNumber < 0 || strokeNumber > 48)
			{
				return false;
			}
			StrokeUnit strokeUnit = ChineseChar.strokeDictionary.GetStrokeUnit((int)strokeNumber);
			return strokeUnit != null;
        }
        /// <summary>
        /// 检索具有指定拼音的字符数。
        /// </summary>
        /// <param name="pinyin">显示需要被识别的拼音字符串。</param>
        /// <returns>返回具有指定拼音的字符数。 如果拼音不是有效值则返回-1。</returns>
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
        /// <summary>
        /// 检索指定字符的笔画数。 
        /// </summary>
        /// <param name="ch">指出需要识别的字符。</param>
        /// <returns>返回指定字符的笔画数。 如果字符不是有效值则返回-1。 </returns>
		public static short GetStrokeNumber(char ch)
		{
			if (!ChineseChar.IsValidChar(ch))
			{
				return -1;
			}
			CharUnit charUnit = ChineseChar.charDictionary.GetCharUnit(ch);
			return (short)charUnit.StrokeNumber;
        }
        /// <summary>
        /// 检索具有指定笔画数的所有字符串。
        /// </summary>
        /// <param name="strokeNumber">指出需要被识别的笔画数。</param>
        /// <returns>返回具有指定笔画数的字符列表。 如果笔画数是无效值返回空。</returns>
		public static char[] GetChars(short strokeNumber)
		{
			if (!ChineseChar.IsValidStrokeNumber(strokeNumber))
			{
				return null;
			}
			StrokeUnit strokeUnit = ChineseChar.strokeDictionary.GetStrokeUnit((int)strokeNumber);
			return strokeUnit.CharList;
        }
        /// <summary>
        /// 检索具有指定笔画数的字符个数。
        /// </summary>
        /// <param name="strokeNumber">显示需要被识别的笔画数。</param>
        /// <returns>返回具有指定笔画数的字符数。 如果笔画数是无效值返回-1。</returns>
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
