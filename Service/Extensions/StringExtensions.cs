using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;

#pragma warning disable 1584,1711,1572,1581,1580

namespace Service.Extensions
{
    public static class StringExtensions
    {
        #region Public Methods

        /// <summary>
        ///     Deserializes the given JSON string into an object of type T
        /// </summary>
        /// <param name="jsonString">A JSON string holding the object to deserialize.</param>
        /// <typeparam name="T">The type of the object.</typeparam>
        /// <returns>The deserialized object.</returns>
        /// <exception cref="ArgumentNullException">jsonstring parameter is null. </exception>
        /// <exception cref="JsonException">Thrown if there was an exception deserializing the string.</exception>
        public static async Task<T> DeserializeObjectAsync<T>(this string jsonString)
        {
            if (jsonString.IsNullOrEmpty())
            {
                return default(T);
            }

            return await Task.Run(() =>
                                  {
                                      using (var stringReader = new StringReader(jsonString))
                                      {
                                          return (T)new JsonSerializer().Deserialize(stringReader, typeof(T));
                                      }
                                  });
        }

        /// <summary>
        ///     This will return the parameter if it is not null.  If it is null an empty string is returned.
        /// </summary>
        /// <param name="string">
        ///     the string to check
        /// </param>
        /// <returns> the string parameter if not null or an empty string if it is null </returns>
        public static string EmptyIfNull(this string @string) => @string ?? string.Empty;

        /// <summary>
        ///     Check to see if the two strings are equal.
        /// </summary>
        /// <param name="one"> first string </param>
        /// <param name="two">
        ///     second string
        /// </param>
        /// <returns><see cref="true" /> if both are null or equal.</returns>
        public static bool Equals(string one, string two) => one == null && two == null || one != null && two != null && one.Equals(two);

        public static string FirstLetterToUpper(this string str)
        {
            if (str == null)
            {
                return null;
            }

            if (str.Length > 1)
            {
                return char.ToUpper(str[0]) + str.Substring(1);
            }

            return str.ToUpper();
        }

        /// <exception cref="ArgumentNullException"><paramref name="s" /> is null. </exception>
        /// <exception cref="EncoderFallbackException">
        ///     A fallback occurred (see Character Encoding in the .NET Framework for
        ///     complete explanation)-and-<see cref="P:System.Text.Encoding.EncoderFallback" /> is set to
        ///     <see cref="T:System.Text.EncoderExceptionFallback" />.
        /// </exception>
        public static async Task<byte[]> GetBytesAsync(this string s)
        {
            return await Task.Run(() => new UTF8Encoding().GetBytes(s));
        }

        /// <summary>Creates a random string of alpha-numeric characters of the given length.</summary>
        /// <remarks>If the given length is zero or less, the length of the return value is zero.</remarks>
        /// <exception cref="IndexOutOfRangeException">
        ///     <paramref name="index" /> is greater than or equal to the length of this
        ///     object or less than zero.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     <paramref name="minValue" /> is greater than <paramref name="maxValue" />
        ///     .
        /// </exception>
        /// <exception cref="ArgumentNullException">The <paramref name="function" /> parameter is null. </exception>
        public static async Task<string> GetRandomStringAsync(int length)
        {
            return await Task.Run(() =>
                                  {
                                      const string input = "abcdefghijklmnopqrstuvwxyz0123456789";
                                      var builder = new StringBuilder();
                                      for (var i = 0; i < length; i++)
                                      {
                                          var ch = input[Random.Next(0, input.Length)];
                                          builder.Append(ch);
                                      }

                                      return builder.ToString();
                                  });
        }

        /// <exception cref="OverflowException">
        ///     <paramref name="value" /> is greater than <see cref="F:System.Decimal.MaxValue" />
        ///     or less than <see cref="F:System.Decimal.MinValue" />.-or- <paramref name="value" /> is
        ///     <see cref="F:System.Double.NaN" />, <see cref="F:System.Double.PositiveInfinity" />, or
        ///     <see cref="F:System.Double.NegativeInfinity" />.
        /// </exception>
        /// <exception cref="FormatException"><paramref name="format" /> is invalid. </exception>
        /// <exception cref="ArgumentNullException">The <paramref name="function" /> parameter is null. </exception>
        public static async Task<string> GetSizeStringAsync(double size)
        {
            if (size <= 0)
            {
                return "0 B";
            }

            return await Task.Run(() =>
                                  {
                                      var digitGroups = (int)(Math.Log10(size) / Math.Log10(1024));

                                      if (digitGroups > Units.Length)
                                      {
                                          return "Many Bytes";
                                      }

                                      var deci = new decimal(size / Math.Pow(1024, digitGroups));
                                      return deci.ToString("#,##0.#") + " " + Units[digitGroups];
                                  });
        }

        /// <exception cref="ArgumentNullException"><paramref name="buffer" /> is null. </exception>
        /// <exception cref="ArgumentNullException"><paramref name="s" /> is null. </exception>
        /// <exception cref="EncoderFallbackException">
        ///     A fallback occurred (see Character Encoding in the .NET Framework for
        ///     complete explanation)-and-<see cref="P:System.Text.Encoding.EncoderFallback" /> is set to
        ///     <see cref="T:System.Text.EncoderExceptionFallback" />.
        /// </exception>
        public static async Task<Stream> GetStreamAsync(this string s)
        {
            return await Task.Run(async () => new MemoryStream(await s.GetBytesAsync()));
        }

        /// <summary>
        ///     This checks to see if item is any of the possible matches.  It returns true if it equals one.  Item and
        ///     possibleMatches are not allowed to be null.
        /// </summary>
        /// <param name="item">            the string we are looking for </param>
        /// <param name="ignoreCase">      ignore string casing </param>
        /// <param name="possibleMatches">
        ///     the matches we are checking the item against
        /// </param>
        /// <returns> true if item equals one of the possible matches </returns>
        /// <exception cref="ArgumentException">
        ///     <paramref name="comparisonType" /> is not a
        ///     <see cref="T:System.StringComparison" /> value.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     The <paramref name="source" /> argument is null.-or-The
        ///     <paramref name="body" /> argument is null.
        /// </exception>
        /// <exception cref="AggregateException">The exception that contains all the individual exceptions thrown on all threads.</exception>
        /// <exception cref="InvalidOperationException">
        ///     The <see cref="M:System.Threading.Tasks.ParallelLoopState.Stop" /> method
        ///     was previously called. <see cref="M:System.Threading.Tasks.ParallelLoopState.Break" /> and
        ///     <see cref="M:System.Threading.Tasks.ParallelLoopState.Stop" /> may not be used in combination by iterations of the
        ///     same loop.
        /// </exception>
        public static async Task<bool> InAsync(this string item, bool ignoreCase, params string[] possibleMatches)
        {
            return await Task.Run(() =>
                                  {
                                      var retVal = false;

                                      if (item != null && possibleMatches != null)
                                      {
                                          Parallel.ForEach(possibleMatches,
                                                           (possibleMatch, loopState) =>
                                                           {
                                                               if (!ignoreCase && possibleMatch.Contains(item))
                                                               {
                                                                   retVal = true;
                                                                   loopState.Break();
                                                               }

                                                               if (ignoreCase && CultureInfo.CurrentCulture.CompareInfo.IndexOf(possibleMatch, item, CompareOptions.IgnoreCase) >= 0)
                                                               {
                                                                   retVal = true;
                                                                   loopState.Break();
                                                               }
                                                           });
                                      }

                                      return retVal;
                                  });
        }

        /// <summary>
        ///     Determines if a string is all lower case characters
        /// </summary>
        /// <param name="stringToCheck">the string to check if it is all lower case</param>
        /// <returns>True if all lower case, false otherwise</returns>
        public static bool IsAllLower(this string stringToCheck) => !string.IsNullOrEmpty(stringToCheck) && stringToCheck.ToLower().Equals(stringToCheck, StringComparison.InvariantCulture);

        /// <summary>
        ///     Determines if a string is all upper case characters
        /// </summary>
        /// <param name="stringToCheck">the string to check if it is all upper case</param>
        /// <returns>True if all upper case, false otherwise</returns>
        /// <exception cref="ArgumentException">
        ///     <paramref name="comparisonType" /> is not a
        ///     <see cref="T:System.StringComparison" /> value.
        /// </exception>
        public static bool IsAllUpper(this string stringToCheck) => !string.IsNullOrEmpty(stringToCheck) && stringToCheck.ToUpper().Equals(stringToCheck, StringComparison.InvariantCulture);

        public static bool IsNullOrEmpty(this string s) => s == null || s.Equals(string.Empty);

        /// <summary>
        ///     This will return the null if the parameter is null or empty.
        /// </summary>
        /// <param name="s">
        ///     the string to check
        /// </param>
        /// <returns> null if the parameter is null or empty - otherwise the parameter </returns>
        public static string NullIfEmpty(this string s) => string.IsNullOrEmpty(s) ? null : s;

        /// <exception cref="RegexMatchTimeoutException">
        ///     A time-out occurred. For more information about time-outs, see the Remarks
        ///     section.
        /// </exception>
        /// <exception cref="ArgumentNullException"><paramref name="input" /> or <paramref name="replacement" /> is null.</exception>
        /// <exception cref="ArgumentException">A regular expression parsing error occurred. </exception>
        public static async Task<string> ReplaceAllAsync(this string input, string pattern, string replacement)
        {
            return await Task.Run(() => new Regex(pattern).Replace(input, replacement));
        }

        /// <summary>
        ///     Replaces the last occurrence of a string within the target string with the specified replacement
        /// </summary>
        /// <param name="source">The string to modify</param>
        /// <param name="target">The string the is going to be replaced</param>
        /// <param name="replacement">The string to replace the target with</param>
        /// <returns>The string with the last occurrence replaced</returns>
        /// <exception cref="ArgumentNullException"><paramref name="value" /> is null. </exception>
        /// <exception cref="ArgumentException">
        ///     <paramref name="comparisonType" /> is not a valid
        ///     <see cref="T:System.StringComparison" /> value.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     Either <paramref name="startIndex" /> or <paramref name="count" /> is
        ///     less than zero.-or- <paramref name="startIndex" /> plus <paramref name="count" /> specify a position outside this
        ///     instance.
        /// </exception>
        public static async Task<string> ReplaceLastOccurrenceAsync(this string source, string target, string replacement)
        {
            return await Task.Run(() =>
                                  {
                                      var place = source.LastIndexOf(target, StringComparison.Ordinal);
                                      return place == -1 ? source : source.Remove(place, target.Length).Insert(place, replacement);
                                  });
        }

        /// <exception cref="ArgumentNullException">The <paramref name="function" /> parameter is null. </exception>
        public static async Task<string> ToLowerCaseAsync(this string s)
        {
            return await Task.Run(() => s?.ToLower());
        }

        #endregion

        #region Statics And Constants

        private static readonly Random Random = new Random();

        private static readonly string[] Units =
        {
            "B",
            "KB",
            "MB",
            "GB",
            "TB",
            "PB",
            "EB",
            "ZB",
            "YB"
        };

        #endregion
    }
}