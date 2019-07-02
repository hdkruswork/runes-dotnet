using System;
using System.Globalization;
using System.Numerics;
using System.Text;

using static Runes.Predef;

namespace Runes.Math
{
    public class Rational : Scalar<Rational>, IEquatable<Int>
    {
        // Static members

        private const string m_solidus = @"/";

        public static readonly Rational MinusOne = Create(Int.MinusOne);

        public static readonly Rational One = Create(Int.One);

        public static readonly Rational Zero = Create(Int.Zero);

        public static implicit operator Rational(byte num) => Create(num);

        public static implicit operator Rational(sbyte num) => Create(num);

        public static implicit operator Rational(short num) => Create(num);

        public static implicit operator Rational(ushort num) => Create(num);

        public static implicit operator Rational(int num) => Create(num);

        public static implicit operator Rational(long num) => Create(num);

        public static implicit operator Rational(uint num) => Create(num);

        public static implicit operator Rational(ulong num) => Create(num);

        public static implicit operator Rational(BigInteger num) => Create(num);

        public static implicit operator Rational(Int num) => Create(num);

        public static implicit operator Rational(float num) => From(num);

        public static implicit operator Rational(double num) => From(num);

        public static implicit operator Rational(decimal num) => From((double)num);

        public static bool operator ==(Rational r1, Rational r2) => Compare(r1, r2) == 0;

        public static bool operator !=(Rational r1, Rational r2) => Compare(r1, r2) != 0;

        public static bool operator <=(Rational r1, Rational r2) => Compare(r1, r2) <= 0;

        public static bool operator >=(Rational r1, Rational r2) => Compare(r1, r2) >= 0;

        public static bool operator <(Rational r1, Rational r2) => Compare(r1, r2) < 0;

        public static bool operator >(Rational r1, Rational r2) => Compare(r1, r2) > 0;

        public static Rational operator +(Rational r) => r;

        public static Rational operator -(Rational r) => Create(-r.Numerator, r.Denominator, r.IsSimplified);

        public static Rational operator ++(Rational r) => r + One;

        public static Rational operator --(Rational r) => r - One;

        public static Rational operator +(Rational a, Rational b) => // a/b + c/d = (ad + bc)/bd
            Create(a.Numerator * b.Denominator + b.Numerator * a.Denominator, a.Denominator * b.Denominator)
                .Simplify();

        public static Rational operator -(Rational a, Rational b) =>  // a/b - c/d = (ad - bc)/bd
            Create(a.Numerator * b.Denominator - b.Numerator * a.Denominator, a.Denominator * b.Denominator)
                .Simplify();

        public static Rational operator *(Rational a, Rational b) => // a/b * c/d = ac/bd
            Create(a.Numerator * b.Numerator, a.Denominator * b.Denominator)
                .Simplify();

        public static Rational operator /(Rational a, Rational b) => // a/b / c/d = ad/bc
            Create(a.Numerator * b.Denominator, a.Denominator * b.Numerator)
                .Simplify();

        public static Rational operator %(Rational a, Rational b) => // a/b % c/d = (ad % bc)/bd
            Create((a.Numerator * b.Denominator) % (b.Numerator * a.Denominator), a.Denominator * b.Denominator);

        public static Rational Abs(Rational r) => r.Sign < 0 ? -r : r;

        public static int Compare(Rational r1, Rational r2) =>
            (r1.Numerator * r2.Denominator).CompareTo(r2.Numerator * r1.Denominator);

        public static bool TryParse(string str, out Rational number)
        {
            if (BigInteger.TryParse(str, out BigInteger bigInt))
            {
                number = Create(bigInt);
                return true;
            }

            if (double.TryParse(str, out double doubleValue))
            {
                number = From(doubleValue);
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

                number = Create(num, den);
                return true;
            }

            return false;
        }

        public static Rational From(double num)
        {
            if (double.IsNaN(num))
                throw new ArgumentException("Argument is not a number", nameof(num));

            if (double.IsInfinity(num))
                throw new ArgumentException("Argument is infinity", nameof(num));

            var cultureInfo = CultureInfo.CurrentCulture;
            var decimalSeparator = cultureInfo.NumberFormat.NumberDecimalSeparator;

            var numText = num.ToString("F", cultureInfo);
            var split = numText.Split(new[] { decimalSeparator }, StringSplitOptions.RemoveEmptyEntries);

            var numerator = BigInteger.Parse(string.Join("", split));
            var denominator = split.Length > 1
                ? BigInteger.Pow(10, split[1].Length)
                : BigInteger.One;

            return Create(numerator, denominator, false).Simplify();
        }

        public static Rational Create(Int number, Int denominator) => Create(number, denominator, false);

        private static Rational Create(Int number) => Create(number, 1, true);

        private static Rational Create(Int numerator, Int denominator, bool isSimplified)
        {
            if (denominator.Equals(Int.Zero))
                throw new DivideByZeroException();

            if (numerator.Equals(Int.Zero))
            {
                return Zero;
            }

            if (numerator.Equals(denominator))
            {
                return numerator.Sign == denominator.Sign
                    ? One
                    : MinusOne;
            }

            var aNum = Int.Abs(numerator);
            var aDen = Int.Abs(denominator);

            return numerator.Sign == denominator.Sign
                ? new Rational(aNum, aDen, isSimplified)
                : new Rational(-aNum, aDen, isSimplified);
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

        public Rational Inverse => Create(Denominator, Numerator, IsSimplified);

        public override int Sign => Numerator.Sign;

        public Int WholePart => Numerator / Denominator;

        public Rational FractionalPart => Create(Numerator % Denominator, Denominator);

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

            return Create(sNum, sDen, true);
        }

        public override int CompareTo(Rational other) => Compare(this, other);

        public bool Equals(Int num) => Equals(Create(num));

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

        public override Rational ToRational() => this;

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
