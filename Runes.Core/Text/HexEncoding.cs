using Runes.Collections.Immutable;
using System.Text;

using static Runes.Collections.Immutable.Streams;

namespace Runes.Text
{
    public static class Hex
    {
        public static HexDecoder HexDecoder() => Text.HexDecoder.Object;

        public static HexEncoder HexEncoder() => Text.HexEncoder.Object;

        public static byte[] FromHex(this string hex) => HexEncoder().Encode(hex);

        public static string ToHex(this byte[] data) => HexDecoder().Decode(data);
    }

    public sealed class HexDecoder : ITextDecoder<byte[]>
    {
        public static HexDecoder Object => new HexDecoder(TextCaseConstraint.LOWERCASED);

        private readonly TextCaseConstraint constraint;

        private HexDecoder(TextCaseConstraint constraint)
        {
            this.constraint = constraint;
        }

        public string Decode(byte[] data)
        {
            var str = Stream(data)
                .Map(b =>
                {
                    var hiChar = HexAlphabet.Chars[b >> 0x04];
                    var lowChar = HexAlphabet.Chars[b & 0x0f];

                    return $"{hiChar}{lowChar}";
                })
                .FoldLeft(new StringBuilder(), (sb, b) => sb.Append(b))
                .ToString();

            return constraint == TextCaseConstraint.UPPERCASED
                ? str.ToUpper()
                : str;
        }

        public HexDecoder WithCase(TextCaseConstraint constraint) =>
            constraint == Object.constraint ? Object : new HexDecoder(constraint);
    }

    public sealed class HexEncoder : ITextEncoder<byte[]>
    {
        public static HexEncoder Object => new HexEncoder();

        private HexEncoder() { }

        public byte[] Encode(string text) =>
            text.ToLower()
                .ToStream()
                .Sliding(2, 2)
                .Collect(pair =>
                {
                    var (hiChar, lowChar) = (pair[0], pair[1]);
                    var hiIndex = HexAlphabet.Chars.IndexOf(hiChar);
                    var lowIndex = HexAlphabet.Chars.IndexOf(lowChar);
                    byte hi = hiIndex >= 0 ? (byte)(hiIndex << 0x04) : byte.MinValue;
                    byte low = lowIndex >= 0 ? (byte)lowIndex : byte.MinValue;
                    return (byte)(hi | low);
                })
                .ToArray();
    }

    public static class HexAlphabet
    {
        public static readonly string Chars = @"0123456789abcdef";
    }
}
