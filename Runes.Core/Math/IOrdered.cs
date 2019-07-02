using System;

namespace Runes.Math
{
    public interface IOrdered<in A> : IComparable<A>
    {
        bool EqualsTo(A other) => CompareTo(other) == 0;

        bool NonEqualsTo(A other) => !EqualsTo(other);

        bool GreaterThan(A other) => CompareTo(other) > 0;

        bool GreaterEqualsThan(A other) => CompareTo(other) >= 0;

        bool LesserThan(A other) => CompareTo(other) < 0;

        bool LesserEqualsThan(A other) => CompareTo(other) <= 0;
    }

    public abstract class Ordered<A> : IOrdered<A>
    {
        public abstract int CompareTo(A other);

        public bool EqualsTo(A other) => CompareTo(other) == 0;

        public bool NonEqualsTo(A other) => !EqualsTo(other);

        public bool GreaterThan(A other) => CompareTo(other) > 0;

        public bool GreaterEqualsThan(A other) => CompareTo(other) >= 0;

        public bool LesserThan(A other) => CompareTo(other) < 0;

        public bool LesserEqualsThan(A other) => CompareTo(other) <= 0;
    }
}
