namespace Ploch.Common.Matchers;

/// <summary>
///     Defines a contract for matching a specified value of type T.
/// </summary>
/// <typeparam name="T">The type of the value to be matched.</typeparam>
public interface IMatcher<in T>
{
    /// <summary>
    ///     Checks if the given value satisfies the matching criteria.
    /// </summary>
    /// <param name="value">The value to evaluate against the matcher's criteria.</param>
    /// <returns>
    ///     <c>true</c> if the provided value matches the criteria; otherwise, <c>false</c>.
    /// </returns>
    bool IsMatch(T value);
}
