using System.Text;

using static Runes.LazyExtensions;

namespace Runes.Security.Cryptography
{
    public interface IHashAlgorithm
    {
        int HashSize { get; }
        byte[] EmptyHash { get; }

        byte[] Compute(byte[] bytes);
        byte[] Compute(string text, Encoding encoding);
    }

    public abstract class HashAlgorithmBase : IHashAlgorithm
    {
        public int HashSize => EmptyHash.Length;

        public byte[] EmptyHash => emptyHash.Get();

        public byte[] Compute(string text, Encoding encoding) => Compute(encoding.GetBytes(text));

        public byte[] Compute(byte[] bytes) => hashAlgorithm.ComputeHash(bytes);

        protected HashAlgorithmBase(System.Security.Cryptography.HashAlgorithm hashAlgorithm)
        {
            this.hashAlgorithm = hashAlgorithm;
            emptyHash = Lazy(() => Compute(new byte[0]));
        }

        private readonly Lazy<byte[]> emptyHash;
        private readonly System.Security.Cryptography.HashAlgorithm hashAlgorithm;
    }

    public sealed class Sha256Algorithm : HashAlgorithmBase
    {
        public static readonly HashAlgorithmBase Object = new Sha256Algorithm();

        private Sha256Algorithm(): base(System.Security.Cryptography.SHA256.Create()) { }
    }
}
