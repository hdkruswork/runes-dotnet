using System;

namespace Runes
{
    public sealed class Unit
    {
        public static Unit Of(Action action)
        {
            action();
            return Object;
        }
        public static Func<A1, Unit> Of<A1>(Action<A1> action) => new Func<A1, Unit>((a1) =>
        {
            action(a1);
            return Object;
        });
        public static Func<A1, A2, Unit> Of<A1, A2>(Action<A1, A2> action) => new Func<A1, A2, Unit>((a1, a2) =>
        {
            action(a1, a2);
            return Object;
        });
        public static Func<A1, A2, A3, Unit> Of<A1, A2, A3>(Action<A1, A2, A3> action) => new Func<A1, A2, A3, Unit>((a1, a2, a3) =>
        {
            action(a1, a2, a3);
            return Object;
        });

        private static readonly Unit Object = new Unit();

        public override bool Equals(object obj) => obj == Object;
        public override int GetHashCode() => 0;

        public override string ToString() => string.Empty;

        private Unit() { }
    }
}
