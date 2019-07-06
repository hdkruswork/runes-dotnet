using Runes.Collections;
using System;
using System.Numerics;
using Runes.Math;
using Runes.Math.Algorithms;
using Runes.Collections.Mutable;
using Runes.Time;

namespace Runes
{
    public static class Predef
    {
        public static bool As<A>(this object obj, out A result)
        {
            if (obj is A cast)
            {
                result = cast;
                return true;
            }

            result = default;
            return false;
        }

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

        #region Functions

        public static Action<B> Curry<A, B>(this Action<A, B> f, A a) => b => f(a, b);
        public static Action<B, C> Curry<A, B, C>(this Action<A, B, C> f, A a) => (b, c) => f(a, b, c);

        public static Func<B, R> Curry<A, B, R>(this Func<A, B, R> f, A a) => b => f(a, b);
        public static Func<B, C, R> Curry<A, B, C, R>(this Func<A, B, C, R> f, A a) => (b, c) => f(a, b, c);

        #endregion

        #region Option

        public static Option<A> Flatten<A>(this Option<Option<A>> option) => option.GetOrElse(None<A>());

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

        public static Lazy<A> Lazy<A>(A value) => Runes.Lazy<A>.Create(value);
        public static Lazy<A> Lazy<A>(Func<A> f) => Runes.Lazy<A>.Create(f);

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

        public static Func<Try<Unit>> TryFunc<A>(Action action) => () => Try(action);
        public static Func<Try<A>> TryFunc<A>(Func<A> f) => () => Try(f);

        public static bool IsFailure<A, E>(Func<A> func, out E ex) where E : Exception => Try(func).GetIfFailure(out ex);
        public static bool IsSuccess<A>(Func<A> func, out A result) => Try(func).GetIfSuccess(out result);

        internal static Failure<A> Failure<A>(Exception ex) => new Failure<A>(ex);
        internal static Success<A> Success<A>(A result) => new Success<A>(result);

        #endregion

        #region Math

        public static Ordering<A> OrderingBy<A>(Func<A, A, int> comparingFunc) => Ordering<A>.By(comparingFunc);

        public static Ordering<A> OrderingBy<A>() where A : IComparable<A> => Ordering<A>.By((x, y) => x.CompareTo(y));

        public static QuickSortAlgorithm<A> Quicksort<A>() => QuickSortAlgorithm<A>.Object;

        public static Int GCD(Int a, Int b) => Int.GreatestCommonDivisor(a, b);
        public static Int LCM(Int a, Int b) => a / GCD(a, b) * b;

        #region Comparators functions

        public static int Compare(sbyte a, sbyte b) => a.CompareTo(b);
        public static int Compare(byte a, byte b) => a.CompareTo(b);
        public static int Compare(short a, short b) => a.CompareTo(b);
        public static int Compare(ushort a, ushort b) => a.CompareTo(b);
        public static int Compare(int a, int b) => a.CompareTo(b);
        public static int Compare(uint a, uint b) => a.CompareTo(b);
        public static int Compare(long a, long b) => a.CompareTo(b);
        public static int Compare(ulong a, ulong b) => a.CompareTo(b);
        public static int Compare(float a, float b) => a.CompareTo(b);
        public static int Compare(double a, double b) => a.CompareTo(b);
        public static int Compare(decimal a, decimal b) => a.CompareTo(b);
        public static int Compare(BigInteger a, BigInteger b) => a.CompareTo(b);
        public static int Compare(Int a, Int b) => a.CompareTo(b);
        public static int Compare(Rational a, Rational b) => a.CompareTo(b);

        #endregion

        #region Max/Min functions

        public static sbyte Max(params sbyte[] values) => Max(OrderingBy<sbyte>(), sbyte.MaxValue, values);
        public static sbyte Min(params sbyte[] values) => Min(OrderingBy<sbyte>(), sbyte.MinValue, values);
        public static sbyte Max(this IIterable<sbyte> values) => Max(values, OrderingBy<sbyte>(), sbyte.MaxValue);
        public static sbyte Min(this IIterable<sbyte> values) => Min(values, OrderingBy<sbyte>(), sbyte.MinValue);
        public static byte Max(params byte[] values) => Max(OrderingBy<byte>(), byte.MaxValue, values);
        public static byte Min(params byte[] values) => Min(OrderingBy<byte>(), byte.MinValue, values);
        public static byte Max(this IIterable<byte> values) => Max(values, OrderingBy<byte>(), byte.MaxValue);
        public static byte Min(this IIterable<byte> values) => Min(values, OrderingBy<byte>(), byte.MinValue);
        public static short Max(params short[] values) => Max(OrderingBy<short>(), short.MaxValue, values);
        public static short Min(params short[] values) => Min(OrderingBy<short>(), short.MinValue, values);
        public static short Max(this IIterable<short> values) => Max(values, OrderingBy<short>(), short.MaxValue);
        public static short Min(this IIterable<short> values) => Min(values, OrderingBy<short>(), short.MinValue);
        public static ushort Max(params ushort[] values) => Max(OrderingBy<ushort>(), ushort.MaxValue, values);
        public static ushort Min(params ushort[] values) => Min(OrderingBy<ushort>(), ushort.MinValue, values);
        public static ushort Max(this IIterable<ushort> values) => Max(values, OrderingBy<ushort>(), ushort.MaxValue);
        public static ushort Min(this IIterable<ushort> values) => Min(values, OrderingBy<ushort>(), ushort.MinValue);
        public static int Max(params int[] values) => Max(OrderingBy<int>(), int.MaxValue, values);
        public static int Min(params int[] values) => Min(OrderingBy<int>(), int.MinValue, values);
        public static int Max(this IIterable<int> values) => Max(values, OrderingBy<int>(), int.MaxValue);
        public static int Min(this IIterable<int> values) => Min(values, OrderingBy<int>(), int.MinValue);
        public static uint Max(params uint[] values) => Max(OrderingBy<uint>(), uint.MaxValue, values);
        public static uint Min(params uint[] values) => Min(OrderingBy<uint>(), uint.MinValue, values);
        public static uint Max(this IIterable<uint> values) => Max(values, OrderingBy<uint>(), uint.MaxValue);
        public static uint Min(this IIterable<uint> values) => Min(values, OrderingBy<uint>(), uint.MinValue);
        public static long Max(params long[] values) => Max(OrderingBy<long>(), long.MaxValue, values);
        public static long Min(params long[] values) => Min(OrderingBy<long>(), long.MinValue, values);
        public static long Max(this IIterable<long> values) => Max(values, OrderingBy<long>(), long.MaxValue);
        public static long Min(this IIterable<long> values) => Min(values, OrderingBy<long>(), long.MinValue);
        public static ulong Max(params ulong[] values) => Max(OrderingBy<ulong>(), ulong.MaxValue, values);
        public static ulong Min(params ulong[] values) => Min(OrderingBy<ulong>(), ulong.MinValue, values);
        public static ulong Max(this IIterable<ulong> values) => Max(values, OrderingBy<ulong>(), ulong.MaxValue);
        public static ulong Min(this IIterable<ulong> values) => Min(values, OrderingBy<ulong>(), ulong.MinValue);
        public static float Max(params float[] values) => Max(OrderingBy<float>(), float.MaxValue, values);
        public static float Min(params float[] values) => Min(OrderingBy<float>(), float.MinValue, values);
        public static float Max(this IIterable<float> values) => Max(values, OrderingBy<float>(), float.MaxValue);
        public static float Min(this IIterable<float> values) => Min(values, OrderingBy<float>(), float.MinValue);
        public static double Max(params double[] values) => Max(OrderingBy<double>(), double.MaxValue, values);
        public static double Min(params double[] values) => Min(OrderingBy<double>(), double.MinValue, values);
        public static double Max(this IIterable<double> values) => Max(values, OrderingBy<double>(), double.MaxValue);
        public static double Min(this IIterable<double> values) => Min(values, OrderingBy<double>(), double.MinValue);
        public static decimal Max(params decimal[] values) => Max(OrderingBy<decimal>(), decimal.MaxValue, values);
        public static decimal Min(params decimal[] values) => Min(OrderingBy<decimal>(), decimal.MinValue, values);
        public static decimal Max(this IIterable<decimal> values) => Max(values, OrderingBy<decimal>(), decimal.MaxValue);
        public static decimal Min(this IIterable<decimal> values) => Min(values, OrderingBy<decimal>(), decimal.MinValue);

        public static Int Max(Int first, params Int[] values) => Max(OrderingBy<Int>(), first, values);
        public static Int Min(Int first, params Int[] values) => Min(OrderingBy<Int>(), first, values);
        public static Rational Max(Rational first, params Rational[] values) => Max(OrderingBy<Rational>(), first, values);
        public static Rational Min(Rational first, params Rational[] values) => Min(OrderingBy<Rational>(), first, values);

        public static A Max<A>(Ordering<A> ord, A first, params A[] array) =>
            array.FoldLeft(first, (max, curr) => ord.Compare(max, curr) > 0 ? max : curr);
        public static A Min<A>(Ordering<A> ord, A first, params A[] array) =>
            array.FoldLeft(first, (min, curr) => ord.Compare(min, curr) < 0 ? min : curr);
        public static A Max<A>(this IIterable<A> iterable, Ordering<A> ord, A initialMaxValue) =>
            iterable.FoldLeft(initialMaxValue, (max, curr) => ord.Compare(max, curr) > 0 ? max : curr);
        public static A Min<A>(this IIterable<A> iterable, Ordering<A> ord, A initialMinValue) =>
            iterable.FoldLeft(initialMinValue, (min, curr) => ord.Compare(min, curr) < 0 ? min : curr);

        public static Option<A> Max<A>(this A[] array, Ordering<A> ord) => Max(Array(array), ord);
        public static Option<A> Min<A>(this A[] array, Ordering<A> ord) => Min(Array(array), ord);
        public static Option<A> Max<A>(this IIterable<A> iterable, Ordering<A> ord) =>
            iterable
                .HeadOption
                .Map(first => Max(iterable.Tail, ord, first));
        public static Option<A> Min<A>(this IIterable<A> iterable, Ordering<A> ord) =>
            iterable
                .HeadOption
                .Map(first => Min(iterable.Tail, ord, first));

        public static (Int, Int) MinMax(Int first, params Int[] values) =>
            MinMax(Array(values), OrderingBy<Int>(), first);
        public static (Rational, Rational) MinMax(Rational first, params Rational[] values) =>
            MinMax(Array(values), OrderingBy<Rational>(), first);

        public static (A, A) MinMax<A>(Ordering<A> ord, A first, params A[] values) => MinMax(Array(values), ord, first);
        public static (A, A) MinMax<A>(this A[] values, Ordering<A> ord, A defaultValue) => MinMax(Array(values), ord, defaultValue);
        public static (A, A) MinMax<A>(IIterable<A> iterable, Ordering<A> ord, A defaultValue) =>
            iterable
                .FoldLeft((defaultValue, defaultValue), (minMax, it) =>
                {
                    var (min, max) = minMax;
                    if (ord.Compare(it, min) < 0)
                    {
                        return (it, max);
                    }
                    if (ord.Compare(it, max) > 0)
                    {
                        return (min, it);
                    }

                    return (min, max);
                });

        public static Option<(A, A)> MinMax<A>(this A[] values, Ordering<A> ord) => MinMax(Array(values), ord);
        public static Option<(A, A)> MinMax<A>(IIterable<A> iterable, Ordering<A> ord) =>
            iterable
                .HeadOption
                .Map(first => MinMax(iterable.Tail, ord, first));

        #endregion

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

        public static That To<A, That>(this A[] array, IFactory<A, That> factory) where That : IIterable<A> =>
            factory.From(array);

        public static Array<A> ToArray<A>(this A[] array) => Array(array);

        public static Set<A> ToSet<A>(this A[] array) => array.ToArray().ToSet();

        public static Stream<A> ToStream<A>(this A[] array)
        {
            Stream<A> GetStream(A[] arr, long start) =>
                start < arr.LongLength
                    ? Stream(arr[start], () => GetStream(arr, start + 1))
                    : EmptyStream<A>();

            return GetStream(array ?? new A[0], 0);
        }

        #endregion

        #region Lists

        public static List<A> EmptyList<A>() => Collections.List<A>.Empty;

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
        public static List<A> List<A>(A head, List<A> tail) => Collections.List<A>.Create(head, tail);

        #endregion

        #region Arrays

        public static B[] CastItems<A, B>(this A[] array)
        {
            var res = new B[array.LongLength];

            System.Array.Copy(array, res, array.LongLength);

            return res;
        }

        public static Array<A> EmptyArray<A>() => Collections.Array<A>.Empty;

        public static Array<A> Array<A>(params A[] array) =>
            array != null && array.Length > 0
                ? Array(array, 0, array.LongLength, 1)
                : EmptyArray<A>();

        public static ReadOnlyArray<A> ReadOnlyArray<A>(params A[] array) => new ReadOnlyArray<A>(array ?? new A[0]);

        internal static Array<A> Array<A>(A[] array, long startIndex, long length, int step) =>
            Collections.Array<A>.Factory.From(array, startIndex, length, step);

        #endregion

        #region Streams

        public static Stream<A> EmptyStream<A>() => Collections.Stream<A>.Empty;

        public static Stream<A> Flatten<A>(this Stream<IIterable<A>> stream) =>
            stream.HeadOption.GetIfPresent(out var head)
                ? head.ToStream().Append(() => stream.Tail.Flatten())
                : EmptyStream<A>();

        public static Stream<A> Flatten<A>(this Stream<Option<A>> stream) =>
            stream.HeadOption.GetIfPresent(out var head)
                ? head.ToStream().Append(() => stream.Tail.Flatten())
                : EmptyStream<A>();

        public static Stream<A> Stream<A>(params A[] values) => values.ToStream();
        public static Stream<A> Stream<A>(A head, Stream<A> tail) => Collections.Stream<A>.Create(head, () => tail);
        public static Stream<A> Stream<A>(A head, Func<Stream<A>> tailFunc) => Collections.Stream<A>.Create(head, tailFunc);

        public static Stream<BigInteger> StartStream(BigInteger start, int step = 1) =>
            Stream(start, () => StartStream(start + step, step));

        public static Stream<char> ToStream(this System.IO.StringReader reader)
        {
            char first = (char)reader.Read();

            return first != 0
                ? Stream(first, () => ToStream(reader))
                : EmptyStream<char>();
        }

        #endregion

        #region Time

        public static Instant Now() => Time.Instant.From(DateTimeOffset.UtcNow);

        public static Instant Instant(int year, int month, int day, int hour, int minute, int second, long ticks) =>
            Time.Instant.From(year, month, day, hour, minute, second, ticks);

        public static Instant Instant(int year, int month, int day, int hour, int minute, int second) =>
            Time.Instant.From(year, month, day, hour, minute, second);

        public static Instant Instant(int year, int month, int day) => Time.Instant.From(year, month, day);

        public static Instant Instant(Int ticks) => Time.Instant.From(ticks);

        public static Instant Instant(DateTimeOffset dateTimeOffset) => Time.Instant.From(dateTimeOffset);

        public static TimeInterval Ticks(this int value) => TimeInterval.FromTicks(value);
        public static TimeInterval Ticks(this long value) => TimeInterval.FromTicks(value);
        public static TimeInterval Ticks(this Int value) => TimeInterval.FromTicks(value);

        public static TimeInterval Seconds(this int value) => TimeInterval.FromSeconds(value);
        public static TimeInterval Seconds(this long value) => TimeInterval.FromSeconds(value);
        public static TimeInterval Seconds(this Int value) => TimeInterval.FromSeconds(value);

        public static TimeInterval Minutes(this int value) => TimeInterval.FromMinutes(value);
        public static TimeInterval Minutes(this long value) => TimeInterval.FromMinutes(value);
        public static TimeInterval Minutes(this Int value) => TimeInterval.FromMinutes(value);

        public static TimeInterval Hours(this int value) => TimeInterval.FromHours(value);
        public static TimeInterval Hours(this long value) => TimeInterval.FromHours(value);
        public static TimeInterval Hours(this Int value) => TimeInterval.FromHours(value);

        public static TimeInterval Days(this int value) => TimeInterval.FromDays(value);
        public static TimeInterval Days(this long value) => TimeInterval.FromDays(value);
        public static TimeInterval Days(this Int value) => TimeInterval.FromDays(value);

        public static TimeInterval Weeks(this int value) => TimeInterval.FromWeeks(value);
        public static TimeInterval Weeks(this long value) => TimeInterval.FromWeeks(value);
        public static TimeInterval Weeks(this Int value) => TimeInterval.FromWeeks(value);

        public static TimeRange To(this Instant from, Instant to) => TimeRange.Create(from, to);

        public static TimeRange Year(this int year) => TimeRange.Create(Instant(year, 1, 1), Instant(year + 1, 1, 1));
        public static TimeRange Month(this int year, int month)
        {
            var normalizedMonth = Min(Max(1, month), 12);

            var daysInMonth = DateTime.DaysInMonth(year, normalizedMonth);

            var from = Instant(year, normalizedMonth, 1);
            var to = from + daysInMonth.Days();

            return TimeRange.Create(from, to);
        }
        public static TimeRange Day(this int year, int month, int day)
        {
            var normalizedMonth = Min(Max(1, month), 12);

            var daysInMonth = DateTime.DaysInMonth(year, normalizedMonth);
            var normalizedDay = Min(Max(1, day), daysInMonth);

            var from = Instant(year, normalizedMonth, normalizedDay);
            var to = from + 1.Days();

            return TimeRange.Create(from, to);

        }

        #endregion
    }
}
