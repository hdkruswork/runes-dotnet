using System;
using System.Collections.Generic;
using Runes.Collections.Immutable;

using static Runes.Collections.Mutable.ArrayChunks;
using static Runes.Collections.Immutable.Streams;
using static Runes.Lazies;
using static Runes.Units;

namespace Runes.Collections.Mutable
{
    public sealed class ArrayChunk<A> : Traversable<A>
    {
        internal static readonly ArrayChunk<A> Empty = new ArrayChunk<A>(null, 0, 0);

        private readonly A[] array;
        private readonly long startIndex;

        public A this[long idx]
        {
            get => array[startIndex + idx];
            set => array[startIndex + idx] = value;
        }

        public A this[int idx]
        {
            get => array[startIndex + idx];
            set => array[startIndex + idx] = value;
        }

        public bool IsEmpty => LongLength == 0;

        public int Length => (int)LongLength;

        public long LongLength { get; }

        public override Unit Foreach(Action<A> action) => Unit(() =>
        {
            for (long idx = startIndex; idx < LongLength; idx++)
            {
                action(array[idx]);
            }
        });

        public override Unit ForeachWhile(Func<A, bool> p, Action<A> action) => Unit(() =>
        {
            for (long idx = startIndex; idx < LongLength && p(array[idx]); idx++)
            {
                action(array[idx]);
            }
        });

        public override IEnumerator<A> GetEnumerator()
        {
            for (long idx = startIndex; idx < LongLength; idx++)
            {
                yield return array[idx];
            }
        }

        public (ArrayChunk<A>, ArrayChunk<A>) SplitInTwo(long middleIndex)
        {
            var idx = startIndex + middleIndex;
            if (startIndex < idx && idx < LongLength)
            {
                return (ArrayChunks.Chunk(array, startIndex, idx), ArrayChunks.Chunk(array, startIndex + idx, LongLength - idx));
            }
            else if (idx <= 0)
            {
                return (EmptyChunk<A>(), this);
            }
            else // At this point idx is gte LongLength
            {
                return (this, EmptyChunk<A>());
            }
        }

        public Stream<ArrayChunk<A>> SplitInChunks(int chunkSize)
        {
            Stream<ArrayChunk<A>> getStream(ArrayChunk<A> chunk, long chunkSize)
            {
                (var head, var tail) = chunk.SplitInTwo(chunkSize);

                return head.IsEmpty
                    ? Empty<ArrayChunk<A>>()
                    : Stream(head, Lazy(() => getStream(tail, chunkSize)));
            }

            return getStream(this, chunkSize);
        }

        public override Stream<A> ToStream() => Arrays.ImmutableArray(array).ToStream(startIndex, LongLength);

        internal ArrayChunk(A[] array, long startIndex, long length)
        {
            this.array = array ?? Array.Empty<A>();
            var arrayLength = this.array.LongLength;
            if (startIndex >= arrayLength)
            {
                this.array = Array.Empty<A>();
                this.startIndex = 0;
                LongLength = 0;
            }
            else
            {
                this.startIndex = 0 <= startIndex ? 0 : startIndex;
                LongLength = this.startIndex + length <= arrayLength
                    ? length
                    : arrayLength - this.startIndex;
            }
        }
    }

    public static class ArrayChunks
    {
        public static ArrayChunk<A> EmptyChunk<A>() => ArrayChunk<A>.Empty;

        public static ArrayChunk<A> WholeChunk<A>(this A[] array) =>
            Chunk(array, 0, long.MaxValue);

        public static ArrayChunk<A> Chunk<A>(this A[] array, long startIndex, long length) =>
            new ArrayChunk<A>(array, startIndex, length);

        public static (ArrayChunk<A>, ArrayChunk<A>) SplitInTwo<A>(this A[] array, long middleIndex) =>
            WholeChunk(array).SplitInTwo(middleIndex);

        public static Stream<ArrayChunk<A>> SplitInChunks<A>(this A[] array, int chunkSize) =>
            WholeChunk(array).SplitInChunks(chunkSize);
    }
}
