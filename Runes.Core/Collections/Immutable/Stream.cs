using System;
using System.Collections.Generic;
using System.Numerics;

using static Runes.Collections.Immutable.Arrays;
using static Runes.Collections.Immutable.Streams;
using static Runes.Predef;

namespace Runes.Collections.Immutable
{
    public static class Streams
    {
        public static Stream<A> Empty<A>() => EmptyStream<A>.Object;

        public static Stream<int> IncreasingStream(int head) => StartStream(head, 1);
        public static Stream<int> DecreasingStream(int head) => StartStream(head, -1);

        public static Stream<BigInteger> IncreasingStream(BigInteger head) => StartStream(head, 1);
        public static Stream<BigInteger> DecreasingStream(BigInteger head) => StartStream(head, -1);

        public static Stream<int> StartStream(int head, int step) =>
            Stream(head, Lazy(() => IncreasingStream(head + step)));

        public static Stream<BigInteger> StartStream(BigInteger head, int step) =>
            new InfinteStream<BigInteger>(head, Lazy(() => IncreasingStream(head + step)));

        public static Stream<A> Stream<A>(A head, Lazy<Stream<A>> tail) =>
            new ConsStream<A>(head, tail);

        public static Stream<A> Stream<A>(A head, Stream<A> tail) =>
            new FixedConsStream<A>(head, tail);

        public static Stream<A> Stream<A>(A head) =>
            new FixedConsStream<A>(head, Empty<A>());

        public static Stream<A> Stream<A>(IEnumerable<A> e)
        {
            Stream<A> fromIterator(IEnumerator<A> iter) =>
                iter.MoveNext()
                    ? Stream(iter.Current, Lazy(() => fromIterator(iter)))
                    : Empty<A>();

            return fromIterator(e.GetEnumerator());
        }

        public static Stream<A> Stream<A>(params A[] array) => ImmutableArray(array).ToStream();

        public static Stream<A> Stream<A>(A a1, A a2, Lazy<Stream<A>> tail) => Stream(a1).Append(Stream(a2, tail));

        public static Stream<A> Flatten<A, That>(this Stream<That> stream) where That : Traversable<A> =>
            stream
                .HeadOption
                .Map(trav => trav.ToStream().Append(stream.Tail.Flatten<A, That>()))
                .GetOrElse(Empty<A>());

        public static Stream<char> ToStream(this string str) => Stream(str.ToCharArray());

        public static Stream<A> Unzip<A>(this Stream<(A, A)> zipped)
        {
            if (zipped.GetHeadIfPresent(out (A, A) head))
            {
                (var a, var b) = head;

                return Stream(a, b).Append(zipped.Tail.Unzip());
            }

            return Empty<A>();
        }

        private sealed class InfinteStream<A> : ConsStream<A>
        {
            public override Knowable<BigInteger> Count => Unknown<BigInteger>();

            public override Knowable<bool> IsFinite => Known(false);

            internal InfinteStream(A head, Lazy<Stream<A>> tail) : base(head, tail) { }

            internal InfinteStream(A head, Func<Stream<A>> tail) : base(head, tail) { }

            protected internal override Stream<B> BuildStreamLikeThis<B>(B head, Func<Stream<B>> getTailFunc, bool mayRestrict) =>
                mayRestrict
                    ? (Stream<B>) new IndeterminedStream<B>(head, Lazy(() => getTailFunc()))
                    : new InfinteStream<B>(head, Lazy(() => getTailFunc()));
        }

        private sealed class IndeterminedStream<A> : ConsStream<A>
        {
            public override Knowable<BigInteger> Count => Unknown<BigInteger>();

            public override Knowable<bool> IsFinite => Unknown<bool>();

            internal IndeterminedStream(A head, Lazy<Stream<A>> tail) : base(head, tail) { }

            internal IndeterminedStream(A head, Func<Stream<A>> tail) : base(head, tail) { }

            protected internal override Stream<B> BuildStreamLikeThis<B>(B head, Func<Stream<B>> getTailFunc, bool mayRestrict) =>
                new IndeterminedStream<B>(head, Lazy(() => getTailFunc()));
        }

        private class ConsStream<A> : NonEmptyStream<A>
        {
            public override Stream<A> Tail => lazyTail.Get();

            protected internal override string ToInternalString() => $"{Head}, ...";

            internal ConsStream(A head, Lazy<Stream<A>> tail) : base(head) => lazyTail = tail;

            internal ConsStream(A head, Func<Stream<A>> tail) : base(head) => lazyTail = tail;

            private readonly Lazy<Stream<A>> lazyTail;
        }

        private sealed class FixedConsStream<A> : NonEmptyStream<A>
        {
            public override Stream<A> Tail { get; }

            internal FixedConsStream(A head, Stream<A> tail) : base(head) => Tail = tail;

            protected internal override string ToInternalString() =>
                $"{Head}{ (Tail.NonEmpty ? $", {Tail.ToInternalString()}" : "") }";
        }

        private abstract class NonEmptyStream<A> : Stream<A>
        {
            public A Head { get; }

            public override Option<A> HeadOption => Some(Head);

            private protected NonEmptyStream(A head)
            {
                Head = head;
            }

            public void Deconstruct(out A head, out Stream<A> tail)
            {
                head = Head;
                tail = Tail;
            }
        }

        private sealed class EmptyStream<A> : Stream<A>
        {
            public static readonly EmptyStream<A> Object = new EmptyStream<A>();

            public override Knowable<BigInteger> Count => Known(BigInteger.Zero);

            public override Option<A> HeadOption => None<A>();
            public override Stream<A> Tail => this;

            public override Stream<A[]> Sliding(int size, int step) => Empty<A[]>();

            private EmptyStream() { }

            protected internal override string ToInternalString() => "";
        }
    }

    public abstract class Stream<A> : Traversable<A>
    {
        public virtual Knowable<BigInteger> Count => Known(FoldLeft(BigInteger.Zero, (sum, _) => sum + 1));

        public virtual Knowable<bool> IsFinite => Known(true);

        public Knowable<bool> IsInfinite => IsFinite.Map(v => !v);

        public bool IsEmpty => HeadOption.IsEmpty;

        public bool NonEmpty => HeadOption.NonEmpty;

        public abstract Option<A> HeadOption { get; }

        public abstract Stream<A> Tail { get; }

        public Stream<A> Append(A rear)
        {
            return HeadOption
                .Map(head => BuildStreamLikeThis(head, () => Tail.Append(rear), false))
                .GetOrElse(Stream(rear));
        }

        public Stream<A> Append(IEnumerable<A> e) => Append(e is Stream<A> stream ? stream : Stream(e));

        public Stream<A> Append(Stream<A> other)
        {
            var build = GetBuildFunc<A, A, A>(false, this, other);

            return HeadOption
                .Map(head => build(head, () => Tail.Append(other)))
                .GetOrElse(other);
        }

        public Stream<B> Collect<B>(IPartialFunction<A, B> pf) =>
            FlatMap<B, Option<B>>(it => Try(() => pf.Apply(it)).ToOption());

        public Stream<B> Collect<B>(Func<A, B> f) =>
            FlatMap<B, Option<B>>(it => Try(() => f(it)).ToOption());

        public Option<B> CollectFirst<B>(IPartialFunction<A, B> pf) => Collect(pf).HeadOption;

        public Option<B> CollectFirst<B>(Func<A, B> f) => Collect(f).HeadOption;

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
                    .Map(head => BuildStreamLikeThis(head, () => Tail.Drops(count - 1), false))
                    .GetOrElse(Empty<A>())
                : Empty<A>();

        public Stream<A> DropsWhile(Func<A, bool> p) => DropsWhile(p, out _);

        public Stream<A> DropsWhileNot(Func<A, bool> p) => DropsWhileNot(p, out _);

        public Stream<A> DropsWhile(Func<A, bool> p, out int skipped) => DropsWhile(this, p, true, out skipped);

        public Stream<A> DropsWhileNot(Func<A, bool> p, out int skipped) => DropsWhile(this, p, false, out skipped);

        public bool Exists(Func<A, bool> p) => Filter(p).NonEmpty;

        public Stream<A> Filter(Func<A, bool> p) => Filter(this, p, true);

        public Stream<A> FilterNot(Func<A, bool> p) => Filter(this, p, false);

        public Stream<B> FlatMap<B, That>(Func<A, That> f) where That : Traversable<B> =>
            HeadOption
                .Map(head => f(head).ToStream().Append(Tail.FlatMap<B, That>(f)))
                .GetOrElse(Empty<B>());

        public bool ForAll(Func<A, bool> p)
        {
            var curr = this;
            while (curr.GetHeadIfPresent(out A head) && p(head))
            {
                curr = curr.Tail;
            }
            return curr.IsEmpty;
        }

        public override Unit Foreach(Action<A> action) => Unit(() =>
        {
            var curr = this;
            while (curr.GetHeadIfPresent(out A head))
            {
                action(head);
                curr = curr.Tail;
            }
        });

        public override Unit ForeachWhile(Func<A, bool> p, Action<A> action) => Unit(() =>
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
            while (curr.GetHeadIfPresent(out A item))
            {
                yield return item;
                curr = curr.Tail;
            }
        }

        public bool GetHeadIfPresent(out A head) => HeadOption.GetIfPresent(out head);

        public bool GetCountIfFinite(out BigInteger count)
        {
            if (IsFinite.Contains(true) && Count is Known<BigInteger> knownCount)
            {
                count = knownCount.Value;
                return true;
            }

            count = BigInteger.Zero;
            return false;
        }

        public Stream<B> Map<B>(Func<A, B> f) =>
            HeadOption
                .Map(head => BuildStreamLikeThis(f(head), () => Tail.Map(f), false))
                .GetOrElse(Empty<B>());

        public Stream<A> Prepend(A e) => BuildStreamLikeThis(e, () => this, false);

        public Stream<A> Prepend(IEnumerable<A> e) =>
            (e is Stream<A> stream ? stream : Stream(e))
                .Append(this);

        public Stream<A> Prepend(Stream<A> other) => other.Append(this);

        public Stream<A> Reverse() => FoldLeft(Empty<A>(), (acc, it) => acc.Prepend(it));

        public virtual Stream<A[]> Sliding(int size, int step = 1)
        {
            var head = Take(size).ToList().ToArray();
            var tail = this;
            for (int i = 0; i < step; i++)
            {
                tail = tail.Tail;
            }

            return BuildStreamLikeThis(head, () => tail.Sliding(size, step), false);
        }

        public Stream<A> Take(int count) =>
            count > 0
                ? HeadOption
                    .Map(head => Stream(head, Lazy(() => Tail.Take(count - 1))))
                    .GetOrElse(Empty<A>())
                : Empty<A>();

        public Stream<A> TakeWhile(Func<A, bool> p) => TakeWhile(this, p, true);

        public Stream<A> TakeWhileNot(Func<A, bool> p) => TakeWhile(this, p, false);

        public (Stream<A>, Stream<A>) TakeAndDrop(int count) => (Take(count), Drops(count));

        public override Stream<A> ToStream() => this;

        public override string ToString() => $"{{{ToInternalString()}}}";

        public virtual Stream<(A, B)> Zip<B>(Stream<B> other)
        {
            var build = GetBuildFunc<(A, B), A, B>(true, this, other);

            return HeadOption.GetIfPresent(out A headA) && other.HeadOption.GetIfPresent(out B headB)
                ? build((headA, headB), () => Tail.Zip(other.Tail))
                : Empty<(A, B)>();
        }

        public Stream<(A, int)> ZipWithIndex() => Zip(IncreasingStream(0));

        // hidden members

        private static Stream<A> DropsWhile(Stream<A> stream, Func<A, bool> p, bool isTruthly, out int skipped)
        {
            var curr = stream;
            skipped = 0;
            while (curr.HeadOption.Exists(p) == isTruthly)
            {
                curr = curr.Tail;
                skipped++;
            }
            return curr;
        }

        private static Stream<A> Filter(Stream<A> stream, Func<A, bool> p, bool isTruthly)
        {
            var curr = stream;
            while (curr.HeadOption.GetIfPresent(out A head) && p(head) != isTruthly)
            {
                curr = curr.Tail;
            }

            return curr
                .HeadOption
                .Map(head => stream.BuildStreamLikeThis(head, () => Filter(curr, p, isTruthly), true))
                .GetOrElse(Empty<A>());
        }

        private static Stream<A> TakeWhile(Stream<A> stream, Func<A, bool> p, bool isTruthly) =>
            stream.HeadOption.GetIfPresent(out A head) && p(head) == isTruthly
                ? stream.BuildStreamLikeThis(head, () => TakeWhile(stream.Tail, p, isTruthly), true)
                : Empty<A>();

        private static Func<B, Func<Stream<B>>, Stream<B>> GetBuildFunc<B, C, D>(bool finite, Stream<C> first, Stream<D> second)
        {
            Stream<B> firstBuild(B h, Func<Stream<B>> t) => first.BuildStreamLikeThis(h, t, false);
            Stream<B> secondBuild(B h, Func<Stream<B>> t) => second.BuildStreamLikeThis(h, t, false);

            return first.IsFinite.Contains(finite)
                ? (Func<B, Func<Stream<B>>, Stream<B>>)firstBuild
                : secondBuild;
        }

        private protected Stream() { }

        protected internal virtual Stream<B> BuildStreamLikeThis<B>(B head, Func<Stream<B>> getTailFunc, bool mayRestrict) =>
            Stream(head, Lazy(() => getTailFunc()));

        protected internal abstract string ToInternalString();
    }
}
