using System;

using static Runes.Predef;

namespace Runes
{
    public interface IMonad<A>
    {
        bool IsEmpty { get; }
        bool NonEmpty { get; }

        bool Contains(A item);
        IMonad<B> Collect<B>(Func<A, Option<B>> f);
        bool Exist(Func<A, bool> p);
        IMonad<A> Filter(Func<A, bool> p);
        IMonad<A> FilterNot(Func<A, bool> p);
        IMonad<B> FlatMap<B>(Func<A, IMonad<B>> f);
        Unit Foreach(Action<A> action);
        bool GetIfPresent(out A value);
        A GetOrElse(A alternative);
        IMonad<B> Map<B>(Func<A, B> f);
        (IMonad<X>, IMonad<Y>) Unzip<X, Y>(Func<A, (X, Y)> toPairFunc);
        IMonad<(A, B)> Zip<B>(IMonad<B> other);
    }

    public interface IMonad<A, MM> : IMonad<A> where MM : IMonad<A, MM>
    {
        new MM Filter(Func<A, bool> p);
        new MM FilterNot(Func<A, bool> p);
    }

    public abstract class MonadBase<A, MM> : IMonad<A, MM> where MM : MonadBase<A, MM>
    {
        public abstract bool IsEmpty { get; }
        public bool NonEmpty => !IsEmpty;

        public bool Contains(A item) => Exist(it => Equals(item, it));
        public bool Exist(Func<A, bool> p) => GetIfPresent(out A value) && p(value);
        public abstract MM Filter(Func<A, bool> p);
        public abstract MM FilterNot(Func<A, bool> p);
        public Unit Foreach(Action<A> action) => Unit(() =>
        {
            if (GetIfPresent(out A value))
            {
                action(value);
            }
        });
        public abstract bool GetIfPresent(out A value);
        public A GetOrElse(A alternative) => GetIfPresent(out A value) ? value : alternative;

        // protected members

        protected MM This => (MM)this;

        protected That Collect<B, That, MB>(Func<A, Option<B>> f, MB builder)
            where That : MonadBase<B, That>
            where MB : IMonadBuilder<B, That> => FlatMap<B, That, MB>(f, builder);
        protected MM Filter<MB>(Func<A, bool> p, bool isTruthly, MB builder) where MB : IMonadBuilder<A, MM> =>
            Exist(p) == isTruthly
                ? This
                : builder.Build();
        protected That FlatMap<B, That, MB>(Func<A, IMonad<B>> f, MB builder)
            where That : MonadBase<B, That>
            where MB : IMonadBuilder<B, That>
        {
            if (GetIfPresent(out A a) && f(a).GetIfPresent(out B b))
            {
                builder.SetValue(b);
            }

            return builder.Build();
        }
        protected That Map<B, That, MB>(Func<A, B> f, MB builder)
            where That : MonadBase<B, That>
            where MB : IMonadBuilder<B, That>
        {
            if (GetIfPresent(out A value))
            {
                builder.SetValue(f(value));
            }

            return builder.Build();
        }
        protected (Left, Right) Unzip<X, Y, Left, Right, LeftBuilder, RightBuilder>(
            Func<A, (X, Y)> toPairFunc,
            LeftBuilder leftBuilder,
            RightBuilder rightBuilder)
            where Left : MonadBase<X, Left>
            where Right : MonadBase<Y, Right>
            where LeftBuilder : IMonadBuilder<X, Left>
            where RightBuilder : IMonadBuilder<Y, Right>
        {
            if (GetIfPresent(out A value))
            {
                (var x, var y) = toPairFunc(value);

                leftBuilder.SetValue(x);
                rightBuilder.SetValue(y);
            }

            return (leftBuilder.Build(), rightBuilder.Build());
        }
        protected Zipped Zip<B, Zipped, MB>(IMonad<B> other, MB builder)
            where Zipped : MonadBase<(A, B), Zipped>
            where MB : IMonadBuilder<(A, B), Zipped>
        {
            if (GetIfPresent(out A a) && other.GetIfPresent(out B b))
            {
                builder.SetValue((a, b));
            }

            return builder.Build();
        }

        protected abstract IMonad<B> MonadCollect<B>(Func<A, Option<B>> f);
        protected abstract IMonad<B> MonadFlatMap<B>(Func<A, IMonad<B>> f);
        protected abstract IMonad<B> MonadMap<B>(Func<A, B> f);
        protected abstract (IMonad<X>, IMonad<Y>) MonadUnzip<X, Y>(Func<A, (X, Y)> toPairFunc);
        protected abstract IMonad<(A, B)> MonadZip<B>(IMonad<B> other);

        // private members

        IMonad<B> IMonad<A>.Collect<B>(Func<A, Option<B>> f) => MonadCollect(f);
        IMonad<A> IMonad<A>.Filter(Func<A, bool> p) => Filter(p);
        IMonad<A> IMonad<A>.FilterNot(Func<A, bool> p) => FilterNot(p);
        IMonad<B> IMonad<A>.FlatMap<B>(Func<A, IMonad<B>> f) => MonadFlatMap(f);
        IMonad<B> IMonad<A>.Map<B>(Func<A, B> f) => MonadMap(f);
        (IMonad<X>, IMonad<Y>) IMonad<A>.Unzip<X, Y>(Func<A, (X, Y)> toPairFunc) => MonadUnzip(toPairFunc);
        IMonad<(A, B)> IMonad<A>.Zip<B>(IMonad<B> other) => MonadZip(other);
    }

    public interface IMonadBuilder<A, MM> : IBuilder<MM> where MM : IMonad<A, MM>
    {
        void SetValue(A value);
    }
}
