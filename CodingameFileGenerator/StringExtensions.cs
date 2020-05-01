using System.Collections.Generic;

namespace CodingameFileGenerator
{
    public static class StringExtensions
    {
        public static int IndexOfAny(this string str, IEnumerable<string> words)
        {
            int smallestIndex = int.MaxValue;

            foreach (string word in words)
            {
                if (str.Contains(word))
                {
                    int index = str.IndexOf(word);
                    if (index < smallestIndex)
                    {
                        smallestIndex = index;
                    }
                }
            }

            return (smallestIndex < int.MaxValue) ? smallestIndex : -1;
        }
    }
}
