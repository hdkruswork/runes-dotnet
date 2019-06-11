using System;

namespace Runes
{
    public abstract class Option<A> : MonadBase<A, Option<A>>
    {
        internal static readonly Option<A> None = new None<A>();

        public bool Equals(Option<A> other)
        {
            if (IsEmpty && other.IsEmpty)
                return true;
            else if (GetIfPresent(out A value))
                return other.Contains(value);
            else
                return false;
        }

        public Option<B> Collect<B>(Func<A, Option<B>> f) =>
            Collect<B, Option<B>, IMonadBuilder<B, Option<B>>>(f, Option<B>.CreateMonadBuilder());
        
        public override Option<A> Filter(Func<A, bool> p) =>
            Filter(p, true, CreateMonadBuilder());

        public override Option<A> FilterNot(Func<A, bool> p) =>
            Filter(p, false, CreateMonadBuilder());

        public Option<B> FlatMap<B>(Func<A, IMonad<B>> f) =>
            FlatMap<B, Option<B>, IMonadBuilder<B, Option<B>>>(f, Option<B>.CreateMonadBuilder());

        public A GetOrElse(Lazy<A> alternative) => GetIfPresent(out A res) ? res : alternative;

        public A GetOrDefault() => GetIfPresent(out A res) ? res : default;

        public Option<B> Map<B>(Func<A, B> f) =>
            Map<B, Option<B>, IMonadBuilder<B, Option<B>>>(f, Option<B>.CreateMonadBuilder());

        public Option<A> OrElse(Option<A> alternative) => NonEmpty ? this : alternative;

        public Option<A> OrElse(Lazy<Option<A>> alternative) => NonEmpty ? this : alternative;

        public (Option<X>, Option<Y>) Unzip<X, Y>(Func<A, (X, Y)> toPairFunc) =>
            Unzip<X, Y, Option<X>, Option<Y>, IMonadBuilder<X, Option<X>>, IMonadBuilder<Y, Option<Y>>>(toPairFunc, Option<X>.CreateMonadBuilder(), Option<Y>.CreateMonadBuilder());

        public Option<(A, B)> Zip<B>(IMonad<B> other) =>
            Zip<B, Option<(A, B)>, IMonadBuilder<(A, B), Option<(A, B)>>>(other, Option<(A, B)>.CreateMonadBuilder());

        // protected members

        protected static IMonadBuilder<A, Option<A>> CreateMonadBuilder() => new OptionBuilder();

        protected override IMonad<B> MonadCollect<B>(Func<A, Option<B>> f) => Collect(f);
        protected override IMonad<B> MonadFlatMap<B>(Func<A, IMonad<B>> f) => FlatMap(f);
        protected override IMonad<B> MonadMap<B>(Func<A, B> f) => Map(f);
        protected override (IMonad<X>, IMonad<Y>) MonadUnzip<X, Y>(Func<A, (X, Y)> toPairFunc) => Unzip(toPairFunc);
        protected override IMonad<(A, B)> MonadZip<B>(IMonad<B> other) => Zip(other);

        private protected Option() { }

        // inner classes

        public sealed class OptionBuilder : IMonadBuilder<A, Option<A>>
        {
            private Option<A> option = Predef.None<A>();

            public Option<A> Build() => option;

            public void SetValue(A value) =>
                option = Equals(value, null) ? Predef.None<A>() : Predef.Some(value);
        }
    }

    public sealed class None<A> : Option<A>
    {
        public override bool IsEmpty => true;

        public override bool GetIfPresent(out A value)
        {
            value = default;
            return false;
        }

        public override bool Equals(object obj) => obj is Option<A> opt && opt.IsEmpty;

        public override int GetHashCode() => 0.GetHashCode();

        public override string ToString() => "None";
    }

    public sealed class Some<A> : Option<A>
    {
        public A Value { get; }

        public override bool IsEmpty => false;

        public override bool GetIfPresent(out A value)
        {
            value = Value;
            return true;
        }

        public override bool Equals(object obj) => obj is Some<A> other && Equals(Value, other.Value);

        public override int GetHashCode() => Value.GetHashCode();

        public override string ToString() => $"Some({Value})";

        public void Deconstruct(out A value) => value = Value;

        internal Some(A value)
        {
            Value = value;
        }
    }
}
