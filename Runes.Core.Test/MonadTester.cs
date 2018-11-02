using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Runes.Core.Test
{
    public static class MonadTester
    {
        public static void TestMonadType<M>(IMonadBuilder<int, M> builder) where M : IMonad<int>
        {
            TestLeftIdentityRule(builder);
            TestRightIdentityRule(builder);
            TestAssociativityRule(builder);
        }

        private static void TestLeftIdentityRule<M>(IMonadBuilder<int, M> builder) where M : IMonad<int>
        {
            var value = 1;
            var monad = builder.BuildFrom(value);
            IMonad<int> f(int i) => builder.BuildFrom(i * 2);
            Assert.AreEqual(
                monad.FlatMap(f, builder),
                f(value),
                $"Type '{typeof(M)}' doesn't obey the left identity rule of monads"
            );
        }

        private static void TestRightIdentityRule<M>(IMonadBuilder<int, M> builder) where M : IMonad<int>
        {
            var value = 1;
            var monad = builder.BuildFrom(value);
            Assert.AreEqual(
                monad,
                monad.FlatMap(v => builder.BuildFrom(v), builder),
                $"Type '{typeof(M)}' doesn't obey the right identity rule of monads"
            );
        }

        private static void TestAssociativityRule<M>(IMonadBuilder<int, M> builder) where M : IMonad<int>
        {
            var value = 1;
            var monad = builder.BuildFrom(value);
            IMonad<int> f(int i) => builder.BuildFrom(i * 2);
            IMonad<int> g(int i) => builder.BuildFrom(i + 6);

            Assert.AreEqual(
                monad.FlatMap(f, builder).FlatMap(g, builder),
                monad.FlatMap(v => f(v).FlatMap(g, builder), builder),
                $"Type '{typeof(M)}' doesn't obey the associativity rule of monads"
            );
        }
    }
}
