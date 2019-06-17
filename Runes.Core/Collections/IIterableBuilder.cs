namespace Runes.Collections
{
    public interface IIterableBuilder<A> : IBuilder<IIterable<A>>
    {
        IIterableBuilder<A> Append(A item);

        IIterable<A> GetEmpty();
    }

    public interface IIterableBuilder<A, out CC> : IIterableBuilder<A> where CC : IIterable<A>
    {
        new IIterableBuilder<A, CC> Append(A item);

        new CC Build();

        new CC GetEmpty();
    }

    public interface IIterableBuilder<A, out CC, out BB> : IIterableBuilder<A, CC>
        where CC : IIterable<A>
        where BB : IIterableBuilder<A, CC, BB>
    {
        new BB Append(A item);
    }
}
