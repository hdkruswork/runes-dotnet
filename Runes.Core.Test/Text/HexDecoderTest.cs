using Microsoft.VisualStudio.TestTools.UnitTesting;

using static Runes.Text.Hex;

namespace Runes.Text.Test
{
    [TestClass]
    public class HexDecoderTest
    {
        [TestMethod]
        public void TestDecoding()
        {
            var hex = HexDecoder()
                .Decode(new byte [] { 0x23, 0xa4, 0x3c, 0xff, 0x00 });

            Assert.AreEqual("23a43cff00", hex);
        }

        [TestMethod]
        public void TestDecodingInUppercase()
        {
            var hex = HexDecoder()
                .WithCase(TextCaseConstraint.UPPERCASED)
                .Decode(new byte [] { 0x23, 0xa4, 0x3c, 0xff, 0x00 });

            Assert.AreEqual("23A43CFF00", hex);
        }
    }
}
