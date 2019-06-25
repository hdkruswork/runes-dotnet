using System;

namespace Runes.Math.Algorithms
{
    public sealed class QuickSortAlgorithm<A> : ISortingAlgorithm<A, A[]>
    {
        public static readonly QuickSortAlgorithm<A> Object = new QuickSortAlgorithm<A>();

        public A[] Sort(A[] unsortedCollection, Ordering<A> ord)
        {
            var sorted = unsortedCollection;
            Sort(sorted, ord.Compare);
            return sorted;
        }

        public void Sort(A[] array, Func<A, A, int> compare) =>
            DoQuickSort(array, compare, new Random(), 0, array.LongLength - 1);

        private QuickSortAlgorithm() { }

        private static void Swap(A[] array, long idx1, long idx2)
        {
            var temp = array[idx1];
            array[idx1] = array[idx2];
            array[idx2] = temp;
        }

        private static long RandomIndexInRange(long low, long high, Random random) =>
            (long)System.Math.Round(low + (random.NextDouble() * (high - low)));

        private static void DoQuickSort(A[] array, Func<A, A, int> compare, Random random, long low, long high)
        {
            if (low >= high)
            {
                return;
            }

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

            DoQuickSort(array, compare, random, low, q - 1);
            DoQuickSort(array, compare, random, q + 1, high);
        }
    }
}
