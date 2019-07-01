using System;
using System.Collections;
using System.Collections.Generic;
using Runes.Math;

namespace Runes.Collections
{
    public interface IIterable : ICollection, IEnumerable
    {
        Option<object> HeadOption { get; }
        IIterable Tail { get; }

        IIterable<B> As<B>();
        IIterable<B> Collect<B>(Func<object, Option<B>> f);
        bool Correspond<B>(IIterable<B> other, Func<object, B, bool> f);
        IIterable Drops(Int count);
        IIterable DropsWhile(Func<object, bool> p);
        IIterable DropsWhileNot(Func<object, bool> p);
        IIterable DropsWhile(Func<object, bool> p, out Int dropped);
        IIterable DropsWhileNot(Func<object, bool> p, out Int dropped);
        bool Exists(Func<object, bool> p);
        bool ExistsNot(Func<object, bool> p);
        new IIterable Filter(Func<object, bool> p);
        new IIterable FilterNot(Func<object, bool> p);
        IIterable<B> FlatMap<B>(Func<object, IIterable<B>> f);
        bool ForAll(Func<object, bool> p);
        That FoldLeft<That>(That initialValue, Func<That, object, That> f);
        That FoldLeftWhile<That>(That initialValue, Func<That, object, bool> p, Func<That, object, That> f);
        Unit Foreach(Action<object> action);
        Unit ForeachWhile(Func<object, bool> p, Action<object> action);
        IIterable<B> Map<B>(Func<object, B> f);
        (IIterable, IIterable) Partition(Func<object, bool> p);
        IIterable Take(Int count);
        IIterable TakeWhile(Func<object, bool> p);
        IIterable TakeWhileNot(Func<object, bool> p);
        Array<object> ToArray();
        List<object> ToList();
        object[] ToMutableArray();
        Set<object> ToSet();
        Stream<object> ToStream();
        (IIterable<X>, IIterable<Y>) Unzip<X, Y>(Func<object, (X, Y)> f);
        IIterable<(object, object)> Zip(IIterable other);

        // explicit members

        ICollection ICollection.Filter(Func<object, bool> p) => Filter(p);
        ICollection ICollection.FilterNot(Func<object, bool> p) => FilterNot(p);
    }

    public interface IIterable<A> : IIterable, ICollection<A>, IEnumerable<A>
    {
        new Option<A> HeadOption { get; }
        new IIterable<A> Tail { get; }

        IIterable<B> Collect<B>(Func<A, Option<B>> f);
        bool Correspond<B>(IIterable<B> other, Func<A, B, bool> f);
        new IIterable<A> Drops(Int count);
        IIterable<A> DropsWhile(Func<A, bool> p);
        IIterable<A> DropsWhileNot(Func<A, bool> p);
        IIterable<A> DropsWhile(Func<A, bool> p, out Int dropped);
        IIterable<A> DropsWhileNot(Func<A, bool> p, out Int dropped);
        bool Exists(Func<A, bool> p);
        bool ExistsNot(Func<A, bool> p);
        new IIterable<A> Filter(Func<A, bool> p);
        new IIterable<A> FilterNot(Func<A, bool> p);
        IIterable<B> FlatMap<B>(Func<A, IIterable<B>> f);
        bool ForAll(Func<A, bool> p);
        That FoldLeft<That>(That initialValue, Func<That, A, That> f);
        That FoldLeftWhile<That>(That initialValue, Func<That, A, bool> p, Func<That, A, That> f);
        Unit Foreach(Action<A> action);
        Unit ForeachWhile(Func<A, bool> p, Action<A> action);
        IIterable<B> Map<B>(Func<A, B> f);
        (IIterable<A>, IIterable<A>) Partition(Func<A, bool> p);
        new IIterable<A> Take(Int count);
        IIterable<A> TakeWhile(Func<A, bool> p);
        IIterable<A> TakeWhileNot(Func<A, bool> p);
        That To<That>(IFactory<A, That> factory) where That : IIterable<A>;
        new Array<A> ToArray();
        new List<A> ToList();
        new A[] ToMutableArray();
        new Set<A> ToSet();
        new Stream<A> ToStream();
        (IIterable<X>, IIterable<Y>) Unzip<X, Y>(Func<A, (X, Y)> f);
        IIterable<(A, B)> Zip<B>(IIterable<B> other);

        // explicit definitions

        Option<object> IIterable.HeadOption => HeadOption.Map(a => (object)a);
        IIterable IIterable.Tail => Tail;

        IIterable<B> IIterable.Collect<B>(Func<object, Option<B>> f) => Collect((A a) => f(a));
        bool IIterable.Correspond<B>(IIterable<B> other, Func<object, B, bool> f) => Correspond(other, (A a, B b) => f(a, b));
        IIterable IIterable.Drops(Int count) => Drops(count);
        IIterable IIterable.DropsWhile(Func<object, bool> p) => DropsWhile((A a) => p(a));
        IIterable IIterable.DropsWhileNot(Func<object, bool> p) => DropsWhileNot((A a) => p(a));
        IIterable IIterable.DropsWhile(Func<object, bool> p, out Int dropped) => DropsWhile((A a) => p(a), out dropped);
        IIterable IIterable.DropsWhileNot(Func<object, bool> p, out Int dropped) => DropsWhileNot((A a) => p(a), out dropped);
        bool IIterable.Exists(Func<object, bool> p) => Exists((A a) => p(a));
        bool IIterable.ExistsNot(Func<object, bool> p) => ExistsNot((A a) => p(a));
        IIterable IIterable.Filter(Func<object, bool> p) => Filter((A a) => p(a));
        IIterable IIterable.FilterNot(Func<object, bool> p) => FilterNot((A a) => p(a));
        IIterable<B> IIterable.FlatMap<B>(Func<object, IIterable<B>> f) => FlatMap((A a) => f(a));
        bool IIterable.ForAll(Func<object, bool> p) => ForAll((A a) => p(a));
        That IIterable.FoldLeft<That>(That initialValue, Func<That, object, That> f) =>
            FoldLeft(initialValue, (That agg, A a) => f(agg, a));
        That IIterable.FoldLeftWhile<That>(That initialValue, Func<That, object, bool> p, Func<That, object, That> f) =>
            FoldLeftWhile(initialValue, (That agg, A a) => p(agg, a), (That agg, A a) => f(agg, a));
        Unit IIterable.Foreach(Action<object> action) => Foreach((A a) => action(a));
        Unit IIterable.ForeachWhile(Func<object, bool> p, Action<object> action) => ForeachWhile((A a) => p(a), (A a) => action(a));
        IIterable<B> IIterable.Map<B>(Func<object, B> f) => Map((A a) => f(a));
        (IIterable, IIterable) IIterable.Partition(Func<object, bool> p) => Partition((A a) => p(a));
        IIterable IIterable.Take(Int count) => Take(count);
        IIterable IIterable.TakeWhile(Func<object, bool> p) => TakeWhile((A a) => p(a));
        IIterable IIterable.TakeWhileNot(Func<object, bool> p) => TakeWhileNot((A a) => p(a));
        Array<object> IIterable.ToArray() => As<object>().ToArray();
        List<object> IIterable.ToList() => As<object>().ToList();
        object[] IIterable.ToMutableArray() => As<object>().ToMutableArray();
        Set<object> IIterable.ToSet() => As<object>().ToSet();
        Stream<object> IIterable.ToStream() => As<object>().ToStream();
        (IIterable<X>, IIterable<Y>) IIterable.Unzip<X, Y>(Func<object, (X, Y)> f) => Unzip((A a) => f(a));
        IIterable<(object, object)> IIterable.Zip(IIterable other) => As<object>().Zip(other.As<object>());

        ICollection<A> ICollection<A>.Filter(Func<A, bool> p) => Filter(p);
        ICollection<A> ICollection<A>.FilterNot(Func<A, bool> p) => FilterNot(p);

        ICollection ICollection.Filter(Func<object, bool> p) => Filter(p);
        ICollection ICollection.FilterNot(Func<object, bool> p) => FilterNot(p);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    public interface IIterable<A, CC> :IIterable<A>, ICollection<A, CC> where CC : IIterable<A, CC>
    {
        new CC Tail { get; }

        new CC Drops(Int count);
        new CC DropsWhile(Func<A, bool> p);
        new CC DropsWhileNot(Func<A, bool> p);
        new CC DropsWhile(Func<A, bool> p, out Int dropped);
        new CC DropsWhileNot(Func<A, bool> p, out Int dropped);
        new CC Filter(Func<A, bool> p);
        new CC FilterNot(Func<A, bool> p);
        new (CC, CC) Partition(Func<A, bool> p);
        new CC Take(Int count);
        new CC TakeWhile(Func<A, bool> p);
        new CC TakeWhileNot(Func<A, bool> p);

        // explicit definitions

        IIterable<A> IIterable<A>.Tail => Tail;

        IIterable<A> IIterable<A>.Drops(Int count) => Drops(count);
        IIterable<A> IIterable<A>.DropsWhile(Func<A, bool> p) => DropsWhile(p);
        IIterable<A> IIterable<A>.DropsWhileNot(Func<A, bool> p) => DropsWhileNot(p);
        IIterable<A> IIterable<A>.DropsWhile(Func<A, bool> p, out Int dropped) => DropsWhile(p, out dropped);
        IIterable<A> IIterable<A>.DropsWhileNot(Func<A, bool> p, out Int dropped) => DropsWhileNot(p, out dropped);
        IIterable<A> IIterable<A>.Filter(Func<A, bool> p) => Filter(p);
        IIterable<A> IIterable<A>.FilterNot(Func<A, bool> p) => FilterNot(p);
        (IIterable<A>, IIterable<A>) IIterable<A>.Partition(Func<A, bool> p) => Partition(p);
        IIterable<A> IIterable<A>.Take(Int count) => Take(count);
        IIterable<A> IIterable<A>.TakeWhile(Func<A, bool> p) => TakeWhile(p);
        IIterable<A> IIterable<A>.TakeWhileNot(Func<A, bool> p) => TakeWhileNot(p);

        ICollection<A> ICollection<A>.Filter(Func<A, bool> p) => Filter(p);
        ICollection<A> ICollection<A>.FilterNot(Func<A, bool> p) => FilterNot(p);

        ICollection ICollection.Filter(Func<object, bool> p) => Filter((A a) => p(a));
        ICollection ICollection.FilterNot(Func<object, bool> p) => FilterNot((A a) => p(a));
    }
}
