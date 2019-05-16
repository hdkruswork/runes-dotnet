using static Runes.Options;

namespace Runes.Collections.Immutable
{
    public interface ICollectionBuilder<A, CC>: IBuilder<A, CC> where CC: ITraversable<A>
    {
        ICollectionBuilder<A, CC> Append(A elem);
        ICollectionBuilder<A, CC> Append(ITraversable<A> trav);
        new ICollectionBuilder<A, CC> NewBuilder();
        CC Result();
    }

    public abstract class CollectionBuilder<A, CC>
        : BuilderLike<A, CC, CollectionBuilder<A, CC>>, ICollectionBuilder<A, CC> where CC: ISeqLike<A, CC>
    {
        public Option<A> RearOption { get; }
        public CollectionBuilder<A, CC> Init { get; }

        public virtual ICollectionBuilder<A, CC> Append(A elem) => NewBuilder(elem, this);
        public virtual ICollectionBuilder<A, CC> Append(ITraversable<A> trav)
        {
            ICollectionBuilder<A, CC> builder = this;
            foreach (var item in trav)
            {
                builder = builder.Append(item);
            }
            return builder;
        }
        public override CC BuildFrom(A value) => NewBuilder().Append(value).Result();
        public virtual CC Result()
        {
            CollectionBuilder<A, CC> curr = this;
            var res = Empty();
            while (curr.RearOption.GetIfPresent(out A rear))
            {
                res = res.Prepend(rear);
                curr = curr.Init;
            }
            return res;
        }

        protected CollectionBuilder(A rear, CollectionBuilder<A, CC> init)
        {
            RearOption = Some(rear);
            Init = init;
        }

        private protected CollectionBuilder()
        {
            RearOption = None<A>();
            Init = this;
        }

        protected abstract CollectionBuilder<A, CC> NewBuilder(A rear, CollectionBuilder<A, CC> init);

        ICollectionBuilder<A, CC> ICollectionBuilder<A, CC>.NewBuilder() => NewBuilder();
    }
}
