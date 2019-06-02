using System;

namespace Runes
{
    public interface IObservable
    {
        IObservable Subscribe(Action onUpdate);
        IObservable UnsubscribeAll();
    }

    public interface IObservable<A>
    {
        IObservable<A> Subscribe(Action<A> onUpdate);
        IObservable<A> UnsubscribeAll();
    }
}
