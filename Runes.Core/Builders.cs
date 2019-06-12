namespace Runes
{
    public interface IBuilder<out A>
    {
        A Build();
    }
}
