using System;
       
using static Runes.Units;

namespace Runes
{
    public static class Units
    {
        private static readonly Unit Object = new Unit();

        public static Unit Unit() => Object;

        public static Unit Unit(Action action)
        {
            action();
            return Unit();
        }
    }

    public sealed class Unit
    {
        public static implicit operator Unit(Action action) => Unit(action);

        public override bool Equals(object obj) => obj == Unit();

        public override int GetHashCode() => 1;

        public override string ToString() => string.Empty;

        internal Unit() { }
    }
}
