using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Runes
{
    [TestClass]
    public class CoreExtensionsTest
    {
        [TestMethod]
        public void TestIsExtension()
        {
            object obj = "hello";

            if (obj.Is(out string str))
            {
                Assert.AreEqual("hello", str);
            }
            else
            {
                Assert.Fail($"var {nameof(obj)} should be casted as string");
            }
        }
    }
}
