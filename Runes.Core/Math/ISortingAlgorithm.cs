using System.Collections.Generic;

namespace Runes.Math
{
    public interface ISortingAlgorithm<A, CC> where CC : IEnumerable<A>
    {
        CC Sort(CC unsortedCollection, Ordering<A> ord);
    }
}
