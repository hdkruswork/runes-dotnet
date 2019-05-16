using Runes.Collections.Immutable;
using System;

using Mutables = System.Collections;
using static Runes.Collections.Immutable.Arrays;

namespace Runes.Collections
{
    public static class Traversable
    {
        public static ITraversable<A> ToTraversable<A>(this Mutables.Generic.IEnumerable<A> e)
        {
            if (e is Immutable.List<A> list)
                return list;

            var builder = Immutable.List<A>.Builder;
            foreach (var item in e)
            {
                builder = builder.Append(item);
            }
            return builder.Result();
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
        public That FoldLeftWhile<That>(That initialValue, Func<That, A, bool> p, Func<That, A, That> f)
        {
            var result = initialValue;
            ForeachWhile(it => p(result, it), it => result = f(result, it));
            return result;
        }
        public abstract Unit Foreach(Action<A> action);
        public abstract Unit ForeachWhile(Func<A, bool> p, Action<A> action);

        public abstract Mutables.Generic.IEnumerator<A> GetEnumerator();

        public virtual List<A> ToList() => FoldLeft(List<A>.Builder.NewBuilder(), (bf, it) => bf.Append(it)).Result();

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
