using AutoFixture;
using FluentAssertions;
using Xunit;

namespace Ploch.Common.Tests.Reflection
{
    public class ObjectCloningHelperTests
    {
        private static readonly Fixture _fixture = new();

        [Fact]
        public void CopyPropertiesExcludedTest()
        {
            var testSourceObj = _fixture.Create<TestTypes.TestTypeWithMixedSettersAndGetter>();
            var testTargetObj = _fixture.Create<TestTypes.TestTypeWithMixedSettersAndGetter>();

            testSourceObj.CopyPropertiesExcluding(testTargetObj,
                                                  nameof(TestTypes.TestTypeWithMixedSettersAndGetter.IntProp2),
                                                  nameof(TestTypes.TestTypeWithMixedSettersAndGetter.StringProp2));

            testTargetObj.IntProp.Should().Be(testSourceObj.IntProp);
            testTargetObj.IntProp2.Should().NotBe(testSourceObj.IntProp2);
            testTargetObj.StringProp.Should().Be(testSourceObj.StringProp);
            testTargetObj.StringProp2.Should().NotBe(testSourceObj.StringProp2);
            testTargetObj._stringPropNoGetter.Should().NotBe(testSourceObj._stringPropNoGetter);
            testTargetObj.StringPropNoSetter.Should().NotBe(testSourceObj.StringPropNoSetter);
        }

        [Fact]
        public void CopyPropertiesIncludedTest()
        {
            var testSourceObj = _fixture.Create<TestTypes.TestTypeWithMixedSettersAndGetter>();
            var testTargetObj = _fixture.Create<TestTypes.TestTypeWithMixedSettersAndGetter>();

            testSourceObj.CopyPropertiesIncludeOnly(testTargetObj,
                                                    nameof(TestTypes.TestTypeWithMixedSettersAndGetter.IntProp2),
                                                    nameof(TestTypes.TestTypeWithMixedSettersAndGetter.StringProp2));

            testTargetObj.IntProp2.Should().Be(testSourceObj.IntProp2);
            testTargetObj.StringProp2.Should().Be(testSourceObj.StringProp2);

            testTargetObj.IntProp.Should().NotBe(testSourceObj.IntProp);
            testTargetObj.StringProp.Should().NotBe(testSourceObj.StringProp);
            testTargetObj._stringPropNoGetter.Should().NotBe(testSourceObj._stringPropNoGetter);
            testTargetObj.StringPropNoSetter.Should().NotBe(testSourceObj.StringPropNoSetter);
        }

        [Fact]
        public void CopyPropertiesTest()
        {
            var testSourceObj = _fixture.Create<TestTypes.TestTypeWithMixedSettersAndGetter>();
            var testTargetObj = _fixture.Create<TestTypes.TestTypeWithMixedSettersAndGetter>();

            testSourceObj.CopyProperties(testTargetObj);

            testTargetObj.IntProp.Should().Be(testSourceObj.IntProp);
            testTargetObj.IntProp2.Should().Be(testSourceObj.IntProp2);
            testTargetObj.StringProp.Should().Be(testSourceObj.StringProp);
            testTargetObj.StringProp2.Should().Be(testSourceObj.StringProp2);
            testTargetObj._stringPropNoGetter.Should().NotBe(testSourceObj._stringPropNoGetter);
            testTargetObj.StringPropNoSetter.Should().NotBe(testSourceObj.StringPropNoSetter);
        }
    }
}