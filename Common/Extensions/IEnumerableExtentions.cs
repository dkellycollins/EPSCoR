using System.Collections.Generic;
using System.Text;

namespace EPSCoR.Common.Extensions
{
    public static class IEnumerableExtentions
    {
        /// <summary>
        /// Concats each string in the collection into a single string placing a comma in between each value.
        /// </summary>
        /// <param name="source"></param>
        /// <returns>String representation of the collection.</returns>
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

        /// <summary>
        /// Concats each string in the collection into a single string placing a comma in between each value.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="format">A composite format string.</param>
        /// <returns>String representation of the collection.</returns>
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
