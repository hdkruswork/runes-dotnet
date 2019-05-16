using System;

using static Runes.Options;

namespace Runes
{
    public static class Options
    {
        public static Option<A> Option<A>(A value) where A: class => value != null ? Some(value) : None<A>();
        public static Option<A> Option<A>(A? value) where A: struct => value.HasValue ? Some(value.Value) : None<A>();

        public static Option<A> None<A>() => Runes.Option<A>.None;
        public static Some<A> Some<A>(A value) => new Some<A>(value);
    }

    public abstract class Option<A> : MonadLike<A, Option<A>>
    {
        public static IMonadBuilder<A, Option<A>> Builder => OptionBuilder.Object;

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

        public Option<B> FlatMap<B>(Func<A, Option<B>> f) => FlatMap(f, Option<B>.Builder);

        public A GetOrElse(A alternative) => GetIfPresent(out A res) ? res : alternative;

        public A GetOrElse(Lazy<A> alternative) => GetIfPresent(out A res) ? res : alternative;

        public Option<B> Map<B>(Func<A, B> f) => Map(f, Option<B>.Builder);

        public Option<A> OrElse(Option<A> alternative) => NonEmpty ? this : alternative;

        public Option<A> OrElse(Lazy<Option<A>> alternative) => NonEmpty ? this : alternative;

        public Option<(A, B)> Zip<B>(Option<B> other) => Zip(other, Option<(A, B)>.Builder);

        public Option<(A, int)> ZipWithIndex() => ZipWithIndex(Option<(A, int)>.Builder);

        protected override IMonadBuilder<A, Option<A>> GetBuilder() => Builder;

        private protected Option() { }

        private sealed class OptionBuilder: MonadBuilder<A, Option<A>>
        {
            public static readonly OptionBuilder Object = new OptionBuilder();

            public override Option<A> BuildFrom(A value) => Some(value);

            public override Option<A> Empty() => None<A>();

            public override Option<A> TransformFrom(IMonad<A> other) =>
                other is Option<A> option ? option : base.TransformFrom(other);

            private OptionBuilder() { }
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

        public override int GetHashCode() => "None".GetHashCode();

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

        public override int GetHashCode() => "Some".GetHashCode() ^ Value.GetHashCode();

        public override string ToString() => $"Some({Value})";

        public void Deconstruct(out A value) => value = Value;

        internal Some(A value)
        {
            Value = value;
        }
    }
}
