using System.Collections.Generic;
using System.Text;

namespace EPSCoR.Common.Extensions
{
    public static class IEnumerableExtentions
    {
        public static string ToCommaSeparatedString(this IEnumerable<string> source)
        {
            StringBuilder valueBuilder = new StringBuilder();
            foreach (string str in source)
            {
                valueBuilder.Append(str + ", ");
            }
            valueBuilder.Remove(valueBuilder.Length - 2, 2);

            return valueBuilder.ToString();
        }

        public static string ToCommaSeparatedString(this IEnumerable<string> source, string format)
        {
            StringBuilder valueBuilder = new StringBuilder();
            foreach (string str in source)
            {
                valueBuilder.Append(string.Format(format + ", ", str));
            }
            valueBuilder.Remove(valueBuilder.Length - 2, 2);

            return valueBuilder.ToString();
        }
    }
}
