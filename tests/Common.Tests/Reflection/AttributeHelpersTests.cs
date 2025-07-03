using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using FluentAssertions;
using Xunit;

namespace Ploch.Common.Tests.Reflection;

[SuppressMessage("ReSharper", "PossibleMultipleEnumeration", Justification = "It doesn't matter in the test code and could reduce readability")]
public class AttributeHelpersTests
{
    [Fact]
    public void GetAttributes_Inherited_From_ParentTest()
    {
        var attributes = typeof(TestTypes.ClassWithInherited_Attribute1_1_And_Attribute2).GetCustomAttributes<TestTypes.Attribute1Attribute>(true);

        attributes.Should().HaveCount(2);
#pragma warning disable S2219 // Runtime type checking should be simplified - this would reduce readability
        attributes.Should().Contain(static attr => attr is TestTypes.Attribute1Attribute);
#pragma warning restore S2219
        attributes.Should().Contain(static attr => attr is TestTypes.Attribute1_1Attribute);
    }

    [Fact]
    public void GetAttributesSingleNotInheritedTest()
    {
        var attributes = typeof(TestTypes.ClassWith_Attribute2).GetCustomAttributes<TestTypes.Attribute2Attribute>();

        attributes.Should().HaveCount(1);
        var attribute = attributes.Single();

        attribute.Name.Should().Be(nameof(TestTypes.Attribute2Attribute));
        attribute.GetType().Should().Be<TestTypes.Attribute2Attribute>();
    }
}
