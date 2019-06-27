namespace Runes.Collections
{
    public interface ISet<A> : ICollection<A>
    {
        ISet<A> Difference(ISet<A> other);
        ISet<A> Intersection(ISet<A> other);
        ISet<A> Union(ISet<A> other);
    }

    public interface ISet<A, CC> : ISet<A>, ICollection<A, CC> where CC : ISet<A, CC>
    {
        CC Difference(CC other);
        CC Intersection(CC other);
        CC Union(CC other);
    }
}
