namespace NetVisionProc.Common.Extensions
{
    /// <summary>
    /// Author: Nikita Nikitins .
    /// Email: nikitinsn6@gmail.com .
    /// </summary>
    [ExcludeFromCodeCoverage]
    public static class ListExt
    {
        /// <summary>
        /// Checks if the IList is null or empty.
        /// </summary>
        /// <typeparam name="T">Type of elements in the IList.</typeparam>
        /// <param name="values">The IList to check.</param>
        /// <returns>True if the IList is null or has zero elements; otherwise, false.</returns>
        public static bool IsNullOrEmpty<T>([NotNullWhen(false)] this IList<T>? values)
        {
            return values is null || values.Count == 0;
        }

        /// <summary>
        /// Checks if the IEnumerable is null or empty.
        /// </summary>
        /// <typeparam name="T">Type of elements in the IEnumerable.</typeparam>
        /// <param name="values">The IEnumerable to check.</param>
        /// <returns>True if the IEnumerable is null or has no elements; otherwise, false.</returns>
        public static bool IsNullOrEmpty<T>([NotNullWhen(false)] this IEnumerable<T>? values)
        {
            return values is null || !values.Any();
        }

        /// <summary>
        /// Checks if the IList is not null and has at least one item.
        /// </summary>
        /// <typeparam name="T">Type of elements in the IList.</typeparam>
        /// <param name="values">The IList to check.</param>
        /// <returns>True if the IList is not null and has at least one element; otherwise, false.</returns>
        public static bool HasValue<T>([NotNullWhen(true)] this IList<T>? values)
        {
            return values is not null && values.Count > 0;
        }

        /// <summary>
        /// Checks if the IEnumerable is not null and has at least one item.
        /// </summary>
        /// <typeparam name="T">Type of elements in the IEnumerable.</typeparam>
        /// <param name="values">The IEnumerable to check.</param>
        /// <returns>True if the IEnumerable is not null and has at least one element; otherwise, false.</returns>
        public static bool HasValue<T>([NotNullWhen(true)] this IEnumerable<T>? values)
        {
            return values is not null && values.Any();
        }
    }
}