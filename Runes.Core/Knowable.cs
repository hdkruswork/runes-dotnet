using System;

using static Runes.Predef;

namespace Runes
{
    public abstract class Knowable<A> : IEquatable<Knowable<A>>, IEquatable<A>
    {
        public abstract bool IsKnown { get; }
        public bool IsUnknown => !IsKnown;

        public bool Equals(Knowable<A> other) => this.ToOption().Equals(other.ToOption());

        public bool Equals(A other) => this is Known<A> known && Equals(known, other);

        public override bool Equals(object obj) =>
            (obj is A a && Equals(a)) || (obj is Knowable<A> knowable && Equals(knowable));

        public override int GetHashCode() => this.ToOption().GetHashCode();

        public bool Contains(A value) =>
            this is Known<A> known && Equals(value, known.Value);

        public Knowable<B> FlatMap<B>(Func<A, Knowable<B>> f) =>
            this is Known<A> known ? f(known.Value) : Unknown<B>();

        public Knowable<B> FlatMap<B>(Func<A, Option<B>> f) =>
            FlatMap(a => f(a).ToKnowable());

        public Knowable<B> Map<B>(Func<A, B> f) =>
            this is Known<A> known ? Known(f(known.Value)) : (Knowable<B>) Unknown<B>();
    }

    public sealed class Known<A> : Knowable<A>
    {
        public override bool IsKnown => true;

        public A Value { get; }

        public override string ToString() => $"Known({Value})";

        public void Deconstruct(out A value) => value = Value;

        internal Known(A value) => Value = value;
    }

    public sealed class Unknown<A> : Knowable<A>
    {
        public override bool IsKnown => false;

        public override string ToString() => "Unknown";

        internal Unknown() { }
    }
}
