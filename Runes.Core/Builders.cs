using Runes.Collections;
using System.Collections.Generic;

namespace Runes
{
    public interface IBuilder<A, CC>
    {
        IBuilder<A, CC> NewBuilder();
        CC BuildFrom(A value);
        CC Empty();
    }

    public interface IBuilderLike<A, CC, Repr>: IBuilder<A, CC> where Repr: IBuilderLike<A, CC, Repr>
    {
        new Repr NewBuilder();
    }

    public abstract class BuilderLike<A, CC, Repr> : IBuilderLike<A, CC, Repr> where Repr : BuilderLike<A, CC, Repr>
    {
        public abstract CC BuildFrom(A value);
        public abstract Repr NewBuilder();

        public abstract CC Empty();

        IBuilder<A, CC> IBuilder<A, CC>.NewBuilder() => NewBuilder();
    }
}
