using System;
using Runes.Math;
using static Runes.Predef;

namespace Runes.Math
{
    public abstract class Scalar<This> : IOrdered<This>, IEquatable<This>
        where This : Scalar<This>
    {
        public static This operator +(Scalar<This> a, This b) => a.Add(b);
        public static This operator -(Scalar<This> a, This b) => a.Subtract(b);
        public static This operator *(Scalar<This> a, This b) => a.Multiply(b);
        public static This operator /(Scalar<This> a, This b) => a.Divide(b);

        public static This operator -(Scalar<This> a) => a.Negate();

        public static bool operator ==(Scalar<This> a, This b) => Equals(a, b);
        public static bool operator !=(Scalar<This> a, This b) => !(a == b);
        public static bool operator <(Scalar<This> a, This b) => a.LesserThan(b);
        public static bool operator >(Scalar<This> a, This b) => a.GreaterThan(b);
        public static bool operator <=(Scalar<This> a, This b) => a.LesserEqualsThan(b);
        public static bool operator >=(Scalar<This> a, This b) => a.GreaterEqualsThan(b);

        public static bool Equals(Scalar<This> a, This b)
        {
            return ReferenceEquals(a, b) || !(a is null) && a.EqualsTo(b);
        }

        public abstract This Add(This another);
        public abstract This Subtract(This another);
        public abstract This Multiply(This another);
        public abstract This Divide(This another);
        public virtual Option<This> SafeDivide(This another) => Option(another).FilterNot(n => n.IsZero).Map(Divide);

        public abstract This Negate();

        public virtual bool EqualsTo(This another) => ((IOrdered<This>)this).EqualsTo(another);
        public virtual bool NonEqualsTo(This another) => ((IOrdered<This>)this).NonEqualsTo(another);
        public virtual bool GreaterThan(This another) => ((IOrdered<This>)this).GreaterThan(another);
        public virtual bool GreaterEqualsThan(This another) => ((IOrdered<This>)this).GreaterEqualsThan(another);
        public virtual bool LesserThan(This another) => ((IOrdered<This>)this).LesserThan(another);
        public virtual bool LesserEqualsThan(This another) => ((IOrdered<This>)this).LesserEqualsThan(another);

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
