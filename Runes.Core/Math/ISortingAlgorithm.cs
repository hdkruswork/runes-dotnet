using Runes.Collections;

namespace Runes.Math
{
    public interface ISortingAlgorithm<A, CC> where CC : IIterable<A>
    {
        CC Sort(CC unsortedCollection, Ordering<A> ord);
    }
}
