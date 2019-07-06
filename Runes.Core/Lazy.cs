using System;

using static Runes.Predef;

namespace Runes
{
    public sealed class Lazy<A>
    {
        public static implicit operator Lazy<A>(Func<A> f) => Lazy(f);
        public static implicit operator A(Lazy<A> lazy) => lazy.Get();

        public static Lazy<A> Create(A value) => new Lazy<A>(value);
        public static Lazy<A> Create(Func<A> f) => new Lazy<A>(f);

        public bool IsEvaluated { get; private set; }

        public A Get() => GetIfNotEvaluated().GetOrElse(value);

        public Option<A> GetIfEvaluated() => IsEvaluated ? Some(value) : Option<A>.None;

        public Option<A> GetIfNotEvaluated()
        {
            Option<A> res = None<A>();
            if (!IsEvaluated)
            {
                lock (syncObj)
                {
                    if (!IsEvaluated)
                    {
                        value = getValueFunc();
                        IsEvaluated = true;
                        getValueFunc = null;
                        syncObj = null;
                        res = Some(value);
                    }
                }
            }
            return res;
        }

        public override string ToString() =>
            GetIfEvaluated()
                .Map(v => v.ToString())
                .GetOrElse("Lazy(...)");

        private Lazy(Func<A> f)
        {
            getValueFunc = f;
            value = default;
            IsEvaluated = false;
            syncObj = new object();
        }
        private Lazy(A value)
        {
            getValueFunc = null;
            this.value = value;
            IsEvaluated = true;
            syncObj = null;
        }

        private A value;
        private Func<A> getValueFunc;
        private object syncObj;
    }
}
