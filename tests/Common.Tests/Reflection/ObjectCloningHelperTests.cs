using AutoFixture;
using Ploch.Common.Tests.TestTypes.TestingTypes;

namespace Ploch.Common.Tests.Reflection;

public class ObjectCloningHelperTests
{
    private static readonly Fixture Fixture = new();

    [Fact]
    public void CopyPropertiesExcludedTest()
    {
        var testSourceObj = Fixture.Create<TestTypeWithMixedSettersAndGetter>();
        var testTargetObj = Fixture.Create<TestTypeWithMixedSettersAndGetter>();

        testSourceObj.CopyPropertiesExcluding(testTargetObj,
                                              nameof(TestTypeWithMixedSettersAndGetter.IntProp2),
                                              nameof(TestTypeWithMixedSettersAndGetter.StringProp2));

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
        var testSourceObj = Fixture.Create<TestTypeWithMixedSettersAndGetter>();
        var testTargetObj = Fixture.Create<TestTypeWithMixedSettersAndGetter>();

        testSourceObj.CopyPropertiesIncludeOnly(testTargetObj,
                                                nameof(TestTypeWithMixedSettersAndGetter.IntProp2),
                                                nameof(TestTypeWithMixedSettersAndGetter.StringProp2));

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
        var testSourceObj = Fixture.Create<TestTypeWithMixedSettersAndGetter>();
        var testTargetObj = Fixture.Create<TestTypeWithMixedSettersAndGetter>();

        testSourceObj.CopyProperties(testTargetObj);

        testTargetObj.IntProp.Should().Be(testSourceObj.IntProp);
        testTargetObj.IntProp2.Should().Be(testSourceObj.IntProp2);
        testTargetObj.StringProp.Should().Be(testSourceObj.StringProp);
        testTargetObj.StringProp2.Should().Be(testSourceObj.StringProp2);
        testTargetObj._stringPropNoGetter.Should().NotBe(testSourceObj._stringPropNoGetter);
        testTargetObj.StringPropNoSetter.Should().NotBe(testSourceObj.StringPropNoSetter);
    }
}
