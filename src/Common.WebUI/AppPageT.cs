namespace Ploch.Common.WebUI;

/// <summary>
///     Represents a page in the application.
/// </summary>
/// <remarks>
///     This class is used to represent a page in the application.
/// </remarks>
/// <param name="Name">The page name.</param>
/// <param name="Path">The page path.</param>
public record AppPage(string Name, string Path, string Title);
