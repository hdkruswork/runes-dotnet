namespace Runes.Collections.Immutable
{
    public interface ISeqLike<A, Repr> : ISeqViewLike<A, Repr> where Repr : ISeqLike<A, Repr>
    {
        Repr Append(A rear);
        Repr Append(Repr other);
        Repr Prepend(A e);
        Repr Prepend(Repr e);
    }

    public abstract class SeqLike<A, Repr> : SeqViewLike<A, Repr>, ISeqLike<A, Repr> where Repr : SeqLike<A, Repr>
    {
        public virtual Repr Append(A rear)
        {
            var builder = NewBuilder();
            foreach (var item in this)
            {
                builder.Append(item);
            }
            builder.Append(rear);
            return builder.Build();
        }

        public virtual Repr Append(Repr other)
        {
            var builder = NewBuilder();
            foreach (var item in this)
            {
                builder.Append(item);
            }
            foreach (var item in other)
            {
                builder.Append(item);
            }
            return builder.Build();
        }

        public abstract Repr Prepend(A e);

        public virtual Repr Prepend(Repr e)
        {
            var builder = NewBuilder();
            foreach (var item in e)
            {
                builder.Append(item);
            }
            foreach (var item in this)
            {
                builder.Append(item);
            }
            return builder.Build();
        }

        public override Repr Reverse() => FoldLeft(NewBuilder().Build(), (acc, it) => acc.Prepend(it));
    }
}
