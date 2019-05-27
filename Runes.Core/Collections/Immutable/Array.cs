using System;
using System.Collections.Generic;
using static Runes.Collections.Immutable.Streams;
using static Runes.Lazies;
using static Runes.Options;
using static Runes.Units;

namespace Runes.Collections.Immutable
{
    public static class Arrays
    {
        public static Array<T> ImmutableArray<T>(T[] array) => ToImmutableArray(array);

        public static Array<T> ImmutableArray<T>(T item, long length)
        {
            var arr = new T[length];
            System.Array.Fill(arr, item);
            return ImmutableArray(arr);
        }

        public static Array<T> ToImmutableArray<T>(this T[] array)
        {
            T[] result = new T[array.LongLength];
            array.CopyTo(result.AsMemory());
            return new Array<T>(result);
        }

        public static List<T> ToList<T>(this T[] array) => new Array<T>(array).ToList();
    }

    public sealed class Array<T> : Traversable<T>
    {
        public static explicit operator Array<T>(T[] array) => new Array<T>(array);

        public Option<T> this[int index] => this[(long)index];

        public Option<T> this[long index] =>
            index >= 0L && index < array.LongLength
                ? Some(array[index])
                : None<T>();

        public Option<T> First => this[0L];

        public Option<T> Last => this[LongLength - 1];

        public int Length => array.Length;

        public long LongLength => array.LongLength;

        public bool GetFirst(out T first) => First.GetIfPresent(out first);

        public bool GetItemAt(int idx, out T item) => GetItemAt((long)idx, out item);

        public bool GetItemAt(long idx, out T item) => this[idx].GetIfPresent(out item);

        public bool GetLastIf(out T last) => Last.GetIfPresent(out last);

        public That FoldRight<That>(That initialValue, Func<That, T, That> f)
        {
            var result = initialValue;
            for (var i = LongLength - 1; i >= 0; i--)
            {
                result = f(result, array[i]);
            }
            return result;
        }

        public That FoldRightWhile<That>(That initialValue, Func<That, T, bool> p, Func<That, T, That> f)
        {
            var result = initialValue;
            for (var i = LongLength - 1; i >= 0 && p(result, array[i]); i--)
            {
                result = f(result, array[i]);
            }
            return result;
        }

        public override Unit Foreach(Action<T> action) => Unit(() =>
        {
            foreach (var item in array)
            {
                action(item);
            }
        });

        public override Unit ForeachWhile(Func<T, bool> p, Action<T> action) => Unit(() =>
        {
            for (var i = 0L; i < array.LongLength && p(array[i]); i++)
            {
                action(array[i]);
            }
        });

        public override IEnumerator<T> GetEnumerator()
        {
            foreach (var item in array)
            {
                yield return item;
            }
        }

        public override T[] ToArray() => array.ToImmutableArray().array;

        public override Stream<T> ToStream()
        {
            Stream<T> fromArrayIndex(T[] array, long idx) =>
                idx >= 0 && idx < array.LongLength
                    ? Stream(array[idx], Lazy(() => fromArrayIndex(array, idx + 1)))
                    : Empty<T>();

            return fromArrayIndex(array, 0L);
        }

        // Private members

        private readonly T[] array;

        internal Array(T[] array)
        {
            this.array = array ?? new T[0];
        }
    }
}
