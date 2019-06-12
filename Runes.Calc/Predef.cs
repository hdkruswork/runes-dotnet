using Runes.Collections;
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

        public static sbyte Max(params sbyte[] values) => Max(values, Compare, sbyte.MaxValue);
        public static sbyte Min(params sbyte[] values) => Min(values, Compare, sbyte.MinValue);
        public static sbyte Max(this ITraversable<sbyte> values) => Max(values, Compare, sbyte.MaxValue);
        public static sbyte Min(this ITraversable<sbyte> values) => Min(values, Compare, sbyte.MinValue);
        public static byte Max(params byte[] values) => Max(values, Compare, byte.MaxValue);
        public static byte Min(params byte[] values) => Min(values, Compare, byte.MinValue);
        public static byte Max(this ITraversable<byte> values) => Max(values, Compare, byte.MaxValue);
        public static byte Min(this ITraversable<byte> values) => Min(values, Compare, byte.MinValue);
        public static short Max(params short[] values) => Max(values, Compare, short.MaxValue);
        public static short Min(params short[] values) => Min(values, Compare, short.MinValue);
        public static short Max(this ITraversable<short> values) => Max(values, Compare, short.MaxValue);
        public static short Min(this ITraversable<short> values) => Min(values, Compare, short.MinValue);
        public static ushort Max(params ushort[] values) => Max(values, Compare, ushort.MaxValue);
        public static ushort Min(params ushort[] values) => Min(values, Compare, ushort.MinValue);
        public static ushort Max(this ITraversable<ushort> values) => Max(values, Compare, ushort.MaxValue);
        public static ushort Min(this ITraversable<ushort> values) => Min(values, Compare, ushort.MinValue);
        public static int Max(params int[] values) => Max(values, Compare, int.MaxValue);
        public static int Min(params int[] values) => Min(values, Compare, int.MinValue);
        public static int Max(this ITraversable<int> values) => Max(values, Compare, int.MaxValue);
        public static int Min(this ITraversable<int> values) => Min(values, Compare, int.MinValue);
        public static uint Max(params uint[] values) => Max(values, Compare, uint.MaxValue);
        public static uint Min(params uint[] values) => Min(values, Compare, uint.MinValue);
        public static uint Max(this ITraversable<uint> values) => Max(values, Compare, uint.MaxValue);
        public static uint Min(this ITraversable<uint> values) => Min(values, Compare, uint.MinValue);
        public static long Max(params long[] values) => Max(values, Compare, long.MaxValue);
        public static long Min(params long[] values) => Min(values, Compare, long.MinValue);
        public static long Max(this ITraversable<long> values) => Max(values, Compare, long.MaxValue);
        public static long Min(this ITraversable<long> values) => Min(values, Compare, long.MinValue);
        public static ulong Max(params ulong[] values) => Max(values, Compare, ulong.MaxValue);
        public static ulong Min(params ulong[] values) => Min(values, Compare, ulong.MinValue);
        public static ulong Max(this ITraversable<ulong> values) => Max(values, Compare, ulong.MaxValue);
        public static ulong Min(this ITraversable<ulong> values) => Min(values, Compare, ulong.MinValue);
        public static float Max(params float[] values) => Max(values, Compare, float.MaxValue);
        public static float Min(params float[] values) => Min(values, Compare, float.MinValue);
        public static float Max(this ITraversable<float> values) => Max(values, Compare, float.MaxValue);
        public static float Min(this ITraversable<float> values) => Min(values, Compare, float.MinValue);
        public static double Max(params double[] values) => Max(values, Compare, double.MaxValue);
        public static double Min(params double[] values) => Min(values, Compare, double.MinValue);
        public static double Max(this ITraversable<double> values) => Max(values, Compare, double.MaxValue);
        public static double Min(this ITraversable<double> values) => Min(values, Compare, double.MinValue);
        public static decimal Max(params decimal[] values) => Max(values, Compare, decimal.MaxValue);
        public static decimal Min(params decimal[] values) => Min(values, Compare, decimal.MinValue);
        public static decimal Max(this ITraversable<decimal> values) => Max(values, Compare, decimal.MaxValue);
        public static decimal Min(this ITraversable<decimal> values) => Min(values, Compare, decimal.MinValue);

        public static A Max<A>(this A[] array, Func<A, A, int> comparator, A initialMaxValue) =>
            array.FoldLeft(initialMaxValue, (agg, curr) => comparator(agg, curr) > 0 ? agg : curr);
        public static A Min<A>(this A[] array, Func<A, A, int> comparator, A initialMinValue) =>
            array.FoldLeft(initialMinValue, (agg, curr) => comparator(agg, curr) < 0 ? agg : curr);
        public static A Max<A>(this ITraversable<A> traversable, Func<A, A, int> comparator, A initialMaxValue) =>
            traversable.FoldLeft(initialMaxValue, (agg, curr) => comparator(agg, curr) > 0 ? agg : curr);
        public static A Min<A>(this ITraversable<A> traversable, Func<A, A, int> comparator, A initialMinValue) =>
            traversable.FoldLeft(initialMinValue, (agg, curr) => comparator(agg, curr) < 0 ? agg : curr);

        public static Option<A> Max<A>(this A[] array, Func<A, A, int> comparator) => Max(Array(array), comparator);
        public static Option<A> Min<A>(this A[] array, Func<A, A, int> comparator) => Min(Array(array), comparator);
        public static Option<A> Max<A>(this ITraversable<A> traversable, Func<A, A, int> comparator) =>
            traversable
                .HeadOption
                .Map(first => Max(traversable.Tail, comparator, first));
        public static Option<A> Min<A>(this ITraversable<A> traversable, Func<A, A, int> comparator) =>
            traversable
                .HeadOption
                .Map(first => Min(traversable.Tail, comparator, first));

        #endregion

        public static Int Int(byte i) => new Int(i);
        public static Int Int(sbyte i) => new Int(i);
        public static Int Int(short i) => new Int(i);
        public static Int Int(ushort i) => new Int(i);
        public static Int Int(int i) => new Int(i);
        public static Int Int(uint i) => new Int(i);
        public static Int Int(long i) => new Int(i);
        public static Int Int(ulong i) => new Int(i);
        public static Int Int(BigInteger i) => new Int(i);

        public static Rational Rational(byte num) => Rational(num, 1, true);
        public static Rational Rational(sbyte num) => Rational(num, 1, true);
        public static Rational Rational(short num) => Rational(num, 1, true);
        public static Rational Rational(ushort num) => Rational(num, 1, true);
        public static Rational Rational(int num) => Rational(num, 1, true);
        public static Rational Rational(uint num) => Rational(num, 1, true);
        public static Rational Rational(long num) => Rational(num, 1, true);
        public static Rational Rational(ulong num) => Rational(num, 1, true);
        public static Rational Rational(decimal num) => Rational((double) num);
        public static Rational Rational(float num) => Rational((double) num);
        public static Rational Rational(double num)
        {
            if (double.IsNaN(num))
                throw new ArgumentException("Argument is not a number", nameof(num));
            else if (double.IsInfinity(num))
                throw new ArgumentException("Argument is infinity", nameof(num));

            var cultureInfo = CultureInfo.CurrentCulture;
            var decimalSeparator = cultureInfo.NumberFormat.NumberDecimalSeparator;

            var numText = num.ToString("F", cultureInfo);
            var split = numText.Split(new string[] { decimalSeparator }, StringSplitOptions.RemoveEmptyEntries);

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
    }
}
