using System;

using static Runes.Predef;

namespace Runes
{
    public sealed class Unit
    {
        public static implicit operator Unit(Action action) => Unit(action);

        public override bool Equals(object obj) => obj == Unit();

        public override int GetHashCode() => 1;

        public override string ToString() => string.Empty;

        internal Unit() { }
    }
}
