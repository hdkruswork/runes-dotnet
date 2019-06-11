namespace Runes.Collections
{
    public interface ICollectionBuilder<A, CC> : IBuilder<CC> where CC : CollectionBase<A, CC>
    {
        void Add(A item);
    }
}
