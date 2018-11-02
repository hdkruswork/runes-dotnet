using Runes.Collections;
using Runes.Collections.Immutable;
using System;
using System.Collections.Generic;

namespace Runes
{
    public interface IMonad<A> : ITraversable<A>
    {
        bool IsEmpty { get; }
        bool NonEmpty { get; }

        bool Contains(A item);
        bool Exists(Func<A, bool> p);
        That FlatMap<B, That>(Func<A, IMonad<B>> f, IMonadBuilder<B, That> builder) where That : IMonad<B>;
        bool ForAll(Func<A, bool> p);
        bool GetIfPresent(out A value);
        That Map<B, That>(Func<A, B> f, IMonadBuilder<B, That> builder) where That : IMonad<B>;
        That Zip<B, That>(Option<B> other, IMonadBuilder<(A, B), That> builder) where That: IMonad<(A, B)>;
        That ZipWithIndex<That>(IMonadBuilder<(A, int), That> builder) where That: IMonad<(A, int)>;
    }

    public interface IMonadLike<A, Repr>: IMonad<A> where Repr: IMonadLike<A, Repr>
    {
        Repr Filter(Func<A, bool> p);
        Repr FilterNot(Func<A, bool> p);
    }

    public abstract class MonadLike<A, Repr>: Traversable<A>, IMonadLike<A, Repr> where Repr: IMonadLike<A, Repr>
    {
        public abstract bool IsEmpty { get; }

        public bool NonEmpty => !IsEmpty;

        public bool Contains(A item) => GetIfPresent(out A value) && Equals(value, item);

        public bool Exists(Func<A, bool> p) => GetIfPresent(out A value) && p(value);

        public Repr Filter(Func<A, bool> p) => Filter(p, true);

        public Repr FilterNot(Func<A, bool> p) => Filter(p, false);
        
        public bool ForAll(Func<A, bool> p) => !GetIfPresent(out A value) || p(value);

        public abstract bool GetIfPresent(out A value);

        public override Unit Foreach(Action<A> action) => Unit.Of(() =>
        {
            if (GetIfPresent(out A item))
            {
                action(item);
            }
        });

        public override Unit ForeachWhile(Func<A, bool> p, Action<A> action) => Unit.Of(() =>
        {
            if (GetIfPresent(out A item) && p(item))
            {
                action(item);
            }
        });

        public override IEnumerator<A> GetEnumerator()
        {
            if (GetIfPresent(out A item))
            {
                yield return item;
            }
        }

        public override IStream<A> ToStream() =>
            GetIfPresent(out A item) ? Stream.From(item) : Stream.Empty<A>();

        // Protected members

        protected abstract IMonadBuilder<A, Repr> GetBuilder();

        protected That FlatMap<B, That>(Func<A, IMonad<B>> f, IMonadBuilder<B, That> builder) where That: IMonad<B> =>
            GetIfPresent(out A value) ? builder.TransformFrom(f(value)) : builder.Empty();

        protected That Map<B, That>(Func<A, B> f, IMonadBuilder<B, That> builder) where That: IMonad<B> =>
            GetIfPresent(out A value) ? builder.BuildFrom(f(value)) : builder.Empty();

        protected That Zip<B, That>(Option<B> other, IMonadBuilder<(A, B), That> builder) where That: IMonad<(A, B)> =>
            GetIfPresent(out A thisValue) && other.GetIfPresent(out B otherValue)
                ? builder.BuildFrom((thisValue, otherValue))
                : builder.Empty();
        
        protected That ZipWithIndex<That>(IMonadBuilder<(A, int), That> builder) where That: IMonad<(A, int)> =>
            GetIfPresent(out A thisValue) ? builder.BuildFrom((thisValue, 0)) : builder.Empty();

        // Private members

        private Repr Filter(Func<A, bool> p, bool isTruthly)
        {
            var builder = GetBuilder();
            return GetIfPresent(out A value) && p(value) == isTruthly
                ? builder.TransformFrom(this)
                : builder.Empty();
        }

        That IMonad<A>.FlatMap<B, That>(Func<A, IMonad<B>> f, IMonadBuilder<B, That> builder) => FlatMap(f, builder);
        
        That IMonad<A>.Map<B, That>(Func<A, B> f, IMonadBuilder<B, That> builder) => Map(f, builder);

        That IMonad<A>.Zip<B, That>(Option<B> other, IMonadBuilder<(A, B), That> builder) => Zip(other, builder);

        That IMonad<A>.ZipWithIndex<That>(IMonadBuilder<(A, int), That> builder) => ZipWithIndex(builder);
    }

    public interface IMonadBuilder<A, MM>: IBuilderLike<A, MM, IMonadBuilder<A, MM>> where MM: IMonad<A>
    {
        MM TransformFrom(IMonad<A> other);
    }

    public abstract class MonadBuilder<A, MM> : BuilderLike<A, MM, MonadBuilder<A, MM>>, IMonadBuilder<A, MM> where MM : MonadLike<A, MM>
    {
        public virtual MM TransformFrom(IMonad<A> other) => other.GetIfPresent(out A value) ? BuildFrom(value) : Empty();

        public override MonadBuilder<A, MM> NewBuilder() => this;

        IMonadBuilder<A, MM> IBuilderLike<A, MM, IMonadBuilder<A, MM>>.NewBuilder() => NewBuilder();
    }
}
