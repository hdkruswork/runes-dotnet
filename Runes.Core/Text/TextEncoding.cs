namespace Runes.Text
{
    public interface ITextDecoder<T>
    {
        string Decode(T data);
    }

    public interface ITextEncoder<T>
    {
        T Encode(string text);
    }
}
