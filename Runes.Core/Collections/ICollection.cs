using System;

namespace Runes.Collections
{
    public interface ICollection
    {
        bool IsEmpty { get; }
        bool NonEmpty { get; }

        bool Contains(object item);
        ICollection Filter(Func<object, bool> p);
        ICollection FilterNot(Func<object, bool> p);
    }

    public interface ICollection<A> : ICollection
    {
        bool Contains(A item);
        ICollection<A> Filter(Func<A, bool> p);
        ICollection<A> FilterNot(Func<A, bool> p);

        bool ICollection.Contains(object item) => item is A a && Contains(a);
        ICollection ICollection.Filter(Func<object, bool> p) => Filter((A a) => p(a));
        ICollection ICollection.FilterNot(Func<object, bool> p) => FilterNot((A a) => p(a));
    }

    public interface ICollection<A, out CC> : ICollection<A> where CC : ICollection<A, CC>
    {
        new CC Filter(Func<A, bool> p);
        new CC FilterNot(Func<A, bool> p);

        ICollection<A> ICollection<A>.Filter(Func<A, bool> p) => Filter(p);
        ICollection<A> ICollection<A>.FilterNot(Func<A, bool> p) => FilterNot(p);
    }
}
