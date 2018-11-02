using System;

namespace Runes.Collections.Immutable
{
    public static class Seq
    {
        public static Seq<A> Empty<A>() => Seq<A>.Empty;

        public static Seq<A> From<A>(params A[] values)
        {
            var seq = Empty<A>();
            for (int i = values.Length - 1; i >= 0; i--)
            {
                seq = seq.Prepend(values[i]);
            }
            return seq;
        }
    }

    public abstract class Seq<A> : SeqLike<A, Seq<A>>
    {
        internal static readonly EmptySeq<A> Empty = new EmptySeq<A>();

        public Seq<B> Collect<B>(IPartialFunction<A, B> pf) => Collect(pf, Seq.Empty<B>().NewBuilder());

        public Seq<B> FlatMap<B, That>(Func<A, That> f) where That: ITraversable<B> => FlatMap(f, Seq.Empty<B>().NewBuilder());

        public Seq<B> Map<B>(Func<A, B> f) => Map(f, Seq.Empty<B>().NewBuilder());

        public override Seq<A> Prepend(A e) => new ConsSeq<A>(e, this);

        public Seq<(A, B)> Zip<B>(Seq<B> other) => Zip(other, Seq.Empty<(A, B)>().NewBuilder());

        public Seq<(A, int)> ZipWithIndex() => ZipWithIndex(Seq.Empty<(A, int)>().NewBuilder());

        protected override ICollectionBuilder<A, Seq<A>> NewBuilder() => new SeqBuilder<A>();

        // Private members

        private sealed class SeqBuilder<T> : CollectionBuilder<T, Seq<T>>, IBuilder<T, Seq<T>>
        {
            public SeqBuilder() : base() {}
            public SeqBuilder(T rear, CollectionBuilder<T, Seq<T>> init) : base(rear, init) { }

            public override CollectionBuilder<T, Seq<T>> NewBuilder() => new SeqBuilder<T>();

            public override Seq<T> Empty() => Seq.Empty<T>();

            protected override CollectionBuilder<T, Seq<T>> NewBuilder(T rear, CollectionBuilder<T, Seq<T>> init) =>
                new SeqBuilder<T>(rear, init);
        }
    }

    public sealed class EmptySeq<A> : Seq<A>
    {
        internal EmptySeq() { }

        public override Option<A> HeadOption => Option.None<A>();

        public override Seq<A> Tail => this;
    }

    internal class ConsSeq<A> : Seq<A>
    {
        public ConsSeq(A head, Seq<A> tail)
        {
            Head = head;
            Tail = tail;
        }

        public A Head { get; }

        public override Option<A> HeadOption => Option.Some(Head);
        public override Seq<A> Tail { get; }
    }
}
