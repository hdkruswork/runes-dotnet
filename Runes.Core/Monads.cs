using System;
using System.Collections;
using System.Collections.Generic;
using Runes.Collections;
using Runes.Math;
using static Runes.Predef;

namespace Runes
{
    public interface IMonad<A> : IIterable<A>
    {
        IMonad<B> FlatMap<B>(Func<A, IMonad<B>> f);
        bool GetIfPresent(out A value);
        A GetOrElse(A alternative);
        A GetOrElse(Func<A> alternative);
        IMonad<A> OrElse(IMonad<A> alternative);
    }

    public interface IMonad<A, MM> : IMonad<A> where MM : IMonad<A, MM>
    {
        new MM Filter(Func<A, bool> p);
        new MM FilterNot(Func<A, bool> p);
        MM OrElse(MM alternative);
    }

    public abstract class MonadBase<A, MM> : IMonad<A, MM> where MM : MonadBase<A, MM>
    {
        public bool IsEmpty => !NonEmpty;
        public bool NonEmpty => GetIfPresent(out _);

        public bool Contains(A item) => GetIfPresent(out var value) && value.Equals(item);
        public MM Drops(Int count) => count == 0 ? This : GetEmpty();
        public MM DropsWhile(Func<A, bool> p) => Exists(p) ? GetEmpty() : This;
        public MM DropsWhileNot(Func<A, bool> p) => ExistsNot(p) ? GetEmpty() : This;
        public MM DropsWhile(Func<A, bool> p, out Int dropped)
        {
            if (Exists(p))
            {
                dropped = 1;
                return GetEmpty();
            }

            dropped = 0;
            return This;
        }
        public MM DropsWhileNot(Func<A, bool> p, out Int dropped)
        {
            if (ExistsNot(p))
            {
                dropped = 1;
                return GetEmpty();
            }

            dropped = 0;
            return This;
        }
        public bool Exists(Func<A, bool> p) => Exists(p, true);
        public bool ExistsNot(Func<A, bool> p) => Exists(p, false);
        public MM Filter(Func<A, bool> p) => Exists(p) ? This : GetEmpty();
        public MM FilterNot(Func<A, bool> p) => ExistsNot(p) ? This : GetEmpty();
        public virtual That FoldLeft<That>(That initialValue, Func<That, A, That> f) =>
            GetIfPresent(out var head)
                ? f(initialValue, head)
                : initialValue;
        public virtual That FoldLeftWhile<That>(That initialValue, Func<That, A, bool> p, Func<That, A, That> f) =>
            GetIfPresent(out var head) && p(initialValue, head)
                ? f(initialValue, head)
                : initialValue;
        public bool ForAll(Func<A, bool> p) => !GetIfPresent(out var value) || p(value);
        public Unit Foreach(Action<A> action) => Unit(() =>
        {
            if (GetIfPresent(out var value))
            {
                action(value);
            }
        });
        public Unit ForeachWhile(Func<A, bool> p, Action<A> action) => Unit(() =>
        {
            if (GetIfPresent(out var head) && p(head))
            {
                action(head);
            }
        });
        public IEnumerator<A> GetEnumerator()
        {
            if (GetIfPresent(out var value))
            {
                yield return value;
            }
        }
        public abstract bool GetIfPresent(out A value);
        public A GetOrElse(A alternative) => GetIfPresent(out var value) ? value : alternative;
        public A GetOrElse(Func<A> alternative) => GetIfPresent(out var value) ? value : alternative();
        public MM OrElse(MM alternative) => NonEmpty ? This : alternative;
        public (MM, MM) Partition(Func<A, bool> p)
        {
            var leftBuilder = GetFactory().NewBuilder();
            var rightBuilder = GetFactory().NewBuilder();

            if (GetIfPresent(out var value))
            {
                if (p(value))
                {
                    leftBuilder.Append(value);
                }
                else
                {
                    rightBuilder.Append(value);
                }

                return (leftBuilder.Build(), rightBuilder.Build());
            }

            return (leftBuilder.GetEmpty(), rightBuilder.GetEmpty());
        }
        public MM Take(Int count) => count > 0 && NonEmpty ? This : GetEmpty();
        public MM TakeWhile(Func<A, bool> p) => Exists(p) ? This : GetEmpty();
        public MM TakeWhileNot(Func<A, bool> p) => ExistsNot(p) ? This : GetEmpty();
        public That To<That>(IFactory<A, That> factory) where That : IIterable<A> => factory.From(this);
        public Array<A> ToArray() => To(Collections.Array<A>.Factory);
        public Collections.List<A> ToList() => To(Collections.List<A>.Factory);
        public A[] ToMutableArray() =>
            GetIfPresent(out var value)
                ? new[] {value}
                : new A[0];
        public Set<A> ToSet() => To(Collections.Set<A>.IterableSet.Factory);
        public Stream<A> ToStream() =>
            GetIfPresent(out var value)
                ? Stream(value)
                : Collections.Stream<A>.Empty;

        // protected members

        protected MM This => (MM) this;

        protected That As<B, That>(IFactory<B, That> factory) where That : IMonad<B>
        {
            var builder = factory.NewBuilder();

            return GetIfPresent(out var value) && value is B cast
                ? builder.Append(cast).Build()
                : builder.GetEmpty();
        }

        protected That Collect<B, That>(Func<A, Option<B>> f, IFactory<B, That> factory) where That : IMonad<B> =>
            GetIfPresent(out var value)
                ? factory.From(f(value))
                : factory.NewBuilder().GetEmpty();

        protected bool Exists(Func<A, bool> p, bool isAffirmative) =>
            GetIfPresent(out var value) && p(value) == isAffirmative;

        protected MM Filter(Func<A, bool> p, bool isAffirmative) =>
            GetIfPresent(out var value) && p(value) == isAffirmative ? This : GetFactory().NewBuilder().Build();

        protected That FlatMap<B, That>(Func<A, IIterable<B>> f, IFactory<B, That> factory) where That : IMonad<B> => 
            GetIfPresent(out var value)
                ? factory.From(f(value))
                : factory.NewBuilder().GetEmpty();

        protected MM GetEmpty() => GetFactory().NewBuilder().GetEmpty();

        protected abstract IFactory<A, MM> GetFactory();

        protected abstract IFactory<B, IMonad<B>> GetFactory<B>();

        protected That Map<B, That>(Func<A, B> f, IFactory<B, That> factory) where That : IMonad<B> =>
            GetIfPresent(out var value)
                ? factory.NewBuilder().Append(f(value)).Build()
                : factory.NewBuilder().GetEmpty();

        protected (Left, Right) Unzip<X, Y, Left, Right>(Func<A, (X, Y)> f, IFactory<X, Left> leftFactory, IFactory<Y, Right> rightFactory)
            where Left : IIterable<X>
            where Right : IIterable<Y>
        {
            var xBuilder = leftFactory.NewBuilder();
            var yBuilder = rightFactory.NewBuilder();
            if (GetIfPresent(out var head))
            {
                var (x, y) = f(head);
                xBuilder.Append(x);
                yBuilder.Append(y);
            }

            return (xBuilder.Build(), yBuilder.Build());
        }

        protected That Zip<B, That>(IIterable<B> other, IFactory<(A, B), That> factory)
            where That : IIterable<(A, B)>
        {
            var builder = factory.NewBuilder();
            if (GetIfPresent(out var a) && other.HeadOption.GetIfPresent(out var b))
            {
                builder.Append((a, b));
            }

            return builder.Build();
        }

        public abstract class MonadBuilder<MB> : IIterableBuilder<A, MM> where MB : MonadBuilder<MB>
        {
            protected Collections.List<A> innerList = Collections.List<A>.Empty;

            public MB Append(A item) => SetValue(item);

            public abstract MM Build();

            public abstract MM GetEmpty();

            public MB SetValue(A value)
            {
                innerList = List(value);

                return This;
            }

            protected MB This => (MB) this;

            IIterableBuilder<A, MM> IIterableBuilder<A, MM>.Append(A item) => Append(item);

            IIterableBuilder<A> IIterableBuilder<A>.Append(A item) => Append(item);

            IIterable<A> IBuilder<IIterable<A>>.Build() => Build();

            IIterable<A> IIterableBuilder<A>.GetEmpty() => GetEmpty();
        }

        // private members

        IMonad<A> IMonad<A>.OrElse(IMonad<A> alternative) => OrElse(GetFactory().From(alternative));

        Option<A> IIterable<A>.HeadOption => Option<A>.Factory.From(This);

        IIterable<A> IIterable<A>.Tail => GetFactory().NewBuilder().GetEmpty();

        IIterable<B> IIterable<A>.As<B>() => As(GetFactory<B>());

        IIterable<B> IIterable<A>.Collect<B>(Func<A, Option<B>> f) => Collect(f, GetFactory<B>());

        IIterable<A> IIterable<A>.Drops(Int count) => Drops(count);

        IIterable<A> IIterable<A>.DropsWhile(Func<A, bool> p) => DropsWhile(p);

        IIterable<A> IIterable<A>.DropsWhileNot(Func<A, bool> p) => DropsWhileNot(p);

        IIterable<A> IIterable<A>.DropsWhile(Func<A, bool> p, out Int dropped) => DropsWhile(p, out dropped);

        IIterable<A> IIterable<A>.DropsWhileNot(Func<A, bool> p, out Int dropped) => DropsWhileNot(p, out dropped);

        IIterable<A> IIterable<A>.Filter(Func<A, bool> p) => Filter(p);

        IIterable<A> IIterable<A>.FilterNot(Func<A, bool> p) => FilterNot(p);

        IIterable<B> IIterable<A>.FlatMap<B>(Func<A, IIterable<B>> f) => FlatMap(f, GetFactory<B>());

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        IIterable<B> IIterable<A>.Map<B>(Func<A, B> f) => Map(f, GetFactory<B>());

        (IIterable<A>, IIterable<A>) IIterable<A>.Partition(Func<A, bool> p) => Partition(p);

        IIterable<A> IIterable<A>.Take(Int count) => Take(count);

        IIterable<A> IIterable<A>.TakeWhile(Func<A, bool> p) => TakeWhile(p);

        IIterable<A> IIterable<A>.TakeWhileNot(Func<A, bool> p) => TakeWhileNot(p);

        (IIterable<X>, IIterable<Y>) IIterable<A>.Unzip<X, Y>(Func<A, (X, Y)> f) =>
            Unzip(f, GetFactory<X>(), GetFactory<Y>());

        IIterable<(A, B)> IIterable<A>.Zip<B>(IIterable<B> other) => Zip(other, GetFactory<(A, B)>());

        Collections.ICollection<A> Collections.ICollection<A>.Filter(Func<A, bool> p) => Filter(p);

        Collections.ICollection<A> Collections.ICollection<A>.FilterNot(Func<A, bool> p) => FilterNot(p);

        IMonad<B> IMonad<A>.FlatMap<B>(Func<A, IMonad<B>> f) => FlatMap(f, GetFactory<B>());
    }
}
