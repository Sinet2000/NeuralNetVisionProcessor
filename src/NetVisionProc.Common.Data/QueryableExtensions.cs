namespace NetVisionProc.Common.Data
{
    public static class QueryableExtensions
    {
        /// <summary>
        /// Adds related entities to the query using the specified navigation properties.
        /// </summary>
        /// <typeparam name="T">The entity type.</typeparam>
        /// <param name="query">The original IQueryable.</param>
        /// <param name="includes">A collection of navigation property names to include in the query.</param>
        /// <returns>The modified IQueryable with included navigation properties.</returns>
        public static IQueryable<T> IncludeNavigationProperties<T>(this IQueryable<T> query, IEnumerable<string>? includes = null)
            where T : class
        {
            if (includes.HasValue())
            {
                foreach (var include in includes!)
                {
                    query = query.Include(include);
                }
            }

            return query;
        }

        public static IOrderedQueryable<TSource> OrderBy<TSource, TKey>(this IQueryable<TSource> query, Expression<Func<TSource, TKey>> keySelector, bool desc)
        {
            if (desc)
            {
                return query.OrderByDescending(keySelector);
            }

            return query.OrderBy(keySelector);
        }

        public static IOrderedQueryable<TSource> ThenBy<TSource, TKey>(this IOrderedQueryable<TSource> query, Expression<Func<TSource, TKey>> keySelector, bool desc)
        {
            if (desc)
            {
                return query.ThenByDescending(keySelector);
            }

            return query.ThenBy(keySelector);
        }

        public static IOrderedQueryable<TSource> OrderBy<TSource, TKey>(this IQueryable<TSource> query, Expression<Func<TSource, TKey>> keySelector, bool useThenBy, bool desc)
        {
            if (useThenBy)
            {
                return ((IOrderedQueryable<TSource>)query).ThenBy(keySelector, desc);
            }

            return query.OrderBy(keySelector, desc);
        }
    }
}
