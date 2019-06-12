namespace Runes.Text
{
    public interface ITextDecoder<in T>
    {
        string Decode(T data);
    }

    public interface ITextEncoder<out T>
    {
        T Encode(string text);
    }
}
