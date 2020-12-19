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
            var attributes = typeof(TestTypes.ClassWithInherited_Attribute1_1_and_Attribute2)
                .GetCustomAttributes<TestTypes.Attribute1>(true);

            attributes.Should().HaveCount(2);
            attributes.Should().ContainSingle(attr => attr.GetType() == typeof(TestTypes.Attribute1));
            attributes.Should().ContainSingle(attr => attr.GetType() == typeof(TestTypes.Attribute1_1));
        }

        /// <exception cref="TypeLoadException">
        ///     A custom attribute type cannot be loaded.
        /// </exception>
        [Fact]
        public void GetAttributesSingleNotInheritedTest()
        {
            var attributes =
                typeof(TestTypes.ClassWith_Attribute2).GetCustomAttributes<TestTypes.Attribute2>();

            attributes.Should().HaveCount(1);
            var attribute = attributes.Single();

            attribute.Name.Should().Be(nameof(TestTypes.Attribute2));
            attribute.GetType().Should().Be<TestTypes.Attribute2>();
        }
    }
}