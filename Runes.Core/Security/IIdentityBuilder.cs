namespace Runes.Security
{
    public interface IIdentityBuilder<T>
    {
        string Build(T obj);
    }
}
