using Dawn;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Ploch.Common.WebUI.TagUtilities
{
    /// <summary>
    ///     Helper class for creating <see cref="SelectListItem" /> collections.
    /// </summary>
    public static class SelectListHelper
    {
        /// <summary>
        ///     Creates a list of <see cref="SelectListItem" /> from the <paramref name="items" /> collection.
        /// </summary>
        /// <remarks>
        ///     Maps items from the <paramref name="items" /> to <see cref="SelectListItem" /> using the
        ///     <paramref name="textFunc" /> and
        ///     <paramref name="valueFunc" /> functions.
        /// </remarks>
        /// <param name="items">The source collection.</param>
        /// <param name="textFunc">
        ///     Function mapping an item to the <see cref="SelectListItem.Text" /> property of a
        ///     <see cref="SelectListItem" />.
        /// </param>
        /// <param name="valueFunc">
        ///     Function mapping an item to the <see cref="SelectListItem.Value" /> property of a
        ///     <see cref="SelectListItem" />.
        /// </param>
        /// <param name="includeNull">
        ///     If <c>True</c> then <c>string.Empty</c> value entry will be added to the result
        ///     collection. Optional, defaults to <c>False</c>.
        /// </param>
        /// <param name="nullText">The <c>string.Empty</c> item text.</param>
        /// <typeparam name="TModel">The <paramref name="items" /> collection item type.</typeparam>
        /// <returns>a list of <see cref="SelectListItem" /> from the <paramref name="items" /> collection.</returns>
        public static IList<SelectListItem> CreateFor<TModel>(IEnumerable<TModel> items,
                                                              Func<TModel, object> textFunc,
                                                              Func<TModel, object> valueFunc,
                                                              bool includeNull = false,
                                                              string nullText = "--- Select ---")
        {
            // ReSharper disable once PossibleMultipleEnumeration - false-positive
            Guard.Argument(items, nameof(items)).NotNull();
            Guard.Argument(textFunc, nameof(textFunc)).NotNull();
            Guard.Argument(valueFunc, nameof(valueFunc)).NotNull();

            // ReSharper disable once PossibleMultipleEnumeration - false-positive
#pragma warning disable CC0031 // Check for null before calling a delegate - false-positive
            var result = items.Select(item => new SelectListItem(textFunc(item).ToString(), valueFunc(item).ToString())).ToList();
#pragma warning restore CC0031
            if (includeNull)
            {
                result.Insert(0, new SelectListItem(nullText, string.Empty));
            }

            return result;
        }
    }
}