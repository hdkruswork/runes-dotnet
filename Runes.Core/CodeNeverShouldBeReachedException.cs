using System;

namespace Runes
{
    public sealed class CodeNeverShouldBeReachedException : Exception
    {
        public CodeNeverShouldBeReachedException() : base("This code should never be reached")
        {
        }
    }
}
