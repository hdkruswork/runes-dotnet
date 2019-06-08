using Runes.Collections.Immutable;
using System;
using System.Numerics;

using static Runes.Calc.Predef;
using static Runes.Collections.Immutable.Streams;
using static Runes.Predef;

namespace Runes.Calc
{
    public sealed class Int : Scalar<Int>, IEquatable<BigInteger>, IEquatable<int>
    {
        // Static members

        public static implicit operator Int(byte value) => Int(value);
        public static implicit operator Int(sbyte value) => Int(value);
        public static implicit operator Int(short value) => Int(value);
        public static implicit operator Int(ushort value) => Int(value);
        public static implicit operator Int(int value) => Int(value);
        public static implicit operator Int(uint value) => Int(value);
        public static implicit operator Int(long value) => Int(value);
        public static implicit operator Int(ulong value) => Int(value);
        public static implicit operator Int(BigInteger value) => Int(value);

        public static explicit operator float(Int num) => (float)num.value;
        public static explicit operator double(Int num) => (double)num.value;

        public static Int operator %(Int a, Int b) => a.Remainder(b);

        public static readonly Int MinusOne = Int(-1);
        public static readonly Int One = Int(1);
        public static readonly Int Zero = Int(0);

        public static Stream<Int> AllValues
        {
            get
            {
                Stream<Int> getNextStream(Int value) =>
                    Stream(value, Stream(-value, Lazy(() => getNextStream(value + 1))));

                return Stream(Zero, Lazy(() => getNextStream(1)));
            }
        }

        public static Stream<Int> StartStream(Int startingValue) =>
            Stream(startingValue, Lazy(() => StartStream(startingValue + 1)));

        public static Stream<Int> StartStream(Int from, Int to) => 
            from <= to
                ? Stream(from, Lazy(() => StartStream(from + 1, to)))
                : Empty<Int>();

        public static Int GreatestCommonDivisor(Int a, Int b) => BigInteger.GreatestCommonDivisor(a.value, b.value);

        public static Int Abs(Int num) => num.value.Sign == BigInteger.One.Sign ? num : BigInteger.Abs(num.value);

        // Instance members

        private readonly BigInteger value;

        public override int Sign => value.Sign;

        public override bool IsOne => value == 1;

        public override bool IsZero => value == 0;

        public override int CompareTo(Int another) => value.CompareTo(another.value);

        public override Int Add(Int another)
        {
            if (another.IsZero)
            {
                return this;
            }
            else if (IsZero)
            {
                return another;
            }
            else
            {
                return value + another.value;
            }
        }

        public override Int Divide(Int another)
        {
            if (another.IsOne)
            {
                return this;
            }
            else
            {
                return value / another.value;
            }
        }

        public Rational ExactDivide(Int another) => Rational(value, another.value);

        public override Int Multiply(Int another)
        {
            if (another.IsOne)
            {
                return this;
            }
            else if (IsOne)
            {
                return another;
            }
            else
            {
                return value * another.value;
            }
        }

        public override Int Negate() => -value;

        public override Int Substract(Int another)
        {
            if (another.IsZero)
            {
                return this;
            }
            else
            {
                return value - another.value;
            }
        }

        public double Log() => BigInteger.Log(value);

        public Int Remainder(Int another) => this - (this / another) * another;

        public override bool Equals(object obj) => obj is Int i && EqualsTo(i);

        public bool Equals(BigInteger other) => value.Equals(other);

        public bool Equals(int other) => value.Equals(other);

        public override int GetHashCode() => value.GetHashCode();

        public override string ToString() => value.ToString();

        public string ToString(string format, IFormatProvider formatProvider) => value.ToString(format, formatProvider);

        internal Int(BigInteger value)
        {
            this.value = value;
        }

        protected override int InnerGetHashCode() => GetHashCode();
    }
}
