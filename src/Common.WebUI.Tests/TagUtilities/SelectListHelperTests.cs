using Microsoft.AspNetCore.Mvc.Rendering;
using Objectivity.AutoFixture.XUnit2.AutoMoq.Attributes;
using Ploch.Common.WebUI.TagUtilities;

namespace Ploch.Common.WebUI.Tests.TagUtilities;

public class SelectListHelperTests
{
    [Theory]
    [AutoMockData]
    public void CreateFor_with_includeNull_false_should_create_list_SelectListItem(IList<TestModel> testModels)
    {
        SelectListHelper.CreateFor(testModels, m => m.MyText + m.MyTextSuffix, m => m.MyValue + m.MyValueSuffix)
                        .Should()
                        .BeEquivalentTo(testModels.Select(m => new SelectListItem(m.MyText + m.MyTextSuffix, m.MyValue + m.MyValueSuffix)));
    }

    [Theory]
    [AutoMockData]
    public void CreateFor_with_includeNull_true_should_create_list_SelectListItem_with_addition_of_empty_string_item(IList<TestModel> testModels)
    {
        var expectedItems = testModels.Select(m => new SelectListItem(m.MyText + m.MyTextSuffix, m.MyValue + m.MyValueSuffix)).ToList();
        expectedItems.Insert(0, new SelectListItem("EmptyItem", string.Empty));
        SelectListHelper.CreateFor(testModels, m => m.MyText + m.MyTextSuffix, m => m.MyValue + m.MyValueSuffix, true, "EmptyItem")
                        .Should()
                        .BeEquivalentTo(expectedItems);
    }

    public class TestModel
    {
        public string? MyValue { get; set; }

        public string? MyText { get; set; }

        public int MyValueSuffix { get; set; }

        public int MyTextSuffix { get; set; }
    }
}