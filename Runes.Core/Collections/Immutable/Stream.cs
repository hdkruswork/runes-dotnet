using System;
using System.Collections.Generic;
using System.Numerics;

namespace Runes.Collections.Immutable
{
    public static class Stream
    {
        public static Stream<A> Empty<A>() => Stream<A>.Empty;

        public static Stream<int> From(int head) => Of(head, Lazy.From(() => From(head + 1)));

        public static Stream<A> Of<A>(A head, Lazy<Stream<A>> tail) => new ConsStream<A>(head, tail);

        public static Stream<A> Of<A>(A head, Stream<A> tail) => new FixedConsStream<A>(head, tail);

        public static Stream<A> Of<A>(A head) => new FixedConsStream<A>(head, Empty<A>());

        public static Stream<A> Of<A>(IEnumerable<A> e)
        {
            Stream<A> fromIterator(IEnumerator<A> iter) =>
                iter.MoveNext()
                    ? Of(iter.Current, Lazy.From(() => fromIterator(iter)))
                    : Empty<A>();

            return fromIterator(e.GetEnumerator());
        }

        public static Stream<A> Of<A>(params A[] array)
        {
            var res = Empty<A>();
            for (int i = array.Length - 1; i >= 0; i--)
            {
                res = res.Prepend(array[i]);
            }
            return res;
        }

        public static Stream<A> Of<A>(A a1, A a2, Lazy<Stream<A>> tail) => Of(a1).Append(Of(a2, tail));

        public static Stream<A> Flatten<A, That>(this Stream<That> stream) where That: Traversable<A> => 
            stream
                .HeadOption
                .Map(trav => trav.ToStream().Append(stream.Tail.Flatten<A, That>()))
                .GetOrElse(Empty<A>());
    }

    public abstract class Stream<A> : Traversable<A>
    {
        internal static readonly EmptyStream<A> Empty = new EmptyStream<A>();

        public bool IsEmpty => HeadOption.IsEmpty;

        public bool NonEmpty  => HeadOption.NonEmpty;

        public abstract Option<A> HeadOption { get; }

        public abstract Stream<A> Tail { get; }

        public Stream<A> Append(A rear) =>
            HeadOption
                .Map(head => Stream.Of(head, Lazy.From(() => Tail.Append(rear))))
                .GetOrElse(Stream.Of(rear));

        public Stream<A> Append(IEnumerable<A> e) => Append(e is Stream<A> stream ? stream : Stream.Of(e));

        public Stream<A> Append(Stream<A> other) =>
            HeadOption
                .Map(head => Stream.Of(head, Lazy.From(() => Tail.Append(other))))
                .GetOrElse(other);

        public Stream<B> Collect<B>(IPartialFunction<A, B> pf) =>
            FlatMap<B, Option<B>>(it => Code.Try(() => pf.Apply(it)).ToOption());

        public Option<B> CollectFirst<B>(IPartialFunction<A, B> pf) => Collect(pf).HeadOption;

        public bool Correspond<B>(Stream<B> other, Func<A, B, bool> p)
        {
            var streamA = this;
            var streamB = other;
            while (streamA.GetHeadIfPresent(out A headA) && streamB.GetHeadIfPresent(out B headB) && p(headA, headB))
            {
                streamA = streamA.Tail;
                streamB = streamB.Tail;
            }

            return streamA.IsEmpty && streamB.IsEmpty;
        }

        public Stream<A> Drops(int count) =>
            count > 0
                ? HeadOption
                    .Map(head => Stream.Of(head, Lazy.From(() => Tail.Drops(count -1))))
                    .GetOrElse(Stream.Empty<A>())
                : Stream.Empty<A>();

        public Stream<A> DropsWhile(Func<A, bool> p) => DropsWhile(p, out _);

        public Stream<A> DropsWhileNot(Func<A, bool> p) => DropsWhileNot(p, out _);

        public Stream<A> DropsWhile(Func<A, bool> p, out int skipped) => DropsWhile(p, true, out skipped);

        public Stream<A> DropsWhileNot(Func<A, bool> p, out int skipped) => DropsWhile(p, false, out skipped);

        public bool Exists(Func<A, bool> p) => Filter(p).NonEmpty;

        public Stream<A> Filter(Func<A, bool> p) => Filter(this, p, true);

        public Stream<A> FilterNot(Func<A, bool> p) => Filter(this, p, false);

        public Stream<B> FlatMap<B, That>(Func<A, That> f) where That : Traversable<B> =>
            HeadOption
                .Map(head => f(head).ToStream().Append(Tail.FlatMap<B, That>(f)))
                .GetOrElse(Stream.Empty<B>());

        public bool ForAll(Func<A, bool> p)
        {
            var curr = this;
            while (curr.GetHeadIfPresent(out A head) && p(head))
            {
                curr = curr.Tail;
            }
            return curr.IsEmpty;
        }

        public override Unit Foreach(Action<A> action) => Unit.Of(() =>
        {
            var curr = this;
            while (curr.GetHeadIfPresent(out A head))
            {
                action(head);
                curr = curr.Tail;
            }
        });

        public override Unit ForeachWhile(Func<A, bool> p, Action<A> action) => Unit.Of(() =>
        {
            var curr = this;
            while (curr.GetHeadIfPresent(out A head) && p(head))
            {
                action(head);
                curr = curr.Tail;
            }
        });

        public override IEnumerator<A> GetEnumerator()
        {
            var curr = this;
            while(curr.GetHeadIfPresent(out A item))
            {
                yield return item;
                curr = curr.Tail;
            }
        }

        public bool GetHeadIfPresent(out A head) => HeadOption.GetIfPresent(out head);

        public Stream<B> Map<B>(Func<A, B> f) =>
            HeadOption
                .Map(head => Stream.Of(f(head), Lazy.From(() => Tail.Map(f))))
                .GetOrElse(Stream.Empty<B>());

        public Stream<A> Prepend(A e) => Stream.Of(e, this);

        public Stream<A> Prepend(IEnumerable<A> e) =>
            (e is Stream<A> stream ? stream : Stream.Of(e))
                .Append(this);

        public Stream<A> Prepend(Stream<A> other) => other.Append(this);

        public Stream<A> Reverse() => FoldLeft(Stream.Empty<A>(), (acc, it) => acc.Prepend(it));

        public BigInteger Size() => FoldLeft(BigInteger.Zero, (sum, _) => sum + 1);

        public Stream<A> Take(int count) =>
            count > 0
                ? HeadOption
                    .Map(head => Stream.Of(head, Lazy.From(() => Tail.Take(count - 1))))
                    .GetOrElse(Stream.Empty<A>())
                : Stream.Empty<A>();

        public Stream<A> TakeWhile(Func<A, bool> p) => TakeWhile(this, p, true);

        public Stream<A> TakeWhileNot(Func<A, bool> p) => TakeWhile(this, p, false);

        public override Stream<A> ToStream() => this;

        public override string ToString() => $"{{{ToInternalString()}}}";

        public Stream<(A, B)> Zip<B>(Stream<B> other) =>
            HeadOption.GetIfPresent(out A headA) && other.HeadOption.GetIfPresent(out B headB)
                ? Stream.Of((headA, headB), Lazy.From(() => Tail.Zip(other.Tail)))
                : Stream.Empty<(A, B)>();

        public Stream<(A, int)> ZipWithIndex() =>
            HeadOption.GetIfPresent(out A head)
                ? Stream.Of((head, 0), Lazy.From(() => Tail.ZipWithIndex()))
                : Stream.Empty<(A, int)>();

        private protected Stream() { }

        protected internal abstract string ToInternalString();

        private Stream<A> Filter(Stream<A> stream, Func<A, bool> p, bool isTruthly)
        {
            var curr = stream;
            while (curr.HeadOption.GetIfPresent(out A head) && p(head) != isTruthly)
            {
                curr = curr.Tail;
            }

            return curr
                .HeadOption
                .Map(head => Stream.Of(head, Lazy.From(() => Filter(curr, p, isTruthly))))
                .GetOrElse(Stream.Empty<A>());
        }

        private Stream<A> DropsWhile(Func<A, bool> p, bool isTruthly, out int skipped)
        {
            var curr = this;
            skipped = 0;
            while(curr.HeadOption.Exists(p) == isTruthly)
            {
                curr = curr.Tail;
                skipped++;
            }
            return curr;
        }

        private Stream<A> TakeWhile(Stream<A> stream, Func<A, bool> p, bool isTruthly) =>
            stream.HeadOption.GetIfPresent(out A head) && p(head) == isTruthly
                ? Stream.Of(head, Lazy.From(() => TakeWhile(stream.Tail, p, isTruthly)))
                : Stream.Empty<A>();
    }

    public abstract class NonEmptyStream<A> : Stream<A>
    {
        public A Head { get; }

        public override Option<A> HeadOption => Option.Some(Head);

        private protected NonEmptyStream(A head) => Head = head;

        public void Deconstruct(out A head, Stream<A> tail)
        {
            head = Head;
            tail = Tail;
        }
    }

    internal sealed class EmptyStream<A> : Stream<A>
    {
        public override Option<A> HeadOption => Option.None<A>();
        public override Stream<A> Tail => this;

        internal EmptyStream() { }

        protected internal override string ToInternalString() => "";
    }

    internal sealed class ConsStream<A> : NonEmptyStream<A>
    {
        public override Stream<A> Tail => lazyTail.Get();

        protected internal override string ToInternalString() => $"{Head}, ...";

        internal ConsStream(A head, Lazy<Stream<A>> tail) : base(head)
        {
            lazyTail = tail;
        }
        internal ConsStream(A head, Func<Stream<A>> tail) : base(head)
        {
            lazyTail = tail;
        }

        private readonly Lazy<Stream<A>> lazyTail;
    }

    internal sealed class FixedConsStream<A> : NonEmptyStream<A>
    {
        public override Stream<A> Tail { get; }

        internal FixedConsStream(A head, Stream<A> tail) : base(head)
        {
            Tail = tail;
        }

        protected internal override string ToInternalString() =>
            $"{Head}{ (Tail.NonEmpty ? $", {Tail.ToInternalString()}" : "") }";
    }
}
