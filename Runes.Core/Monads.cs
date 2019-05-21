using Runes.Collections;
using Runes.Collections.Immutable;
using System;
using System.Collections.Generic;

using static Runes.Collections.Immutable.Streams;
using static Runes.Options;
using static Runes.Units;

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

        public override Unit Foreach(Action<A> action) => Unit(() =>
        {
            if (GetIfPresent(out A item))
            {
                action(item);
            }
        });

        public override Unit ForeachWhile(Func<A, bool> p, Action<A> action) => Unit(() =>
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

        public override Stream<A> ToStream() =>
            GetIfPresent(out A item) ? Stream(item) : Streams.Empty<A>();

        // Protected members

        protected abstract IMonadBuilder<A, Repr> GetBuilder();

        protected That FlatMap<B, That>(Func<A, IMonad<B>> f, IMonadBuilder<B, That> builder) where That: IMonad<B> =>
            GetIfPresent(out A value)
                ? builder.SetValueFrom(f(value)).Build()
                : builder.Clear().Build();

        protected That Map<B, That>(Func<A, B> f, IMonadBuilder<B, That> builder) where That: IMonad<B> =>
            GetIfPresent(out A value)
                ? builder.SetValue(f(value)).Build()
                : builder.Clear().Build();

        protected That Zip<B, That>(Option<B> other, IMonadBuilder<(A, B), That> builder) where That: IMonad<(A, B)> =>
            GetIfPresent(out A thisValue) && other.GetIfPresent(out B otherValue)
                ? builder.SetValue((thisValue, otherValue)).Build()
                : builder.Clear().Build();
        
        protected That ZipWithIndex<That>(IMonadBuilder<(A, int), That> builder) where That: IMonad<(A, int)> =>
            GetIfPresent(out A thisValue)
                ? builder.SetValue((thisValue, 0)).Build()
                : builder.Clear().Build();

        // Private members

        private Repr Filter(Func<A, bool> p, bool isTruthly)
        {
            var builder = GetBuilder();
            return GetIfPresent(out A value) && p(value) == isTruthly
                ? builder.SetValueFrom(this).Build()
                : builder.Clear().Build();
        }

        That IMonad<A>.FlatMap<B, That>(Func<A, IMonad<B>> f, IMonadBuilder<B, That> builder) => FlatMap(f, builder);
        
        That IMonad<A>.Map<B, That>(Func<A, B> f, IMonadBuilder<B, That> builder) => Map(f, builder);

        That IMonad<A>.Zip<B, That>(Option<B> other, IMonadBuilder<(A, B), That> builder) => Zip(other, builder);

        That IMonad<A>.ZipWithIndex<That>(IMonadBuilder<(A, int), That> builder) => ZipWithIndex(builder);
    }

    public interface IMonadBuilder<A, M> : IBuilder<M> where M : IMonad<A>
    {
        IMonadBuilder<A, M> Clear();

        IMonadBuilder<A, M> SetValue(A value);

        IMonadBuilder<A, M> SetValueFrom(IMonad<A> other);
    }

    public abstract class MonadBuilder<A, M> : IMonadBuilder<A, M> where M : IMonad<A>
    {
        private Option<A> option = null;

        public IMonadBuilder<A, M> Clear()
        {
            option = None<A>();

            return this;
        }

        public IMonadBuilder<A, M> SetValue(A value)
        {
            option = Some(value);

            return this;
        }

        public IMonadBuilder<A, M> SetValueFrom(IMonad<A> other) =>
            other.GetIfPresent(out A value)
                ? SetValue(value)
                : Clear();

        public abstract M Build();

        protected Option<A> GetOption() => option ?? None<A>();
    }
}
