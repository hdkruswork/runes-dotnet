using System.Collections.Generic;
using System;

namespace Runes.Text
{
    public sealed class StringBuilder : IBuilder<string>
    {
        private readonly LinkedList<string> strings = new LinkedList<string>();

        public int Length { get; private set; } = 0;

        public int Count {  get; private set; } = 0;

        public StringBuilder Append(object obj) => Add(obj, it => strings.AddLast(it));

        public StringBuilder Prepend(object obj) => Add(obj, it => strings.AddFirst(it));

        public StringBuilder RemoveFirst()
        {
            if (strings.Count > 0)
            {
                var str = strings.First.Value;
                strings.RemoveFirst();
                Length -= str.Length;
                Count -= 1;
            }

            return this;
        }

        public StringBuilder RemoveLast()
        {
            if (strings.Count > 0)
            {
                var str = strings.Last.Value;
                strings.RemoveLast();
                Length -= str.Length;
                Count -= 1;
            }

            return this;
        }

        public unsafe string Build()
        {
            var res = new char[Length];
            fixed(char* chars = res)
            {
                char* curr = chars;
                foreach (var str in strings)
                {
                    var strLength = str.Length;
                    fixed (char* pStr = str)
                    {
                        Buffer.MemoryCopy(pStr, curr, (Length - strLength) * 2, strLength * 2);
                    }
                    curr += strLength;
                }
            }
            
            return new string(res);
        }

        public override string ToString() => Build();

        private StringBuilder Add(object obj, Action<string> addFunc)
        {
            var str = obj?.ToString();
            if (!string.IsNullOrEmpty(str))
            {
                addFunc(str);
                Length += str.Length;
                Count += 1;
            }

            return this;
        }
    }
}
