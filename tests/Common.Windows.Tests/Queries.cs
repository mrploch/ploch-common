namespace Ploch.Common.Windows.Tests;

public static class Queries
{
    public static IEnumerable<T> GetWithEmptyProperty<T>(this IEnumerable<T> items, Func<T, string?> propertySelector) =>
        items.Where(i => string.IsNullOrWhiteSpace(propertySelector(i)));
}