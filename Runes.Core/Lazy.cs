using System;

using static Runes.OptionExtensions;

namespace Runes
{
    public sealed class Lazy<A>
    {
        public static implicit operator Lazy<A>(Func<A> func) => new Lazy<A>(func);
        public static implicit operator A(Lazy<A> lazy) => lazy.Get();

        public bool IsComputed { get; private set; }

        public A Get() => GetIfNotComputed().GetOrElse(value);

        public Option<A> GetIfComputed() => IsComputed ? Some(value) : Option<A>.None;

        public Option<A> GetIfNotComputed()
        {
            Option<A> res = None<A>();
            if (!IsComputed)
            {
                lock(this)
                {
                    if (!IsComputed)
                    {
                        value = getValueFunc();
                        IsComputed = true;
                        res = Some(value);
                    }
                }
            }
            return res;
        }

        public override string ToString() =>
            GetIfComputed()
                .Map(v => v.ToString())
                .GetOrElse("Lazy(...)");

        internal Lazy(Func<A> function)
        {
            getValueFunc = function;
            value = default;
            IsComputed = false;
        }

        private A value;
        private readonly Func<A> getValueFunc;
    }

    public static class LazyExtensions
    {
        public static Lazy<A> Lazy<A>(Func<A> get) => new Lazy<A>(get);
    }
}
