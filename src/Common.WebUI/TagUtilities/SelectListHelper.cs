using Microsoft.AspNetCore.Mvc.Rendering;

namespace Ploch.Common.WebUI.TagUtilities
{
    public static class SelectListHelper
    {
        public static IList<SelectListItem> CreateFor<TModel>(IEnumerable<TModel> items,
                                                              Func<TModel, object> textFunc,
                                                              Func<TModel, object> valueFunc,
                                                              bool includeNull = false,
                                                              string nullText = "--- Select ---")
        {
            var result = items.Select(item => new SelectListItem(textFunc(item).ToString(), valueFunc(item).ToString())).ToList();
            if (includeNull)
            {
                result.Insert(0, new SelectListItem(nullText, string.Empty));
            }

            return result;
        }
    }
}