using System;
using Runes.Collections.Mutable;

namespace Runes.Math
{
    public sealed class QuickSortAlgorithm<A> : ISortingAlgorithm<A, MutableArray<A>>
    {
        public static readonly QuickSortAlgorithm<A> Object = new QuickSortAlgorithm<A>();

        public MutableArray<A> Sort(MutableArray<A> unsortedCollection, Ordering<A> ord)
        {
            var sorted = unsortedCollection;
            Sort(sorted, ord.Compare);
            return sorted;
        }

        public void Sort(A[] array, Func<A, A, int> compare) =>
            DoQuicksort(array, compare, new Random(), 0, array.LongLength - 1);

        private QuickSortAlgorithm() { }

        private void Swap(MutableArray<A> array, long idx1, long idx2)
        {
            var temp = array[idx1];
            array[idx1] = array[idx2];
            array[idx2] = temp;
        }

        private long RandomIndexInRange(long low, long high, Random random) =>
            (long)System.Math.Round(low + (random.NextDouble() * (high - low)));

        private void DoQuicksort(MutableArray<A> array, Func<A, A, int> compare, Random random, long low, long high)
        {
            if (low < high)
            {
                var pivotIndex = RandomIndexInRange(low, high, random);
                var idx = low - 1;

                Swap(array, pivotIndex, high);
                var pivot = array[high];

                long j;
                for (j = low; j < high; j++)
                {
                    if (compare(array[j], pivot) <=0)
                    {
                        idx += 1;
                        Swap(array, idx, j);
                    }
                }

                Swap(array, idx + 1, j);
                var q = idx + 1;

                DoQuicksort(array, compare, random, low, q - 1);
                DoQuicksort(array, compare, random, q + 1, high);
            }
        }
    }
}
