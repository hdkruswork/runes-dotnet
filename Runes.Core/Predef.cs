using Runes.Collections;
using System;
using System.Numerics;

namespace Runes
{
    public static class Predef
    {
        public static Option<A> As<A>(this object obj) where A: class => obj is A casted ? Some(casted) : None<A>();

        public static int GetFieldsHashCode(params object[] fields) =>
            fields.FoldLeft(-1534900553, (hash, field) => hash * -1521134295 + field.GetHashCode());

        public static System.Threading.Tasks.Task<A> Async<A>(Func<A> f) =>
            System.Threading.Tasks.Task<A>.Factory.StartNew(f);

        #region Unit

        private static readonly Unit UnitObject = new Unit();

        public static Unit Unit() => UnitObject;

        public static Unit Unit(Action action)
        {
            action();
            return Unit();
        }

        public static Func<Unit> UnitFunc(Action action) =>
            () => Unit(action);

        public static Func<A, Unit> UnitFunc<A>(Action<A> action) =>
            it => Unit(() => action(it));

        #endregion

        #region Option

        public static Option<A> Option<A>(A value) where A : class => value != null ? Some(value) : None<A>();
        public static Option<A> Option<A>(A? value) where A : struct => value.HasValue ? Some(value.Value) : None<A>();

        public static Option<A> None<A>() => Runes.Option<A>.None;

        public static Some<A> Some<A>(A value) =>
            Equals(value, null) ? throw new ArgumentNullException(nameof(value)) : new Some<A>(value);

        #endregion

        #region Knowable

        public static Known<A> Known<A>(A value) => new Known<A>(value);

        public static Unknown<A> Unknown<A>() => new Unknown<A>();

        public static Knowable<A> ToKnowable<A>(this Option<A> option) =>
            option is Some<A> some ? Known(some.Value) : (Knowable<A>)Unknown<A>();

        public static Option<A> ToOption<A>(this Knowable<A> knowable) =>
            knowable is Known<A> known ? Some(known.Value) : None<A>();

        #endregion

        #region Lazy

        public static Lazy<A> Lazy<A>(Func<A> get) => new Lazy<A>(get);

        #endregion

        #region Try

        public static Try<Unit> Try(Action action) => Try(() => Unit(action));
        public static Try<A> Try<A>(Lazy<A> lazy)
        {
            try
            {
                var res = lazy.Get();
                return Success(res);
            }
            catch (Exception ex)
            {
                return Failure<A>(ex);
            }
        }
        public static Try<A> Try<A>(Func<A> func) => Try(Lazy(func));

        public static bool IsFailure<A, E>(Func<A> func, out E ex) where E : Exception => Try(func).GetIfFailure(out ex);
        public static bool IsSuccess<A>(Func<A> func, out A result) => Try(func).GetIfSuccess(out result);

        internal static Failure<A> Failure<A>(Exception ex) => new Failure<A>(ex);
        internal static Success<A> Success<A>(A result) => new Success<A>(result);

        #endregion

        #region Collections

        public static That FoldLeft<A, That>(this A[] array, That initialValue, Func<That, A, That> f) =>
            FoldLeftWhile(array, initialValue, (agg, curr) => true, f);

        public static That FoldLeftWhile<A, That>(this A[] array, That initialValue, Func<That, A, bool> p, Func<That, A, That> f)
        {
            var result = initialValue;
            for (long i = 0; i < array.LongLength && p(result, array[i]); i++)
            {
                result = f(result, array[i]);
            }

            return result;
        }

        public static That FoldRight<A, That>(this A[] array, That initialValue, Func<That, A, That> f) =>
            FoldRightWhile(array, initialValue, (agg, curr) => true, f);

        public static That FoldRightWhile<A, That>(this A[] array, That initialValue, Func<That, A, bool> p, Func<That, A, That> f)
        {
            var result = initialValue;
            for (long i = array.LongLength - 1; i >= 0 && p(result, array[i]); i--)
            {
                result = f(result, array[i]);
            }

            return result;
        }

        public static Unit Foreach<A>(this A[] array, Func<A, Unit> f)
        {
            void proc(A it) => f(it);

            return Foreach(array, proc);
        }

        public static Unit Foreach<A>(this A[] array, Action<A> f) => Unit(() =>
        {
            foreach (var item in array)
            {
                f(item);
            }
        });

        public static Unit ForeachWhile<A>(this A[] array, Func<A, bool> p, Func<A, Unit> f)
        {
            void proc(A it) => f(it);

            return ForeachWhile(array, p, proc);
        }

        public static Unit ForeachWhile<A>(this A[] array, Func<A, bool> p, Action<A> f) => Unit(() =>
        {
            for (long i = 0; i < array.LongLength && p(array[i]); i++)
            {
                f(array[i]);
            }
        });

        public static Array<char> ToArray(this string text) => Array(text.ToCharArray());

        public static Array<A> ToArray<A>(this A[] array) => Array(array);

        public static MutableArray<A> ToMArray<A>(this A[] array) => MArray(array);

        public static Array<A> ToArray<A>(this ICollection<A> collection)
        {
            if (collection is Array<A> array)
            {
                return array;
            }

            var traversable = ToTraversable(collection);
            return Collections.Array<A>.CreateArrayFrom(traversable);
        }

        public static MutableArray<A> ToMArray<A>(this ICollection<A> collection)
        {
            if (collection is MutableArray<A> array)
            {
                return array;
            }

            var traversable = ToTraversable(collection);
            return Collections.MutableArray<A>.CreateArrayFrom(traversable);
        }

        public static List<A> ToList<A>(this ICollection<A> collection)
        {
            if (collection is List<A> list)
            {
                return list;
            }

            var res = EmptyList<A>();
            var traversable = ToTraversable(collection);
            traversable.Reversed().Foreach(it => res = res.Prepend(it));

            return res;
        }

        public static Stream<A> ToStream<A>(this A[] array)
        {
            Stream<A> GetStream(A[] arr, long start) =>
                start < arr.LongLength
                    ? Stream(arr[start], () => GetStream(arr, start + 1))
                    : EmptyStream<A>();

            return GetStream(array, 0);
        }

        public static Stream<A> ToStream<A>(this ICollection<A> collection)
        {
            if (collection is Stream<A> stream)
            {
                return stream;
            }

            return collection.HeadOption.GetIfPresent(out A head)
                ? Stream(head, () => collection.Tail.ToStream())
                : EmptyStream<A>();
        }

        public static ITraversable<A> ToTraversable<A>(this ICollection<A> collection)
        {
            if (collection is ITraversable<A> traversable)
            {
                return traversable;
            }

            throw new NotSupportedException("Non traversable collections cannot be handled as traversabe");
        }

        #endregion

        #region Lists

        public static List<A> EmptyList<A>() => Lists.Empty<A>();

        public static List<A> List<A>(params A[] items)
        {
            var res = EmptyList<A>();
            if (items != null)
            {
                for (var idx = items.LongLength - 1; idx >= 0; idx--)
                {
                    res = List(items[idx], res);
                } 
            }

            return res;
        }
        public static List<A> List<A>(A head, List<A> tail) => Lists.Create(head, tail);

        public static List<A> ToList<A>(this ITraversable<A> traversable)
        {
            var builder = Collections.List<A>.CreateListBuilder();
            traversable.FoldLeft(Unit(), (_, it) => Unit(() => builder.Add(it)));

            return builder.Build();
        }

        #endregion

        #region Arrays

        public static Array<A> EmptyArray<A>() => Collections.Array<A>.Empty;
        public static MutableArray<A> EmptyMArray<A>() => MutableArray<A>.Empty;

        public static Array<A> Array<A>(params A[] array) =>
            array != null && array.Length > 0
                ? Array(array, 0, array.LongLength, 1)
                : EmptyArray<A>();

        internal static Array<A> Array<A>(A[] array, long startIndex, long length, int step) =>
            new Array<A>(array, startIndex, length, step);

        public static MutableArray<A> MArray<A>(params A[] array) =>
            array != null && array.Length > 0
                ? MArray(array, 0, array.LongLength, 1)
                : EmptyMArray<A>();

        internal static MutableArray<A> MArray<A>(A[] array, long startIndex, long length, int step) =>
            new MutableArray<A>(array, startIndex, length, step);

        #endregion

        #region Streams

        public static Stream<A> EmptyStream<A>() => Collections.Stream<A>.Empty;

        public static Stream<A> Flatten<A, CC>(this Stream<CC> stream) where CC : ITraversable<A, CC> =>
            stream.HeadOption.GetIfPresent(out CC head)
                ? head.ToStream().Append(() => stream.Tail.Flatten<A, CC>())
                : EmptyStream<A>();

        public static Stream<A> Stream<A>(A head) => Stream(head, EmptyStream<A>());
        public static Stream<A> Stream<A>(A head, Stream<A> tail) => new Stream<A>.NonEmptyStream(head, tail);
        public static Stream<A> Stream<A>(A head, Func<Stream<A>> tailFunc) => new Stream<A>.NonEmptyStream(head, tailFunc);

        public static Stream<BigInteger> StartStream(BigInteger start, int step = 1) =>
            Stream(start, () => StartStream(start + step, step));

        #endregion
    }
}
