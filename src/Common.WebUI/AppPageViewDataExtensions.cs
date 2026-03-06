using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Ploch.Common.WebUI;

/// <summary>
///     <see cref="ViewDataDictionary{TModel}" /> extension methods for setting and getting the current page.
/// </summary>
public static class AppPageViewDataExtensions
{
    /// <summary>
    ///     The Current Page key for the <see cref="ViewDataDictionary" />.
    /// </summary>
    public const string CurrentPage = "CurrentPage";

    /// <summary>
    ///     Sets the current page in the <see cref="ViewDataDictionary" />.
    /// </summary>
    /// <param name="viewData">The view data dictionary.</param>
    /// <param name="page">The application page to set as the current page.</param>
    public static void SetCurrentPage(this ViewDataDictionary viewData, AppPage page) => viewData[CurrentPage] = page;

    /// <summary>
    ///     Gets the current page from the <see cref="ViewDataDictionary" />.
    /// </summary>
    /// <param name="viewData">The view data dictionary.</param>
    /// <returns>The current application page if set, otherwise null.</returns>
    public static AppPage? GetCurrentPage(this ViewDataDictionary viewData) => (AppPage?)viewData[CurrentPage];

    /// <summary>
    ///     Gets the title of the current page from the <see cref="ViewDataDictionary" />.
    /// </summary>
    /// <remarks>
    ///     The title is obtained from using the <see cref="AppPage.Title" /> property of the current page.
    /// </remarks>
    /// <param name="viewData">The view data dictionary.</param>
    /// <returns>The title of the current application page if set, otherwise null.</returns>
    public static string? GetPageTitle(this ViewDataDictionary viewData) => viewData.GetCurrentPage()?.Title;
}
