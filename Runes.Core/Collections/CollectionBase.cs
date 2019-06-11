using System;

namespace Runes.Collections
{
    public abstract class CollectionBase<A, CC> : ICollection<A, CC>
        where CC : CollectionBase<A, CC>
    {
        public abstract Option<A> HeadOption { get; }
        public abstract CC Tail { get; }

        public bool IsEmpty => HeadOption.IsEmpty;
        public bool NonEmpty => !IsEmpty;

        public virtual bool Contains(A item) => Exist(it => Equals(it, item));
        public abstract CC Drops(int count);
        public abstract CC DropsWhile(Func<A, bool> p);
        public abstract CC DropsWhile(Func<A, bool> p, out int skipped);
        public abstract CC DropsWhileNot(Func<A, bool> p);
        public abstract CC DropsWhileNot(Func<A, bool> p, out int skipped);
        public virtual bool Exist(Func<A, bool> p) => Filter(p).NonEmpty;
        public abstract CC Filter(Func<A, bool> p);
        public abstract CC FilterNot(Func<A, bool> p);
        public abstract CC Take(int count);
        public abstract CC TakeWhile(Func<A, bool> p);
        public abstract CC TakeWhileNot(Func<A, bool> p);

        // protected members

        protected CC This => (CC)this;

        protected abstract ICollection<B> CollectionAs<B>() where B : class;
        protected abstract ICollection<B> CollectionCollect<B>(Func<A, Option<B>> f);
        protected abstract ICollection<B> CollectionFlatMap<B>(Func<A, ICollection<B>> f);
        protected abstract ICollection<B> CollectionMap<B>(Func<A, B> f);
        protected abstract (ICollection<X>, ICollection<Y>) CollectionUnzip<X, Y>(Func<A, (X, Y)> toPairFunc);
        protected abstract ICollection<(A, B)> CollectionZip<B>(ICollection<B> other);
        protected abstract ICollection<(A, int)> CollectionZipWithIndex();

        // private members

        ICollection<A> ICollection<A>.Tail => Tail;

        ICollection<B> ICollection<A>.As<B>() => CollectionAs<B>();
        ICollection<B> ICollection<A>.Collect<B>(Func<A, Option<B>> f) => CollectionCollect(f);
        ICollection<A> ICollection<A>.Drops(int count) => Drops(count);
        ICollection<A> ICollection<A>.DropsWhile(Func<A, bool> p) => DropsWhile(p);
        ICollection<A> ICollection<A>.DropsWhile(Func<A, bool> p, out int skipped) => DropsWhile(p, out skipped);
        ICollection<A> ICollection<A>.DropsWhileNot(Func<A, bool> p) => DropsWhileNot(p);
        ICollection<A> ICollection<A>.DropsWhileNot(Func<A, bool> p, out int skipped) => DropsWhileNot(p, out skipped);
        ICollection<A> ICollection<A>.Filter(Func<A, bool> p) => Filter(p);
        ICollection<A> ICollection<A>.FilterNot(Func<A, bool> p) => FilterNot(p);
        ICollection<B> ICollection<A>.FlatMap<B>(Func<A, ICollection<B>> f) => CollectionFlatMap(f);
        ICollection<B> ICollection<A>.Map<B>(Func<A, B> f) => CollectionMap(f);
        ICollection<A> ICollection<A>.Take(int count) => Take(count);
        ICollection<A> ICollection<A>.TakeWhile(Func<A, bool> p) => TakeWhile(p);
        ICollection<A> ICollection<A>.TakeWhileNot(Func<A, bool> p) => TakeWhileNot(p);
        (ICollection<X>, ICollection<Y>) ICollection<A>.Unzip<X, Y>(Func<A, (X, Y)> toPairFunc) => CollectionUnzip(toPairFunc);
        ICollection<(A, B)> ICollection<A>.Zip<B>(ICollection<B> other) => CollectionZip(other);
        ICollection<(A, int)> ICollection<A>.ZipWithIndex() => CollectionZipWithIndex();
    }
}
