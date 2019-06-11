using System;

namespace Runes.Collections
{
    public interface ICollection<A>
    {
        Option<A> HeadOption { get; }
        ICollection<A> Tail { get; }

        bool IsEmpty { get; }
        bool NonEmpty { get; }

        ICollection<B> As<B>() where B : class;
        bool Contains(A item);
        ICollection<B> Collect<B>(Func<A, Option<B>> f);
        ICollection<A> Drops(int count);
        ICollection<A> DropsWhile(Func<A, bool> p);
        ICollection<A> DropsWhileNot(Func<A, bool> p);
        ICollection<A> DropsWhile(Func<A, bool> p, out int skipped);
        ICollection<A> DropsWhileNot(Func<A, bool> p, out int skipped);
        bool Exist(Func<A, bool> p);
        ICollection<A> Filter(Func<A, bool> p);
        ICollection<A> FilterNot(Func<A, bool> p);
        ICollection<B> FlatMap<B>(Func<A, ICollection<B>> f);
        ICollection<B> Map<B>(Func<A, B> f);
        ICollection<A> Take(int count);
        ICollection<A> TakeWhile(Func<A, bool> p);
        ICollection<A> TakeWhileNot(Func<A, bool> p);
        (ICollection<X>, ICollection<Y>) Unzip<X, Y>(Func<A, (X, Y)> toPairFunc);
        ICollection<(A, B)> Zip<B>(ICollection<B> other);
        ICollection<(A, int)> ZipWithIndex();
    }

    public interface ICollection<A, CC> : ICollection<A> where CC : ICollection<A, CC>
    {
        new CC Tail { get; }

        new CC Drops(int count);
        new CC DropsWhile(Func<A, bool> p);
        new CC DropsWhileNot(Func<A, bool> p);
        new CC DropsWhile(Func<A, bool> p, out int skipped);
        new CC DropsWhileNot(Func<A, bool> p, out int skipped);
        new CC Filter(Func<A, bool> p);
        new CC FilterNot(Func<A, bool> p);
        new CC Take(int count);
        new CC TakeWhile(Func<A, bool> p);
        new CC TakeWhileNot(Func<A, bool> p);
    }
}
