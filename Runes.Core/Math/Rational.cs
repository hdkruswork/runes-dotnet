using System;
using System.Globalization;
using System.Numerics;
using System.Text;

using static Runes.Math.Predef;
using static Runes.Predef;

namespace Runes.Math
{
    public class Rational
        : Scalar<Rational>, IEquatable<Int>
    {
        // Static members

        private const string m_solidus = @"/";

        public static readonly Rational MinusOne = Rational(Int.MinusOne);

        public static readonly Rational One = Rational(Int.One);

        public static readonly Rational Zero = Rational(Int.Zero);

        public static implicit operator Rational(byte num) => Rational((BigInteger)num);

        public static implicit operator Rational(sbyte num) => Rational((BigInteger)num);

        public static implicit operator Rational(short num) => Rational((BigInteger)num);

        public static implicit operator Rational(ushort num) => Rational((BigInteger)num);

        public static implicit operator Rational(int num) => Rational((BigInteger)num);

        public static implicit operator Rational(long num) => Rational((BigInteger)num);

        public static implicit operator Rational(uint num) => Rational((BigInteger)num);

        public static implicit operator Rational(ulong num) => Rational((BigInteger)num);

        public static implicit operator Rational(BigInteger num) => Rational(num);

        public static implicit operator Rational(Int num) => Rational(num);

        public static implicit operator Rational(float num) => Rational(num);

        public static implicit operator Rational(double num) => Rational(num);

        public static bool operator ==(Rational r1, Rational r2) => Compare(r1, r2) == 0;

        public static bool operator !=(Rational r1, Rational r2) => Compare(r1, r2) != 0;

        public static bool operator <=(Rational r1, Rational r2) => Compare(r1, r2) <= 0;

        public static bool operator >=(Rational r1, Rational r2) => Compare(r1, r2) >= 0;

        public static bool operator <(Rational r1, Rational r2) => Compare(r1, r2) < 0;

        public static bool operator >(Rational r1, Rational r2) => Compare(r1, r2) > 0;

        public static Rational operator +(Rational r) => r;

        public static Rational operator -(Rational r) => Rational(-r.Numerator, r.Denominator);

        public static Rational operator ++(Rational r) => r + One;

        public static Rational operator --(Rational r) => r - One;

        public static Rational operator +(Rational a, Rational b) => // a/b + c/d = (ad + bc)/bd
            Rational(a.Numerator * b.Denominator + b.Numerator * a.Denominator, a.Denominator * b.Denominator)
                .Simplify();

        public static Rational operator -(Rational a, Rational b) =>  // a/b - c/d = (ad - bc)/bd
            Rational(a.Numerator * b.Denominator - b.Numerator * a.Denominator, a.Denominator * b.Denominator)
                .Simplify();

        public static Rational operator *(Rational a, Rational b) => // a/b * c/d = ac/bd
            Rational(a.Numerator * b.Numerator, a.Denominator * b.Denominator)
                .Simplify();

        public static Rational operator /(Rational a, Rational b) => // a/b / c/d = ad/bc
            Rational(a.Numerator * b.Denominator, a.Denominator * b.Numerator)
                .Simplify();

        public static Rational operator %(Rational a, Rational b) => // a/b % c/d = (ad % bc)/bd
            Rational((a.Numerator * b.Denominator) % (b.Numerator * a.Denominator), a.Denominator * b.Denominator);

        public static Rational Abs(Rational r) => r.Sign < 0 ? -r : r;

        public static int Compare(Rational r1, Rational r2) =>
            (r1.Numerator * r2.Denominator).CompareTo(r2.Numerator * r1.Denominator);

        public static bool TryParse(string str, out Rational number)
        {
            if (BigInteger.TryParse(str, out BigInteger bigInt))
            {
                number = Rational(bigInt);
                return true;
            }

            if (double.TryParse(str, out double doubleValue))
            {
                number = Rational(doubleValue);
                return true;
            }

            number = Zero;
            var split = str.Split(new[] { m_solidus }, StringSplitOptions.RemoveEmptyEntries);
            if (split.Length > 0 && BigInteger.TryParse(split[0], out BigInteger num))
            {
                if (split.Length <= 1 || !BigInteger.TryParse(split[1], out BigInteger den))
                {
                    den = BigInteger.One;
                }

                number = Rational(num, den);
                return true;
            }

            return false;
        }

        // Instance members

        internal Rational(Int numerator, Int denominator, bool isSimplified)
        {
            Numerator = numerator;
            Denominator = denominator;
            IsSimplified = isSimplified;
        }

        public override bool IsZero => this == Zero;

        public override bool IsOne => this == One;

        public bool IsMinusOne => this == MinusOne;

        public bool IsInteger => Simplify().Denominator == 1;

        public bool IsSimplified { get; }

        public Rational Inverse => Rational(Denominator, Numerator, IsSimplified);

        public override int Sign => Numerator.Sign;

        public Int WholePart => Numerator / Denominator;

        public Rational FractionalPart => Rational(Numerator % Denominator, Denominator);

        public Int Numerator { get; }

        public Int Denominator { get; }

        public override Rational Add(Rational r) => this + r;

        public override Rational Divide(Rational r) => this / r;

        public override Rational Multiply(Rational r) => this * r;

        public override Rational Subtract(Rational r) => this - r;

        public override Rational Negate() => -this;

        public Rational Simplify()
        {
            if (IsSimplified) return this;
            var gcd = GCD(Numerator, Denominator);
            var sNum = Numerator / gcd;
            var sDen = Denominator / gcd;

            return Rational(sNum, sDen, true);
        }

        public override int CompareTo(Rational other) => Compare(this, other);

        public bool Equals(Int num) => Equals(Rational(num));

        public double ToDouble()
        {
            var s = Simplify();

            return s.IsInteger
                ? (double)s.Numerator
                : System.Math.Exp(s.Numerator.Log() - s.Denominator.Log());
        }

        public string ToNumberString()
        {
            var s = Simplify();

            return s.IsInteger
                ? s.Numerator.ToString()
                : ToDouble().ToString(CultureInfo.InvariantCulture);
        }

        public override bool Equals(object obj)
        {
            switch (obj)
            {
                case Rational num:
                    return Equals(num);

                case BigInteger num:
                    return Equals(num);

                default:
                    return false;
            }
        }

        public override int GetHashCode()
        {
            var s = Simplify();

            return GetFieldsHashCode(s.Numerator, s.Denominator);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append(Numerator.ToString("R", CultureInfo.InvariantCulture));
            if (!IsInteger)
            {
                sb.Append(m_solidus);
                sb.Append(Denominator.ToString("R", CultureInfo.InvariantCulture));
            }

            return sb.ToString();
        }

        public void Deconstruct(out Int num, out Int den)
        {
            num = Numerator;
            den = Denominator;
        }

        protected override int InnerGetHashCode() => GetHashCode();
    }
}
