using System;

namespace Runes.Collections
{
    public interface ISet<A> : ICollection<A>
    {
        new ISet<A> Filter(Func<A, bool> p);
        new ISet<A> FilterNot(Func<A, bool> p);

        ISet<A> Difference(ISet<A> other);
        ISet<A> Intersection(ISet<A> other);
        ISet<A> Union(ISet<A> other);

        // explicit members

        ICollection<A> ICollection<A>.Filter(Func<A, bool> p) => Filter(p);
        ICollection<A> ICollection<A>.FilterNot(Func<A, bool> p) => FilterNot(p);
    }

    public interface ISet<A, CC> : ISet<A>, ICollection<A, CC> where CC : ISet<A, CC>
    {
        new CC Filter(Func<A, bool> p);
        new CC FilterNot(Func<A, bool> p);

        CC Difference(CC other);
        CC Intersection(CC other);
        CC Union(CC other);

        // explicit members

        ISet<A> ISet<A>.Filter(Func<A, bool> p) => Filter(p);
        ISet<A> ISet<A>.FilterNot(Func<A, bool> p) => FilterNot(p);

        ISet<A> ISet<A>.Difference(ISet<A> other) => Difference((CC)other);
        ISet<A> ISet<A>.Intersection(ISet<A> other) => Intersection((CC)other);
        ISet<A> ISet<A>.Union(ISet<A> other) => Union((CC)other);
    
        ICollection<A> ICollection<A>.Filter(Func<A, bool> p) => Filter(p);
        ICollection<A> ICollection<A>.FilterNot(Func<A, bool> p) => FilterNot(p);
    }
}
