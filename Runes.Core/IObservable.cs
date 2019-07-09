using Runes.Collections;
using System;

namespace Runes
{
    public interface IObservable
    {
        IObservable Subscribe(Action onUpdate);
        IObservable UnsubscribeAll();
    }

    public interface IObservable<out A>
    {
        IObservable<A> Subscribe(Action<A> onUpdate);
        IObservable<A> UnsubscribeAll();
    }

    public interface IObservable<A, out OB> : IObservable<A> where OB : IObservable<A, OB>
    {
        new OB Subscribe(Action<A> onUpdate);
        new OB UnsubscribeAll();

        IObservable<A> IObservable<A>.Subscribe(Action<A> onUpdate) => Subscribe(onUpdate);
        IObservable<A> IObservable<A>.UnsubscribeAll() => UnsubscribeAll();
    }

    public abstract class Observable : IObservable
    {
        private List<Action> subscriberList = List<Action>.Empty;

        public IObservable Subscribe(Action onUpdate)
        {
            lock (this)
            {
                subscriberList = subscriberList.Prepend(onUpdate);
            }

            return this;
        }

        public IObservable UnsubscribeAll()
        {
            lock (this)
            {
                subscriberList = List<Action>.Empty;
            }

            return this;
        }

        protected IObservable NotifyAll()
        {
            subscriberList.Foreach(action => action());

            return this;
        }
    }

    public abstract class Observable<A> : IObservable<A>
    {
        private List<Action<A>> subscriberList = List<Action<A>>.Empty;

        public IObservable<A> Subscribe(Action<A> onUpdate)
        {
            lock (this)
            {
                subscriberList = subscriberList.Prepend(onUpdate);
            }

            return this;
        }

        public IObservable<A> UnsubscribeAll()
        {
            lock (this)
            {
                subscriberList = List<Action<A>>.Empty;
            }

            return this;
        }

        protected IObservable<A> NotifyAll(A data)
        {
            subscriberList.Foreach(action => action(data));

            return this;
        }
    }

    public abstract class Observable<A, OB> : IObservable<A, OB> where OB : Observable<A, OB>
    {
        private List<Action<A>> subscriberList = List<Action<A>>.Empty;

        public OB Subscribe(Action<A> onUpdate)
        {
            lock (this)
            {
                subscriberList = subscriberList.Prepend(onUpdate);
            }

            return This;
        }

        public OB UnsubscribeAll()
        {
            lock (this)
            {
                subscriberList = List<Action<A>>.Empty;
            }

            return This;
        }

        protected OB NotifyAll(A data)
        {
            subscriberList.Foreach(action => action(data));

            return This;
        }

        private OB This => (OB)this;
    }
}
