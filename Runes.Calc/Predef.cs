using Runes.Collections;
using Runes.Math;
using System;
using System.Globalization;
using System.Numerics;

using static Runes.Predef;

namespace Runes.Calc
{
    public static class Predef
    {
        public static Int GCD(Int a, Int b) => Calc.Int.GreatestCommonDivisor(a, b);
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
        
        #region Int

        public static Int Int(byte i) => new Int(i);
        public static Int Int(sbyte i) => new Int(i);
        public static Int Int(short i) => new Int(i);
        public static Int Int(ushort i) => new Int(i);
        public static Int Int(int i) => new Int(i);
        public static Int Int(uint i) => new Int(i);
        public static Int Int(long i) => new Int(i);
        public static Int Int(ulong i) => new Int(i);
        public static Int Int(BigInteger i) => new Int(i);

        #endregion

        #region Rational

        public static Rational Rational(byte num) => Rational(num, 1, true);
        public static Rational Rational(sbyte num) => Rational(num, 1, true);
        public static Rational Rational(short num) => Rational(num, 1, true);
        public static Rational Rational(ushort num) => Rational(num, 1, true);
        public static Rational Rational(int num) => Rational(num, 1, true);
        public static Rational Rational(uint num) => Rational(num, 1, true);
        public static Rational Rational(long num) => Rational(num, 1, true);
        public static Rational Rational(ulong num) => Rational(num, 1, true);
        public static Rational Rational(decimal num) => Rational((double)num);
        public static Rational Rational(float num) => Rational((double)num);
        public static Rational Rational(double num)
        {
            if (double.IsNaN(num))
                throw new ArgumentException("Argument is not a number", nameof(num));
            else if (double.IsInfinity(num))
                throw new ArgumentException("Argument is infinity", nameof(num));

            var cultureInfo = CultureInfo.CurrentCulture;
            var decimalSeparator = cultureInfo.NumberFormat.NumberDecimalSeparator;

            var numText = num.ToString("F", cultureInfo);
            var split = numText.Split(new[] { decimalSeparator }, StringSplitOptions.RemoveEmptyEntries);

            var numerator = BigInteger.Parse(string.Join("", split));
            var denominator = split.Length > 1
                ? BigInteger.Pow(10, split[1].Length)
                : BigInteger.One;

            return Rational(numerator, denominator)
                .Simplify();
        }
        public static Rational Rational(BigInteger number) => Rational(Int(number), Calc.Int.One, true);
        public static Rational Rational(Int number) => Rational(number, Calc.Int.One, true);
        public static Rational Rational(Int num, Int den) => Rational(num, den, false);

        internal static Rational Rational(Int numerator, Int denominator, bool isSimplified)
        {
            if (denominator.Equals(Calc.Int.Zero))
                throw new DivideByZeroException();

            if (numerator.Equals(Calc.Int.Zero))
            {
                return Calc.Rational.Zero;
            }

            if (numerator.Equals(denominator))
            {
                return numerator.Sign == denominator.Sign
                    ? Calc.Rational.One
                    : Calc.Rational.MinusOne;
            }

            var aNum = Calc.Int.Abs(numerator);
            var aDen = Calc.Int.Abs(denominator);

            return numerator.Sign == denominator.Sign
                ? new Rational(aNum, aDen, isSimplified)
                : new Rational(-aNum, aDen, isSimplified);
        }

        #endregion

        #region Range

        public static Range To(this Int from, Int inclusiveTo) => Range(from, inclusiveTo);

        public static Range DownTo(this Int from, Int inclusiveTo) => Range(from, inclusiveTo, -1);

        public static Range Range(Int from, Int inclusiveTo) => Range(from, inclusiveTo, 1);

        public static Range Range(Int from, Int inclusiveTo, int increment)
        {
            var (min, max) = MinMax(OrderingBy<Int>(), from, inclusiveTo);
            return increment >= 0
                ? new Range(min, max, increment)
                : new Range(max, min, increment);
        }

        #endregion
    }
}
