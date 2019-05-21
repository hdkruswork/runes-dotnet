using Microsoft.VisualStudio.TestTools.UnitTesting;

using static Runes.Text.Hex;

namespace Runes.Text.Test
{
    [TestClass]
    public class HexEncoderTest
    {
        [TestMethod]
        public void TestEncoding()
        {
            var hex = "23a43CfF00";
            var bytes = HexEncoder().Encode(hex);

            Assert.AreEqual(5, bytes.Length);
            Assert.AreEqual(0x23, bytes[0]);
            Assert.AreEqual(0xa4, bytes[1]);
            Assert.AreEqual(0x3c, bytes[2]);
            Assert.AreEqual(0xff, bytes[3]);
            Assert.AreEqual(0x00, bytes[4]);
        }
    }
}
