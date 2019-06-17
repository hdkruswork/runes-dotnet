using System;

namespace Runes.Collections
{
    public interface ICollection<A>
    {
        bool IsEmpty { get; }
        bool NonEmpty { get; }

        bool Contains(A item);
        ICollection<A> Filter(Func<A, bool> p);
        ICollection<A> FilterNot(Func<A, bool> p);
    }

    public interface ICollection<A, out CC> : ICollection<A> where CC : ICollection<A, CC>
    {
        new CC Filter(Func<A, bool> p);
        new CC FilterNot(Func<A, bool> p);
    }
}
