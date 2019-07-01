namespace Runes.Collections
{
    public interface IIterableBuilder<A, out CC> : IBuilder<CC> where CC : IIterable<A>
    {
        IIterableBuilder<A, CC> Append(A item);

        new CC Build();

        CC GetEmpty();
    }

    public interface IIterableBuilder<A, out CC, out BB> : IIterableBuilder<A, CC>
        where CC : IIterable<A>
        where BB : IIterableBuilder<A, CC, BB>
    {
        new BB Append(A item);

        IIterableBuilder<A, CC> IIterableBuilder<A, CC>.Append(A item) => Append(item);
    }
}
