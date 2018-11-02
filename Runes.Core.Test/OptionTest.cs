using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Runes.Core.Test
{
    [TestClass]
    public class OptionTest
    {
        [TestMethod]
        public void Test_option_type_is_a_monad() => MonadTester.TestMonadType(Option<int>.Builder);

        [TestMethod]
        public void Test_from_null() => Assert.AreEqual(Option.None<string>(), Option.From<string>(null));

        [TestMethod]
        public void Test_from_not_null() => Assert.AreEqual(Option.Some(""), Option.From(""));

        [TestMethod]
        public void Test_from_null_Nullable() => Assert.AreEqual(Option.None<int>(), Option.From(default(int?)));

        [TestMethod]
        public void Test_from_not_null_Nullable() => Assert.AreEqual(Option.Some(10), Option.From((int?)10));

        [TestMethod]
        public void Test_None()
        {
            var none = Option.None<int>();

            Assert.IsTrue(none.IsEmpty);
            Assert.IsFalse(none.NonEmpty);
            Assert.AreEqual(none, none.FlatMap(_ => Option.None<int>()));
            Assert.AreEqual(none, none.FlatMap(v => Option.Some(v + 10)));
            Assert.AreEqual(none, none.Map(v => v * 10));
            Assert.AreEqual(10, none.GetOrElse(10));
        }

        [TestMethod]
        public void Test_Some()
        {
            var some = Option.Some(5);

            Assert.IsFalse(some.IsEmpty);
            Assert.IsTrue(some.NonEmpty);
            Assert.AreEqual(Option.None<int>(), some.FlatMap(_ => Option.None<int>()));
            Assert.AreEqual(Option.Some(15), some.FlatMap(v => Option.Some(v + 10)));
            Assert.AreEqual(Option.Some(50), some.Map(v => v * 10));
            Assert.AreEqual(5, some.GetOrElse(10));
        }
    }
}
