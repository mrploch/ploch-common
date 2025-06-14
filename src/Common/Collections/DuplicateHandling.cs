namespace Ploch.Common.Collections;

/// <summary>
///     Specifies that duplicate entries will be ignored when adding items to a collection or dictionary.
///     Existing entries will remain unchanged, and the new values will not be added.
/// </summary>
public enum DuplicateHandling
{
    /// <summary>
    ///     Specifies that duplicate entries will be ignored when adding items to a collection or dictionary.
    /// </summary>
    Ignore,

    /// <summary>
    ///     Specifies that duplicate entries will be overwritten when adding items to a collection or dictionary.
    /// </summary>
    Overwrite,

    /// <summary>
    ///     Specifies that duplicate entries will throw an exception when adding items to a collection or dictionary.
    /// </summary>
    Throw
}
