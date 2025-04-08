using System.Collections.Generic;

namespace KingdomRenderer.Shared.ArchieV1.Extensions
{
    public static class StringExtension
    {
        public static string Replace(this string str, IEnumerable<string> values, string replacement)
        {
            foreach (var value in values)
            {
                str = value.Replace(value, replacement);
            }

            return str;
        }
    }
}