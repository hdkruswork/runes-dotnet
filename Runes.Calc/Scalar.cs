using System;

using static Runes.Predef;

namespace Runes.Calc
{
    public abstract class Scalar<This> : IComparable<This>, IEquatable<This>
        where This : Scalar<This>
    {
        public static This operator +(Scalar<This> a, This b) => a.Add(b);
        public static This operator -(Scalar<This> a, This b) => a.Substract(b);
        public static This operator *(Scalar<This> a, This b) => a.Multiply(b);
        public static This operator /(Scalar<This> a, This b) => a.Divide(b);

        public static This operator -(Scalar<This> a) => a.Negate();

        public static bool operator ==(Scalar<This> a, This b) => a.EqualsTo(b);
        public static bool operator !=(Scalar<This> a, This b) => !a.EqualsTo(b);
        public static bool operator <(Scalar<This> a, This b) => a.LesserThan(b);
        public static bool operator >(Scalar<This> a, This b) => a.GreaterThan(b);
        public static bool operator <=(Scalar<This> a, This b) => a.LesserEqualsThan(b);
        public static bool operator >=(Scalar<This> a, This b) => a.GreaterEqualsThan(b);

        public abstract This Add(This another);
        public abstract This Substract(This another);
        public abstract This Multiply(This another);
        public abstract This Divide(This another);
        public virtual Option<This> SafeDivide(This another) => Option(another).FilterNot(n => n.IsZero).Map(Divide);

        public abstract This Negate();

        public virtual bool EqualsTo(This another) => CompareTo(another) == 0;
        public virtual bool NonEqualsTo(This another) => CompareTo(another) != 0;
        public virtual bool GreaterThan(This another) => CompareTo(another) > 0;
        public virtual bool GreaterEqualsThan(This another) => CompareTo(another) >= 0;
        public virtual bool LesserThan(This another) => CompareTo(another) < 0;
        public virtual bool LesserEqualsThan(This another) => CompareTo(another) <= 0;

        public abstract int CompareTo(This another);
        public bool Equals(This other) => EqualsTo(other);
        public override bool Equals(object obj) => obj is This other && EqualsTo(other);
        public override int GetHashCode() => InnerGetHashCode();

        public abstract int Sign { get; }
        public abstract bool IsOne { get; }
        public abstract bool IsZero { get; }

        protected abstract int InnerGetHashCode();
    }
}
