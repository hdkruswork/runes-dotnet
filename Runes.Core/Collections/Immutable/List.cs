using System;
using static Runes.Collections.Immutable.Arrays;
using static Runes.Predef;

namespace Runes.Collections.Immutable
{
    public static class Lists
    {
        public static List<A> List<A>(params A[] values) => ImmutableArray(values).ToList();
    }

    public abstract class List<A> : SeqLike<A, List<A>>
    {
        public static readonly List<A> Empty = new EmptyList<A>();

        public List<B> Collect<B>(IPartialFunction<A, B> pf) => Collect(pf, List<B>.Empty.NewBuilder());

        public Option<A> GetAt(int idx)
        {
            var currIdx = idx >= 0 ? 0 : -Size();
            var curr = this;
            while (currIdx < idx && curr.NonEmpty)
            {
                curr = curr.Tail;
                currIdx += 1;
            }
            return curr.HeadOption;
        }

        public List<B> FlatMap<B, That>(Func<A, That> f) where That : ITraversable<B> => FlatMap(f, List<B>.Empty.NewBuilder());

        public List<B> Map<B>(Func<A, B> f) => Map(f, List<B>.Empty.NewBuilder());

        public override List<A> Prepend(A e) => new ConsList<A>(e, this);

        public List<(A, B)> Zip<B>(List<B> other) => Zip(other, List<(A, B)>.Empty.NewBuilder());

        public List<(A, int)> ZipWithIndex() => ZipWithIndex(List<(A, int)>.Empty.NewBuilder());

        protected override ITraversableBuilder<A, List<A>> NewBuilder() => new ListBuilder<A>();
    }

    public sealed class EmptyList<A> : List<A>
    {
        public override Option<A> HeadOption => None<A>();

        public override List<A> Tail => this;

        public override string ToString() => string.Empty;

        internal EmptyList() { }
    }

    internal class ConsList<A> : List<A>
    {
        public ConsList(A head, List<A> tail)
        {
            Head = head;
            Tail = tail;
        }

        public A Head { get; }

        public override Option<A> HeadOption => Some(Head);
        public override List<A> Tail { get; }

        public override string ToString() =>
            $"{Head}{(Tail.NonEmpty ? $", {Tail}" : string.Empty)}";
    }

    public sealed class ListBuilder<T> : TraversableBuilder<T, List<T>>
    {
        public override List<T> Build() =>
            stream
                .ToImmutableArray()
                .FoldRight(List<T>.Empty, (list, it) => list.Prepend(it));
    }
}
