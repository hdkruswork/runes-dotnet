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
}
