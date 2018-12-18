using Microsoft.VisualStudio.TestTools.UnitTesting;

using static Runes.OptionExtensions;

namespace Runes.Core.Test
{
    [TestClass]
    public class OptionTest
    {
        [TestMethod]
        public void Test_option_type_is_a_monad() => MonadTest.TestMonadType(Option<int>.Builder);

        [TestMethod]
        public void Test_from_null() => Assert.AreEqual(None<string>(), Option<string>(null));

        [TestMethod]
        public void Test_from_not_null() => Assert.AreEqual(Some(""), Option(""));

        [TestMethod]
        public void Test_from_null_Nullable() => Assert.AreEqual(None<int>(), Option(default(int?)));

        [TestMethod]
        public void Test_from_not_null_Nullable() => Assert.AreEqual(Some(10), Option((int?)10));

        [TestMethod]
        public void Test_None()
        {
            var none = None<int>();

            Assert.IsTrue(none.IsEmpty);
            Assert.IsFalse(none.NonEmpty);
            Assert.AreEqual(none, none.FlatMap(_ => None<int>()));
            Assert.AreEqual(none, none.FlatMap(v => Some(v + 10)));
            Assert.AreEqual(none, none.Map(v => v * 10));
            Assert.AreEqual(10, none.GetOrElse(10));
        }

        [TestMethod]
        public void Test_Some()
        {
            var some = Some(5);

            Assert.IsFalse(some.IsEmpty);
            Assert.IsTrue(some.NonEmpty);
            Assert.AreEqual(None<int>(), some.FlatMap(_ => None<int>()));
            Assert.AreEqual(Some(15), some.FlatMap(v => Some(v + 10)));
            Assert.AreEqual(Some(50), some.Map(v => v * 10));
            Assert.AreEqual(5, some.GetOrElse(10));
        }
    }
}
