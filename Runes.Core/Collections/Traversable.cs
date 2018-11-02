using Runes.Collections.Immutable;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Runes.Collections
{
    public static class Traversable
    {
        public static ITraversable<A> ToTraversable<A>(this IEnumerable<A> e)
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

    public interface ITraversable<A>: IEnumerable<A>
    {
        That FoldLeft<That>(That initialValue, Func<That, A, That> f);

        That FoldLeftWhile<That>(That initialValue, Func<That, A, bool> p, Func<That, A, That> f);

        Unit Foreach(Action<A> action);

        Unit ForeachWhile(Func<A, bool> p, Action<A> action);

        Immutable.List<A> ToList();

        IStream<A> ToStream();
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

        public abstract IEnumerator<A> GetEnumerator();

        public virtual Immutable.List<A> ToList() => FoldLeft(Immutable.List<A>.Builder, (bf, it) => bf.Append(it)).Result();

        public abstract IStream<A> ToStream();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
