using Runes.Math;
using System;

namespace Runes.Collections.Operators
{
    public sealed class SizeCompareOps : IComparable<Int>
    {
        public static bool operator ==(SizeCompareOps sizeCompareOps, Int size) => sizeCompareOps.CompareTo(size) == 0;

        public static bool operator !=(SizeCompareOps sizeCompareOps, Int size) => sizeCompareOps.CompareTo(size) != 0;

        public static bool operator >(SizeCompareOps sizeCompareOps, Int size) => sizeCompareOps.CompareTo(size) > 0;

        public static bool operator <(SizeCompareOps sizeCompareOps, Int size) => sizeCompareOps.CompareTo(size) < 0;

        public static bool operator >=(SizeCompareOps sizeCompareOps, Int size) => sizeCompareOps.CompareTo(size) >= 0;

        public static bool operator <=(SizeCompareOps sizeCompareOps, Int size) => sizeCompareOps.CompareTo(size) <= 0;

        public SizeCompareOps(IIterable iterable) => Iterable = iterable;

        public int CompareTo(Int size)
        {
            var curr = Iterable;
            var currSize = Int.Zero;

            while (Iterable.NonEmpty && currSize < size + 1)
            {
                curr = curr.Tail;
                currSize += 1;
            }

            if (currSize < size)
            {
                return -1;
            }
            else if (currSize > size)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }

        private IIterable Iterable { get; }
    }
}
