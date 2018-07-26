using System;
using System.Collections.Generic;
using System.Linq;

#pragma warning disable 1584,1711,1572,1581,1580

namespace Service.Extensions
{
    public static class GeneralUtils
    {
        #region Public Methods

        /// <summary>
        ///     Adds unique elements from source to target list
        /// </summary>
        /// <typeparam name="T">Type of collections</typeparam>
        /// <param name="target">Target list</param>
        /// <param name="sourceList">Source list</param>
        /// <param name="checkSourceListUniqueness">If set to false method won't check for duplicates in sourceList</param>
        /// <exception cref="NullReferenceException">target is null</exception>
        /// <exception cref="ArgumentNullException"><paramref name="collection" /> is null.</exception>
        public static void AddUnique<T>(this List<T> target, List<T> sourceList, bool checkSourceListUniqueness)
        {
            if (target == null)
            {
                throw new NullReferenceException();
            }

            if (sourceList == null)
            {
                return;
            }

            var hashSet = new HashSet<T>(target);

            foreach (var value in sourceList)
            {
                if (!hashSet.Contains(value))
                {
                    target.Add(value);
                }

                if (checkSourceListUniqueness)
                {
                    hashSet.Add(value);
                }
            }
        }

        /// <summary>
        ///     Merges passed byte arrays into one
        /// </summary>
        /// <param name="arrays">Arrays to merge</param>
        /// <returns>Merged array</returns>
        /// <exception cref="ArgumentNullException">Argument is null</exception>
        /// <exception cref="OverflowException">
        ///     The array is multidimensional and contains more than
        ///     <see cref="F:System.Int32.MaxValue" /> elements.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     <paramref name="src" /> or <paramref name="dst" /> is not an array of primitives.-or- The number of bytes in
        ///     <paramref name="src" /> is less than <paramref name="srcOffset" /> plus <paramref name="count" />.-or- The number
        ///     of bytes in <paramref name="dst" /> is less than <paramref name="dstOffset" /> plus <paramref name="count" />.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     <paramref name="srcOffset" />, <paramref name="dstOffset" />, or <paramref name="count" /> is less than 0.
        /// </exception>
        public static byte[] AppendByteArrays(params byte[][] arrays)
        {
            if (arrays == null)
            {
                throw new ArgumentNullException(nameof(arrays));
            }

            var totalLength = arrays.Sum(array => array.Length);

            var allBytes = new byte[totalLength];
            var offset = 0;
            foreach (var array in arrays)
            {
                Buffer.BlockCopy(array, 0, allBytes, offset, array.Length);
                offset += array.Length;
            }

            return allBytes;
        }

        /// <summary>
        ///     Checks if two dictionaries are equal
        /// </summary>
        /// <typeparam name="TKey">Key type</typeparam>
        /// <typeparam name="TValue">Value type</typeparam>
        /// <param name="first">First dictionary</param>
        /// <param name="second">Second dictionary</param>
        /// <returns>True if two dictionaries are same, false otherwise</returns>
        /// <exception cref="NullReferenceException">first is null</exception>
        /// <remarks>
        ///     It throws if first paramether is null. To get 'true' when both values are null do
        ///     <code>
        /// bool equals = first?.DictEquals(second) ?? RefenceEquals(first, second);
        /// </code>
        /// </remarks>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="collection" /> is null.
        /// </exception>
        public static bool DictEquals<TKey, TValue>(this IDictionary<TKey, TValue> first, IDictionary<TKey, TValue> second)
        {
            if (first == null)
            {
                throw new NullReferenceException();
            }

            if (second == null)
            {
                return false;
            }

            if (ReferenceEquals(first, second))
            {
                return true;
            }

            if (first.Count != second.Count)
            {
                return false;
            }

            var hash = new HashSet<KeyValuePair<TKey, TValue>>(first);

            return second.All(pair => hash.Contains(pair));
        }

        /// <summary>
        ///     Checks if collection is null or empty
        /// </summary>
        /// <typeparam name="T">Element type</typeparam>
        /// <param name="collection">Collection to check</param>
        /// <returns>True if collection is null or empty, false otherwise</returns>
        public static bool IsNullOrEmpty<T>(this ICollection<T> collection)
        {
            if (collection == null)
            {
                return true;
            }

            return collection.Count == 0;
        }

        /// <summary>
        ///     Checks two lists for equality
        /// </summary>
        /// <typeparam name="T">Element type</typeparam>
        /// <param name="first">First list</param>
        /// <param name="second">Second list</param>
        /// <param name="ordered">If set to false treats lists as unordered collections</param>
        /// <returns>True if lists are equal, false otherwise</returns>
        /// <exception cref="NullReferenceException">first is null</exception>
        /// <remarks>
        ///     It throws if first paramether is null. To get 'true' when both values are null do
        ///     <code>
        /// bool equals = first?.ListEquals(second) ?? RefenceEquals(first, second);
        /// </code>
        /// </remarks>
        public static bool ListEquals<T>(this IList<T> first, IList<T> second, bool ordered)
        {
            if (first == null)
            {
                throw new NullReferenceException();
            }

            if (second == null)
            {
                return false;
            }

            if (ReferenceEquals(first, second))
            {
                return true;
            }

            if (first.Count != second.Count)
            {
                return false;
            }

            return ordered ? InternalEqualsOrdered(first, second) : InternalEqualsUnordered(first, second);
        }

        /// <summary>
        ///     Creates a new list containing only the unique items
        /// </summary>
        /// <param name="lists"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">Argument is null</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="capacity" /> is less than 0. </exception>
        public static IEnumerable<T> MergeAndRemoveDuplicates<T>(params IEnumerable<T>[] lists)
        {
            if (lists.All(l => l == null))
            {
                throw new ArgumentNullException(nameof(lists));
            }

            var result = lists.Where(enumerable => enumerable != null)
                              .Aggregate<IEnumerable<T>, IEnumerable<T>>(null,
                                                                         (current, enumerable) =>
                                                                         {
                                                                             IEnumerable<T> second = enumerable as List<T> ?? enumerable.ToList();
                                                                             return current?.Union(second) ?? second.Distinct();
                                                                         });

            return result ?? new List<T>(0);
        }

        #endregion

        #region Private Methods

        private static bool InternalEqualsOrdered<T>(IEnumerable<T> first, IList<T> second)
        {
            var comparer = EqualityComparer<T>.Default;
            return !first.Where((t, i) => !comparer.Equals(t, second[i])).Any();
        }

        private static bool InternalEqualsUnordered<T>(IEnumerable<T> first, IEnumerable<T> second)
        {
            var hashSet = new HashSet<T>(first);

            return second.All(a => hashSet.Contains(a));
        }

        #endregion
    }
}