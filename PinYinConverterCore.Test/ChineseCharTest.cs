using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.International.Converters.PinYinConverter;
namespace PinYinConverterCore.Test
{
    [TestClass]
    public class ChineseCharTest
    {
        [TestMethod]
        public void IsValidCharTest()
        {
            Assert.IsTrue(ChineseChar.IsValidChar('°¡'));
            var cc = new ChineseChar('°¡');
            var pinyin = cc.Pinyins;
        }
    }
}
