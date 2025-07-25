using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using FluentAssertions;
using Ploch.Common.Tests.TestTypes.TestingTypes;

namespace Ploch.Common.Tests.Reflection;

[SuppressMessage("ReSharper", "PossibleMultipleEnumeration", Justification = "It doesn't matter in the test code and could reduce readability")]
public class AttributeHelpersTests
{
    [Fact]
    public void GetAttributes_Inherited_From_ParentTest()
    {
        var attributes = typeof(ClassWithInherited_Attribute1_1_And_Attribute2).GetCustomAttributes<Attribute1Attribute>(true);

        attributes.Should().HaveCount(2);
#pragma warning disable S2219 // Runtime type checking should be simplified - this would reduce readability
        attributes.Should().Contain(static attr => attr is Attribute1Attribute);
#pragma warning restore S2219
        attributes.Should().Contain(static attr => attr is Attribute1_1Attribute);
    }

    [Fact]
    public void GetAttributesSingleNotInheritedTest()
    {
        var attributes = typeof(ClassWith_Attribute2).GetCustomAttributes<Attribute2Attribute>();

        attributes.Should().HaveCount(1);
        var attribute = attributes.Single();

        attribute.Name.Should().Be(nameof(Attribute2Attribute));
        attribute.GetType().Should().Be<Attribute2Attribute>();
    }
}
