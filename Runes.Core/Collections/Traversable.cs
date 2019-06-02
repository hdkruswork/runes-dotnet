using Runes.Collections.Immutable;
using System;
using static Runes.Collections.Builders;
using static Runes.Collections.Immutable.Arrays;
using static Runes.Collections.Immutable.Streams;
using Mutables = System.Collections;

namespace Runes.Collections
{
    public static class Traversable
    {
        public static ITraversable<A> ToTraversable<A>(this Mutables.Generic.IEnumerable<A> e)
        {
            if (e is List<A> list)
            {
                return list;
            }

            return Stream(e).ToList();
        }
    }

    public interface ITraversable<A> : Mutables.Generic.IEnumerable<A>
    {
        That FoldLeft<That>(That initialValue, Func<That, A, That> f);

        That FoldLeftWhile<That>(That initialValue, Func<That, A, bool> p, Func<That, A, That> f);

        Unit Foreach(Action<A> action);

        Unit ForeachWhile(Func<A, bool> p, Action<A> action);

        List<A> ToList();

        A[] ToArray();

        Stream<A> ToStream();
    }

    public abstract class Traversable<A> : ITraversable<A>
    {
        public virtual That FoldLeft<That>(That initialValue, Func<That, A, That> f)
        {
            var result = initialValue;
            Foreach(it => result = f(result, it));
            return result;
        }
        public virtual That FoldLeftWhile<That>(That initialValue, Func<That, A, bool> p, Func<That, A, That> f)
        {
            var result = initialValue;
            ForeachWhile(it => p(result, it), it => result = f(result, it));
            return result;
        }
        public abstract Unit Foreach(Action<A> action);
        public abstract Unit ForeachWhile(Func<A, bool> p, Action<A> action);

        public abstract Mutables.Generic.IEnumerator<A> GetEnumerator();

        public virtual List<A> ToList() => FoldLeft(ListBuilder<A>(), (bf, it) => bf.Append(it)).Build();

        public virtual A[] ToArray()
        {
            var builder = new Mutables.Generic.List<A>();
            Foreach(it => builder.Add(it));
            return builder.ToArray();
        }

        public Array<A> ToImmutableArray() => ImmutableArray(ToArray());

        public abstract Stream<A> ToStream();

        Mutables.IEnumerator Mutables.IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
