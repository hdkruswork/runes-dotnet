using System;

namespace Runes
{
    public interface IPartialFunction<in I, out O>
    {
        bool IsDefinedAt(I arg);
        O Apply(I arg);
    }

    public sealed class PartialFunction<I, O> : IPartialFunction<I, O>
    {
        public O Apply(I input) => ApplyFunc(input);

        public bool IsDefinedAt(I input) => IsDefinedAtFunc(input);
        
        internal PartialFunction(Func<I, O> applyFunc, Func<I, bool> isDefinedAtFunc)
        {
            ApplyFunc = applyFunc;
            IsDefinedAtFunc = isDefinedAtFunc;
        }

        private Func<I, bool> IsDefinedAtFunc { get; }
        private Func<I, O> ApplyFunc { get; }
    }

    public static class PartialFunction
    {
        public static IPartialFunction<I, O> From<I, O>(Func<I, O> applyFunc, Func<I, bool> isDefinedAtFunc) =>
            new PartialFunction<I, O>(applyFunc, isDefinedAtFunc);

        public static IPartialFunction<I, O> From<I, O>(Func<I, O> applyFunc) =>
            From(applyFunc, i => Code.Try(() => applyFunc(i)).IsSuccess);
    }
}
