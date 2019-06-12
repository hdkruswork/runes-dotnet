using Runes.Collections;
using System;

using static Runes.Predef;

namespace Runes.Model
{
    public sealed class PropertyModel<A> : IObservable<A>
    {
        private A value;

        private List<Action<A>> subscribers = EmptyList<Action<A>>();

        public A Value
        {
            get => value;
            set
            {
                if (!Equals(this.value, value))
                {
                    this.value = value;
                    subscribers.Foreach(f => f(value));
                }
            }
        }

        public PropertyModel<A> WithValue(A value)
        {
            Value = value;
            return this;
        }

        public IObservable<A> Subscribe(Action<A> onUpdate)
        {
            subscribers = subscribers.Prepend(onUpdate);
            return this;
        }

        public IObservable<A> UnsubscribeAll()
        {
            subscribers = EmptyList<Action<A>>();
            return this;
        }

        internal PropertyModel() { }
    }

    public static class PropertyModels
    {
        public static PropertyModel<A> PropertyModel<A>() => new PropertyModel<A>();

        public static PropertyModel<A> PropertyModel<A>(A value) => new PropertyModel<A>().WithValue(value);
    }
}
