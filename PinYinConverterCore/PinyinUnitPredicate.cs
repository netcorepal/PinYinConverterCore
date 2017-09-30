using System;
using System.Globalization;

namespace Microsoft.International.Converters.PinYinConverter
{
	internal class PinyinUnitPredicate
	{
		private string ExpectedPinyin;

		internal PinyinUnitPredicate(string pinyin)
		{
			this.ExpectedPinyin = pinyin;
		}

		internal bool Match(PinyinUnit pinyinUnit)
		{
			return string.Compare(pinyinUnit.Pinyin, this.ExpectedPinyin, true, CultureInfo.CurrentCulture) == 0;
		}
	}
}
