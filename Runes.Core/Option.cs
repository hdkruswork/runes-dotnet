using System;
using Runes.Collections;

using static Runes.Predef;

namespace Runes
{
    public abstract class Option<A> : MonadBase<A, Option<A>>
    {
        public static readonly IFactory<A, Option<A>> Factory = new OptionFactory();

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

        public Option<B> Collect<B>(Func<A, Option<B>> f) => GetIfPresent(out var value) ? f(value) : None<B>();

        public Option<B> FlatMap<B>(Func<A, IIterable<B>> f) => FlatMap(f, Option<B>.Factory);

        public Option<B> Map<B>(Func<A, B> f) => Map(f, Option<B>.Factory);

        public (Option<X>, Option<Y>) Unzip<X, Y>(Func<A, (X, Y)> f) =>
            Unzip(f, Option<X>.Factory, Option<Y>.Factory);

        public Option<(A, B)> Zip<B>(IIterable<B> other) => Zip(other, Option<(A, B)>.Factory);

        // protected members

        protected override IFactory<A, Option<A>> GetFactory() => Factory;

        protected override IFactory<B, IMonad<B>> GetFactory<B>() => Option<B>.Factory;

        private protected Option() { }

        // inner classes

        public sealed class OptionFactory : FactoryBase<A, Option<A>>
        {
            public override IIterableBuilder<A, Option<A>> NewBuilder() => new OptionBuilder();

            internal OptionFactory() { }
        }

        public sealed class OptionBuilder : MonadBuilder<OptionBuilder>
        {
            public override Option<A> Build() => innerList.HeadOption;

            public override Option<A> GetEmpty() => None<A>();

            internal OptionBuilder() { }
        }
    }

    public sealed class None<A> : Option<A>
    {
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
