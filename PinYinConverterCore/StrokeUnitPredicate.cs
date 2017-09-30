using System;

namespace Microsoft.International.Converters.PinYinConverter
{
	internal class StrokeUnitPredicate
	{
		private int ExpectedStrokeNum;

		internal StrokeUnitPredicate(int strokeNum)
		{
			this.ExpectedStrokeNum = strokeNum;
		}

		internal bool Match(StrokeUnit strokeUnit)
		{
			return (int)strokeUnit.StrokeNumber == this.ExpectedStrokeNum;
		}
	}
}
