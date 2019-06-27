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
}
