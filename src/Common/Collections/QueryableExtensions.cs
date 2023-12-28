using System;
using System.Linq;

namespace Ploch.Common.Collections;

/// <summary>
///     Extension methods for the <see cref="IQueryable{T}" /> interface.
/// </summary>
public static class QueryableExtensions
{
    /// <summary>
    ///     Creates an enumerable query if the condition is true.
    /// </summary>
    /// <remarks>
    ///     This method is useful when you want to conditionally add a where clause to a query.
    /// </remarks>
    /// <example>
    ///     Method below returns a list of cars ordered by creation date.
    ///     If the <c>createdAfter</c> is not null, the query will be filtered by the creation date.
    ///     If the <c>createdBefore</c> is not null, the query will be further filtered by the creation date.
    ///     <code>
    /// var carsList = GetCars();
    /// carsList.OrderBy(x =&gt; x.Created)
    /// .If(createdAfter.HasValue, x =&gt; x.Where(y =&gt; y.Created &gt; createdAfter!.Value))
    /// .If(createdBefore.HasValue, x =&gt; x.Where(y =&gt; y.Created &lt; createdBefore!.Value))
    /// .If(first.HasValue, x =&gt; x.Take(first!.Value))
    /// </code>
    /// </example>
    /// <param name="queryable">The source enumerable.</param>
    /// <param name="condition">The condition.</param>
    /// <param name="action">The query action to perform on <paramref name="enumerable" />.</param>
    /// <typeparam name="T">The enumerable value type.</typeparam>
    /// <returns>The resulting enumerable.</returns>
    public static IQueryable<T> If<T>(this IQueryable<T> queryable, bool condition, Func<IQueryable<T>, IQueryable<T>> action)
    {
        return queryable.If<IQueryable<T>, T>(condition, action);
    }
}