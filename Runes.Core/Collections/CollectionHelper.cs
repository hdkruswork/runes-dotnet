using Runes.Math;
using System;
using System.Collections.Generic;

using static Runes.Predef;

namespace Runes.Collections
{
    public static class CollectionHelper
    {
        public static That As<A, B, That>(IIterable<A> iter, IFactory<B, That> factory) where That : IIterable<B>
        {
            var current = iter;
            var builder = factory.NewBuilder();
            while (current.HeadOption.GetIfPresent(out var head))
            {
                if (head.As<B>(out var b))
                {
                    builder.Append(b);
                }
                current = current.Tail;
            }

            return builder.Build();
        }

        public static bool Contains<A>(IIterable<A> iter, A item) => Exists(iter, it => Equals(it, item), true);

        public static That Collect<A, B, That>(IIterable<A> iter, Func<A, Option<B>> f, IFactory<B, That> factory) where That : IIterable<B>
        {
            var current = iter;
            var builder = factory.NewBuilder();
            while (current.HeadOption.GetIfPresent(out var head))
            {
                var option = f(head);
                option.Foreach(it => builder.Append(it));
                current = current.Tail;
            }

            return builder.Build();
        }

        public static bool Correspond<A, B>(IIterable<A> iter1, IIterable<B> iter2) =>
            Correspond(iter1, iter2, (a, b) => Equals(a, b));

        public static bool Correspond<A, B>(IIterable<A> iter1, IIterable<B> iter2, Func<A, B, bool> f)
        {
            var curr1 = iter1;
            var curr2 = iter2;
            while (curr1.HeadOption.GetIfPresent(out var head1) && curr2.HeadOption.GetIfPresent(out var head2))
            {
                if (!f(head1, head2))
                {
                    return false;
                }

                curr1 = curr1.Tail;
                curr2 = curr2.Tail;
            }

            return true;
        }

        public static Set<A> Difference<A>(Set<A> set1, Set<A> set2)
        {
            if (set1.IsEmpty || set2 == Set<A>.Universe)
            {
                return Set<A>.Empty;
            }

            if (set1 is Set<A>.IterableSet iter)
            {
                return iter.FilterNot(set2.Contains);
            }

            return Set<A>.Create(it => set1.Contains(it) && !set2.Contains(it));
        }

        public static CC Drops<A, CC>(CC iter, Int count) where CC : IIterable<A, CC>
        {
            var current = iter;
            while (iter.NonEmpty && count > 0)
            {
                current = current.Tail;
                count -= 1;
            }

            return current;
        }

        public static CC DropsWhile<A, CC>(CC iter, Func<A, bool> p, out Int dropped, bool isAffirmative) where CC : IIterable<A, CC>
        {
            var res = iter;
            dropped = 0;
            while (res.HeadOption.GetIfPresent(out var head) && p(head) == isAffirmative)
            {
                res = res.Tail;
                dropped += 1;
            }

            return res;
        }

        public static bool Exists<A>(IIterable<A> iter, Func<A, bool> p, bool isAffirmative)
        {
            var current = iter;
            while (current.HeadOption.GetIfPresent(out var head))
            {
                if (p(head) == isAffirmative)
                {
                    return true;
                }

                current = current.Tail;
            }
            return false;
        }

        public static bool Exists<A>(Func<A, bool> p, bool isAffirmative, params IIterable<A>[] iterables)
        {
            foreach (var iterable in iterables)
            {
                if (Exists(iterable, p, isAffirmative))
                {
                    return true;
                }
            }

            return false;
        }

        public static CC Filter<A, CC>(CC iter, Func<A, bool> p, bool isAffirmative, IFactory<A, CC> factory) where CC : IIterable<A, CC> => 
            iter.FoldLeft(factory.NewBuilder(), (builder, item) => p(item) == isAffirmative ? builder.Append(item) : builder)
                .Build();

        public static That FlatMap<A, B, That>(IIterable<A> iter, Func<A, IIterable<B>> f, IFactory<B, That> factory) where That : IIterable<B>
        {
            var current = iter;
            var builder = factory.NewBuilder();
            while (current.HeadOption.GetIfPresent(out var head))
            {
                var iterable = f(head);
                iterable.Foreach(it => builder.Append(it));
                current = current.Tail;
            }

            return builder.Build();
        }

        public static That FoldLeft<A, That>(IIterable<A> iter, That initialValue, Func<That, A, That> f)
        {
            var res = initialValue;
            var curr = iter;
            while (curr.HeadOption.GetIfPresent(out var head))
            {
                res = f(res, head);
                curr = curr.Tail;
            }

            return res;
        }

        public static That FoldLeftWhile<A, That>(IIterable<A> iter, That initialValue, Func<That, A, bool> p, Func<That, A, That> f)
        {
            var res = initialValue;
            var curr = iter;
            while (curr.HeadOption.GetIfPresent(out var head) && p(res, head))
            {
                res = f(res, head);
                curr = curr.Tail;
            }

            return res;
        }

        public static bool ForAll<A>(IIterable<A> iter, Func<A, bool> p)
        {
            var current = iter;
            while (current.HeadOption.GetIfPresent(out var head))
            {
                if (!p(head))
                {
                    return false;
                }

                current = current.Tail;
            }

            return true;
        }

        public static bool ForAll<A>(Func<A, bool> p, params IIterable<A>[] iterables)
        {
            foreach (var iterable in iterables)
            {
                if (!ForAll(iterable, p))
                {
                    return false;
                }
            }

            return true;
        }

        public static Unit Foreach<A>(IIterable<A> iter, Action<A> action) => Unit(() =>
        {
            var curr = iter;
            while (curr.HeadOption.GetIfPresent(out var head))
            {
                action(head);
                curr = curr.Tail;
            }
        });

        public static Unit ForeachWhile<A>(IIterable<A> iter, Func<A, bool> p, Action<A> action) => Unit(() =>
        {
            var curr = iter;
            while (curr.HeadOption.GetIfPresent(out var head) && p(head))
            {
                action(head);
                curr = curr.Tail;
            }
        });

        public static IEnumerator<A> GetEnumerator<A>(IIterable<A> iter)
        {
            var curr = iter;
            while (curr.HeadOption.GetIfPresent(out var head))
            {
                yield return head;
                curr = curr.Tail;
            }
        }

        public static Set<A> Intersection<A>(Set<A> set1, Set<A> set2)
        {
            if (set1.IsEmpty || set2.IsEmpty)
            {
                return Set<A>.Empty;
            }

            if (set1 == Set<A>.Universe)
            {
                return set2;
            }

            if (set2 == Set<A>.Universe)
            {
                return set1;
            }

            if (set1 is Set<A>.IterableSet)
            {
                return set1.Filter(set2.Contains);
            }

            if (set2 is Set<A>.IterableSet)
            {
                return set2.Filter(set1.Contains);
            }

            return Set<A>.Create(it => set1.Contains(it) && set2.Contains(it));
        }

        public static That Map<A, B, That>(IIterable<A> iter, Func<A, B> f, IFactory<B, That> factory) where That : IIterable<B>
        {
            var current = iter;
            var builder = factory.NewBuilder();
            while (current.HeadOption.GetIfPresent(out var head))
            {
                var b = f(head);
                builder.Append(b);
                current = current.Tail;
            }

            return builder.Build();
        }

        public static (CC, CC) Partition<A, CC>(IIterable<A> iter, Func<A, bool> p, IFactory<A, CC> factory) where CC : IIterable<A, CC>
        {
            var leftBuilder = factory.NewBuilder();
            var rightBuilder = factory.NewBuilder();

            foreach (var item in iter)
            {
                if (p(item))
                {
                    leftBuilder.Append(item);
                }
                else
                {
                    rightBuilder.Append(item);
                }
            }

            return (leftBuilder.Build(), rightBuilder.Build());
        }

        public static S Slice<A, C, S>(IIterable<A> iter, IFactory<A, C> subColFactory, IFactory<C, S> factory, int size, int nextStep)
            where C : IIterable<A, C>
            where S : IIterable<C, S>
        {
            var builder = factory.NewBuilder();
            var curr = iter;
            var currStep = nextStep > 0 ? nextStep : 1;
            while (curr.NonEmpty)
            {
                var subCollection = iter.Take(size).To(subColFactory);
                builder.Append(subCollection);
                curr = iter.Drops(currStep);
            }

            return builder.Build();
        }

        public static S Slice<A, S>(IIterable<A> iter, IFactory<A[], S> factory, int size, int nextStep)
            where S : IIterable<A[], S>
        {
            var builder = factory.NewBuilder();
            var curr = iter;
            var currStep = nextStep > 0 ? nextStep : 1;
            while (curr.NonEmpty)
            {
                var subCollection = iter.Take(size).ToMutableArray();
                builder.Append(subCollection);
                curr = iter.Drops(currStep);
            }

            return builder.Build();
        }

        public static CC Take<A, CC>(CC iter, Int count, IFactory<A, CC> factory) where CC : IIterable<A, CC>
        {
            var (colBuilder, _) = iter.FoldLeftWhile(
                (factory.NewBuilder(), count),
                (agg, _) =>
                {
                    var (_, ct) = agg;
                    return ct > 0;
                },
                (agg, it) =>
                {
                    var (builder, ct) = agg;
                    return (builder.Append(it), ct - 1);
                }
            );

            return colBuilder.Build();
        }

        public static CC TakeWhile<A, CC>(CC iter, Func<A, bool> p, bool isAffirmative, IFactory<A, CC> factory) where CC : IIterable<A, CC> =>
            iter.FoldLeftWhile(factory.NewBuilder(), (_, it) => p(it) == isAffirmative, (b, it) => b.Append(it))
                .Build();

        public static CC To<A, CC>(IIterable<A> iter, IFactory<A, CC> factory) where CC : IIterable<A>
        {
            if (iter is CC col)
            {
                return col;
            }

            return factory.From(iter);
        }

        public static Array<A> ToArray<A>(IIterable<A> iter) => To(iter, Array<A>.Factory);

        public static List<A> ToList<A>(IIterable<A> iter) => To(iter, List<A>.Factory);

        public static A[] ToMutableArray<A>(IIterable<A> iter)
        {
            var list = ToList(iter);
            var array = new A[(long)list.Size];
            list.ZipWithIndex().Foreach(pair =>
            {
                var (it, idx) = pair;
                array[(long)idx] = it;
            });

            return array;
        }

        public static Set<A> ToSet<A>(IIterable<A> iter) => To(iter, Set<A>.IterableSet.Factory);

        public static Stream<A> ToStream<A>(IIterable<A> iter) => To(iter, Stream<A>.Factory);

        public static Set<A> Union<A>(Set<A> set1, Set<A> set2)
        {
            if (set1 == Set<A>.Universe || set2 == Set<A>.Universe)
            {
                return Set<A>.Universe;
            }

            if (set1.IsEmpty)
            {
                return set2;
            }

            if (set2.IsEmpty)
            {
                return set1;
            }

            if (set1 is Set<A>.IterableSet iter1 && set2 is Set<A>.IterableSet iter2)
            {
                return Set<A>.IterableSet
                    .Factory
                    .CreateBuilderFrom(iter1, iter2)
                    .Build();
            }

            return Set<A>.Create(it => set1.Contains(it) || set2.Contains(it));
        }

        public static (Left, Right) Unzip<A, X, Y, Left, Right>(
            IIterable<A> iter,
            Func<A, (X, Y)> f,
            IFactory<X, Left> leftFactory,
            IFactory<Y, Right> rightFactory
        ) where Left : IIterable<X> where Right : IIterable<Y>
        {
            var current = iter;
            var xBuilder = leftFactory.NewBuilder();
            var yBuilder = rightFactory.NewBuilder();
            while (current.HeadOption.GetIfPresent(out var head))
            {
                var (x, y) = f(head);
                xBuilder.Append(x);
                yBuilder.Append(y);
                current = current.Tail;
            }

            return (xBuilder.Build(), yBuilder.Build());
        }

        public static That Zip<A, B, That>(
            IIterable<A> iter,
            IIterable<B> other,
            IFactory<(A, B), That> factory
        ) where That : IIterable<(A, B)>
        {
            var currentThis = iter;
            var currentOther = other;
            var builder = factory.NewBuilder();
            while (currentThis.HeadOption.GetIfPresent(out var a) && other.HeadOption.GetIfPresent(out var b))
            {
                builder.Append((a, b));
                currentThis = currentThis.Tail;
                currentOther = currentOther.Tail;
            }

            return builder.Build();
        }
    }
}
