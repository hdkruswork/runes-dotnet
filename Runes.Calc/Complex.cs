using System;
using Runes.Math;

using static Runes.Calc.Complexes;
using static Runes.Predef;

namespace Runes.Calc
{
    public sealed class Complex<N> : IEquatable<Complex<N>>
        where N: Scalar<N>
    {
        public static Complex<N> operator +(Complex<N> a, Complex<N> b)
        {
            var (r1, i1) = a;
            var (r2, i2) = b;

            return Complex(r1 + r2, i1 + i2);
        }
        public static Complex<N> operator -(Complex<N> a, Complex<N> b)
        {
            var (r1, i1) = a;
            var (r2, i2) = b;

            return Complex(r1 - r2, i1 - i2);
        }
        public static Complex<N> operator *(Complex<N> a, Complex<N> b)
        {
            var (r1, i1) = a;
            var (r2, i2) = b;

            return Complex(r1*r2 - i1*i2, r1*i2 - r2*i1);
        }
        public static Complex<N> operator /(Complex<N> a, Complex<N> b) => a * b.Inverse;

        public N R { get; }
        public N I { get; }

        public bool IsScalar => I.IsZero;
        public Complex<N> Inverse { get; }

        public Complex<N> Add(Complex<N> another) => this + another;
        public Complex<N> Subtract(Complex<N> another) => this - another;
        public Complex<N> Multiply(Complex<N> another) => this * another;
        public Complex<N> Divide(Complex<N> another) => this / another;

        public bool Equals(Complex<N> other) => R.Equals(other.R) && I.Equals(other.I);

        public override bool Equals(object obj) => obj is Complex<N> other && Equals(other);

        public override int GetHashCode() => GetFieldsHashCode(R, I);

        public override string ToString() => $"({R}, {I}i)";

        public bool ToScalar(out N scalar)
        {
            if (IsScalar)
            {
                scalar = R;
                return true;
            }

            scalar = default;
            return false;
        }

        public void Deconstruct(out N r, out N i)
        {
            r = R;
            i = I;
        }

        internal Complex(N r, N i)
        {
            R = r;
            I = i;
            Inverse = Lazy(() =>
            {
                var r2_i2 = r*r + i*i;

                return Complex(r/r2_i2, -(i/r2_i2));
            });
        }
    }

    public static class Complexes
    {
        public static Complex<N> Complex<N>(N r, N i) where N: Scalar<N> => new Complex<N>(r, i);
    }
}
