using Runes.Collections;
using System;
using System.Numerics;

using static Runes.Math.Predef;
using static Runes.Predef;

namespace Runes.Math
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
        public static implicit operator BigInteger(Int num) => num.value;

        public static explicit operator float(Int num) => (float)num.value;
        public static explicit operator double(Int num) => (double)num.value;

        public static Int operator %(Int a, Int b) => a.Remainder(b);

        public static readonly Int MinusOne = -1;
        public static readonly Int One = 1;
        public static readonly Int Zero = 0;

        public static Stream<Int> AllValues
        {
            get
            {
                var zipped = StartStream(1)
                    .Zip(StartStream(-1, -1))
                    .FlatMap(pair => {
                        (Int pos, Int neg) = pair;
                        return Stream(pos, Stream(neg));
                    });

                return Stream(Zero).Append(() => zipped);
            }
        }

        public static Stream<Int> GetValuesFrom(Int from) =>
            StartStream(from)
                .Map(i => Int(i));

        public static Stream<Int> GetValuesInRange(Int from, Int to, bool inclusive = true) => 
            StartStream(from)
                .TakeWhile(num => inclusive ? num <= to : num < to)
                .Map(n => Int(n));

        public static Int GreatestCommonDivisor(Int a, Int b) => BigInteger.GreatestCommonDivisor(a, b);

        public static Int Abs(Int num) => num.Sign == One.Sign ? num : Int(BigInteger.Abs(num));

        // Instance members

        private readonly BigInteger value;

        public override int Sign => value.Sign;

        public override bool IsOne => this == One;

        public override bool IsZero => this == Zero;

        public override int CompareTo(Int another) => value.CompareTo(another);

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

        public Rational ExactDivide(Int another) => Rational(this, another);

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

        public override Int Subtract(Int another)
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

        public double Log() => BigInteger.Log(this);

        public Int Remainder(Int another) => BigInteger.Remainder(this, another);

        public override bool Equals(object obj) => obj is Int i && EqualsTo(i);

        public bool Equals(BigInteger other) => Equals(value, other);

        public bool Equals(int other) => Equals(value, new BigInteger(other));

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
