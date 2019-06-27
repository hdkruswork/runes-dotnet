using Runes.Math;
using System;
using System.Collections;
using System.Collections.Generic;
using static Runes.Predef;

namespace Runes.Collections
{
    public abstract class Set<A> : ISet<A, Set<A>>
    {
        public static readonly IterableSet Empty = new EmptySet();
        public static readonly Set<A> Universe = new UniverseSet();

        public static Set<A> Create(Func<A, bool> containsDefinition) => new LogicSet(containsDefinition);

        public static Set<A> Create(IIterable<A> iterable) => IterableSet.Factory.From(iterable);

        public abstract bool IsEmpty { get; }
        public bool NonEmpty => ! IsEmpty;

        public abstract bool Contains(A item);

        public virtual Set<A> Difference(Set<A> other) => CollectionHelper.Difference(this, other);

        public virtual Set<A> Filter(Func<A, bool> p) => Filter(p, true);

        public virtual Set<A> FilterNot(Func<A, bool> p) => Filter(p, false);

        public virtual Set<A> Intersection(Set<A> other) => CollectionHelper.Intersection(this, other);

        public virtual Set<A> Union(Set<A> other) => CollectionHelper.Union(this, other);

        // protected members

        protected virtual Set<A> Filter(Func<A, bool> p, bool isAffirmative) =>
            Create(a => p(a) == isAffirmative);

        // private members

        ISet<A> ISet<A>.Difference(ISet<A> other) => Difference(Create(other.Contains));

        ICollection<A> ICollection<A>.Filter(Func<A, bool> p) => Filter(p);

        ICollection<A> ICollection<A>.FilterNot(Func<A, bool> p) => FilterNot(p);

        ISet<A> ISet<A>.Intersection(ISet<A> other) => Intersection(Create(other.Contains));

        ISet<A> ISet<A>.Union(ISet<A> other) => Union(Create(other.Contains));

        #region Set Sub Types

        private sealed class LogicSet : Set<A>
        {
            private readonly Func<A, bool> containsDefinition;

            public LogicSet(Func<A, bool> containsDefinition)
            {
                this.containsDefinition = containsDefinition;
            }

            public override bool IsEmpty => false;

            public override bool Contains(A item) => containsDefinition(item);
        }

        public class IterableSet : Set<A>, IIterable<A, IterableSet>
        {
            public static readonly IFactory<A, IterableSet> Factory = new IterableSetFactory();

            public static IterableSet Create(A head) => new IterableSet(head, Empty);

            public static IterableSet Create(A head, IterableSet tail) => new IterableSet(head, tail);

            public override bool IsEmpty => HeadOption.IsEmpty;

            public virtual Option<A> HeadOption { get; }

            public virtual IterableSet Tail { get; }

            public Set<B>.IterableSet As<B>() => CollectionHelper.As(this, GetFactory<B>());

            public Set<B>.IterableSet Collect<B>(Func<A, Option<B>> f) =>
                CollectionHelper.Collect(this, f, GetFactory<B>());

            public override bool Contains(A item) => CollectionHelper.Contains(this, item);

            public bool Correspond<B>(IIterable<B> other, Func<A, B, bool> f) => CollectionHelper.Correspond(this, other, f);

            public IterableSet Drops(Int count) => CollectionHelper.Drops<A, IterableSet>(this, count);

            public IterableSet DropsWhile(Func<A, bool> p) => CollectionHelper.DropsWhile(this, p, out _, true);

            public IterableSet DropsWhileNot(Func<A, bool> p) => CollectionHelper.DropsWhile(this, p, out _, false);

            public IterableSet DropsWhile(Func<A, bool> p, out Int dropped) => CollectionHelper.DropsWhile(this, p, out dropped, true);

            public IterableSet DropsWhileNot(Func<A, bool> p, out Int dropped) => CollectionHelper.DropsWhile(this, p, out dropped, false);

            public bool Exists(Func<A, bool> p) => CollectionHelper.Exists(this, p, true);

            public bool ExistsNot(Func<A, bool> p) => CollectionHelper.Exists(this, p, false);

            public Set<B>.IterableSet FlatMap<B>(Func<A, IIterable<B>> f) => CollectionHelper.FlatMap(this, f, GetFactory<B>());

            public That FoldLeft<That>(That initialValue, Func<That, A, That> f) =>
                CollectionHelper.FoldLeft(this, initialValue, f);

            public That FoldLeftWhile<That>(That initialValue, Func<That, A, bool> p, Func<That, A, That> f) =>
                CollectionHelper.FoldLeftWhile(this, initialValue, p, f);

            public bool ForAll(Func<A, bool> p) => CollectionHelper.ForAll(this, p);

            public Unit Foreach(Action<A> action) => CollectionHelper.Foreach(this, action);

            public Unit ForeachWhile(Func<A, bool> p, Action<A> action) => CollectionHelper.ForeachWhile(this, p, action);

            public IEnumerator<A> GetEnumerator() => CollectionHelper.GetEnumerator(this);

            public Set<B>.IterableSet Map<B>(Func<A, B> f) => CollectionHelper.Map(this, f, GetFactory<B>());

            public (IterableSet, IterableSet) Partition(Func<A, bool> p) => CollectionHelper.Partition(this, p, GetFactory());

            public IterableSet Take(Int count) => CollectionHelper.Take(this, count, GetFactory());

            public IterableSet TakeWhile(Func<A, bool> p) => CollectionHelper.TakeWhile(this, p, true, GetFactory());

            public IterableSet TakeWhileNot(Func<A, bool> p) => CollectionHelper.TakeWhile(this, p, false, GetFactory());

            public CC To<CC>(IFactory<A, CC> factory) where CC : IIterable<A> => CollectionHelper.To(this, factory);

            public Array<A> ToArray() => CollectionHelper.ToArray(this);

            public A[] ToMutableArray() => CollectionHelper.ToMutableArray(this);

            public List<A> ToList() => CollectionHelper.ToList(this);

            public Set<A> ToSet() => this;

            public Stream<A> ToStream() => CollectionHelper.ToStream(this);

            public (Set<X>.IterableSet, Set<Y>.IterableSet) Unzip<X, Y>(Func<A, (X, Y)> f) =>
                CollectionHelper.Unzip(this, f, GetFactory<X>(), GetFactory<Y>());

            public Set<(A, B)>.IterableSet Zip<B>(IIterable<B> other) =>
                CollectionHelper.Zip(this, other, GetFactory<(A, B)>());

            // protected members

            protected override Set<A> Filter(Func<A, bool> p, bool isAffirmative) =>
                CollectionHelper.Filter(this, p, isAffirmative, GetFactory());

            // private members

            protected private IterableSet() { }

            protected private IterableSet(A head, IterableSet tail)
            {
                HeadOption = Some(head);
                Tail = tail;
            }

            private IFactory<A, IterableSet> GetFactory() => Factory;

            private IFactory<B, Set<B>.IterableSet> GetFactory<B>() => Set<B>.IterableSet.Factory;

            IterableSet IIterable<A, IterableSet>.Tail => Tail;

            Option<A> IIterable<A>.HeadOption => HeadOption;

            IIterable<A> IIterable<A>.Tail => Tail;

            IterableSet IIterable<A, IterableSet>.Drops(Int count) => Drops(count);

            IterableSet IIterable<A, IterableSet>.DropsWhile(Func<A, bool> p) => DropsWhile(p);

            IterableSet IIterable<A, IterableSet>.DropsWhileNot(Func<A, bool> p) => DropsWhileNot(p);

            IterableSet IIterable<A, IterableSet>.DropsWhile(Func<A, bool> p, out Int dropped) => DropsWhile(p, out dropped);

            IterableSet IIterable<A, IterableSet>.DropsWhileNot(Func<A, bool> p, out Int dropped) => DropsWhileNot(p, out dropped);

            (IterableSet, IterableSet) IIterable<A, IterableSet>.Partition(Func<A, bool> p) => Partition(p);

            IterableSet IIterable<A, IterableSet>.Take(Int count) => Take(count);

            IterableSet IIterable<A, IterableSet>.TakeWhile(Func<A, bool> p) => TakeWhile(p);

            IterableSet IIterable<A, IterableSet>.TakeWhileNot(Func<A, bool> p) => TakeWhileNot(p);

            IIterable<B> IIterable<A>.As<B>() => As<B>();

            IIterable<B> IIterable<A>.Collect<B>(Func<A, Option<B>> f) => Collect(f);

            IIterable<A> IIterable<A>.Drops(Int count) => Drops(count);

            IIterable<A> IIterable<A>.DropsWhile(Func<A, bool> p) => DropsWhile(p);

            IIterable<A> IIterable<A>.DropsWhileNot(Func<A, bool> p) => DropsWhileNot(p);

            IIterable<A> IIterable<A>.DropsWhile(Func<A, bool> p, out Int dropped) => DropsWhile(p, out dropped);

            IIterable<A> IIterable<A>.DropsWhileNot(Func<A, bool> p, out Int dropped) => DropsWhileNot(p, out dropped);

            bool IIterable<A>.Exists(Func<A, bool> p) => Exists(p);

            bool IIterable<A>.ExistsNot(Func<A, bool> p) => ExistsNot(p);

            IIterable<A> IIterable<A>.Filter(Func<A, bool> p) => (IterableSet)Filter(p);

            IIterable<A> IIterable<A>.FilterNot(Func<A, bool> p) => (IterableSet)FilterNot(p);

            IIterable<B> IIterable<A>.FlatMap<B>(Func<A, IIterable<B>> f) => FlatMap(f);

            bool IIterable<A>.ForAll(Func<A, bool> p) => ForAll(p);

            That IIterable<A>.FoldLeft<That>(That initialValue, Func<That, A, That> f) =>
                FoldLeft(initialValue, f);

            That IIterable<A>.FoldLeftWhile<That>(That initialValue, Func<That, A, bool> p, Func<That, A, That> f) =>
                FoldLeftWhile(initialValue, p, f);

            Unit IIterable<A>.Foreach(Action<A> action) => Foreach(action);

            Unit IIterable<A>.ForeachWhile(Func<A, bool> p, Action<A> action) => ForeachWhile(p, action);

            IIterable<B> IIterable<A>.Map<B>(Func<A, B> f) => Map(f);

            (IIterable<A>, IIterable<A>) IIterable<A>.Partition(Func<A, bool> p) => Partition(p);

            IIterable<A> IIterable<A>.Take(Int count) => Take(count);

            IIterable<A> IIterable<A>.TakeWhile(Func<A, bool> p) => TakeWhile(p);

            IIterable<A> IIterable<A>.TakeWhileNot(Func<A, bool> p) => TakeWhileNot(p);

            (IIterable<X>, IIterable<Y>) IIterable<A>.Unzip<X, Y>(Func<A, (X, Y)> f) => Unzip(f);

            IIterable<(A, B)> IIterable<A>.Zip<B>(IIterable<B> other) => Zip(other);

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

            IterableSet ICollection<A, IterableSet>.Filter(Func<A, bool> p) => (IterableSet)Filter(p);

            IterableSet ICollection<A, IterableSet>.FilterNot(Func<A, bool> p) => (IterableSet)FilterNot(p);

            private sealed class IterableSetBuilder : IIterableBuilder<A, IterableSet, IterableSetBuilder>
            {
                private readonly HashSet<A> hashset = new HashSet<A>();
                private List<A> list = List<A>.Empty;

                public IterableSetBuilder Append(A item)
                {
                    if (hashset.Add(item))
                    {
                        list = list.Prepend(item);
                    }
                    return this;
                }

                public IterableSet Build() => list.FoldLeft(Empty, (set, it) => Create(it, set));

                public IterableSet GetEmpty() => Empty;

                IIterableBuilder<A, IterableSet> IIterableBuilder<A, IterableSet>.Append(A item) => Append(item);

                IIterableBuilder<A> IIterableBuilder<A>.Append(A item) => Append(item);

                IIterable<A> IBuilder<IIterable<A>>.Build() => Build();

                IIterable<A> IIterableBuilder<A>.GetEmpty() => GetEmpty();
            }

            private sealed class IterableSetFactory : IFactory<A, IterableSet>
            {
                public IIterableBuilder<A, IterableSet> CreateBuilderFrom(params IEnumerable<A>[] iterables)
                {
                    var builder = NewBuilder();
                    foreach (var iterable in iterables)
                    {
                        foreach (var item in iterable)
                        {
                            builder.Append(item);
                        }
                    }

                    return builder;
                }

                public IterableSet From(IEnumerable<A> e) => CreateBuilderFrom(e).Build();

                public IIterableBuilder<A, IterableSet> NewBuilder() => new IterableSetBuilder();
            }
        }

        private sealed class UniverseSet : Set<A>
        {
            public override bool IsEmpty => false;

            public override bool Contains(A item) => true;

            public override Set<A> Difference(Set<A> other) => Create(a => !other.Contains(a));

            public override Set<A> Intersection(Set<A> other) => other;

            public override Set<A> Union(Set<A> other) => this;
        }

        private sealed class EmptySet : IterableSet
        {
            public override bool IsEmpty => true;

            public override Option<A> HeadOption => None<A>();

            public override IterableSet Tail => this;

            public override bool Contains(A item) => false;

            public override Set<A> Difference(Set<A> other) => this;

            public override Set<A> Intersection(Set<A> other) => this;

            public override Set<A> Union(Set<A> other) => other;

            // Protected

            protected override Set<A> Filter(Func<A, bool> p, bool isAffirmative) => this;
        }

        #endregion
    }
}
