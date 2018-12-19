using Runes.Text;

namespace Runes.Security.Cryptography
{
    public class SHA256IdBuilder : IIdentityBuilder<byte[]>
    {
        public static readonly SHA256IdBuilder Object = new SHA256IdBuilder();

        private SHA256IdBuilder() { }

        public string Build(byte[] data) => HexDecoder
            .Object
            .Decode(Sha256Algorithm.Object.Compute(data));
    }
}
