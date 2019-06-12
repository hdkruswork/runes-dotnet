namespace Runes.Collections
{
    public interface ICollectionBuilder<in A, out CC> : IBuilder<CC> where CC : CollectionBase<A, CC>
    {
        void Add(A item);
    }
}
