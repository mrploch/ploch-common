using System;
using System.Linq;
using System.Reflection;
using FluentAssertions;
using Xunit;

namespace Ploch.Common.Tests.Reflection
{
    public class AttributeHelpersTests
    {
        [Fact]
        public void GetAttributes_Inherited_From_ParentTest()
        {
            var attributes = typeof(TestTypes.ClassWithInherited_Attribute1_1_And_Attribute2).GetCustomAttributes<TestTypes.Attribute1Attribute>(true);

            attributes.Should().HaveCount(2);
            attributes.Should().Contain(attr => attr is TestTypes.Attribute1Attribute);
            attributes.Should().Contain(attr => attr is TestTypes.Attribute1_1Attribute);
        }

        /// <exception cref="TypeLoadException">
        ///     A custom attribute type cannot be loaded.
        /// </exception>
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
}