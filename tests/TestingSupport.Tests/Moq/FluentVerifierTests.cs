using FluentAssertions;
using Moq;
using Objectivity.AutoFixture.XUnit2.AutoMoq.Attributes;
using Ploch.TestingSupport.Moq;

namespace Ploch.TestingSupport.Tests.Moq;

public class FluentVerifierTests
{
    [Theory]
    [AutoMockData]
    public async Task VerifyFluentAssertion_should_match_verification_using_fluent_assertions(Mock<IMyService1> myServiceMock,
                                                                                              TestRecord1 testRecord1A,
                                                                                              TestRecord2 testRecord2A,
                                                                                              TestRecord1 testRecord1B,
                                                                                              TestRecord2 testRecord2B)
    {
        myServiceMock.Object.DoSomething(testRecord1A, testRecord2A);

        FluentVerifier.VerifyFluentAssertion(() =>
                                             {
                                                 var x = 1;
                                                 var y = 2;
                                             });
        myServiceMock.Verify(x => x.DoSomething(It.Is<TestRecord1>(rec1 => Verify(rec1, testRecord1A)),
                                                It.Is<TestRecord2>(rec2 => Verify(rec2, testRecord2A))));
    }

    private static bool Verify(TestRecord1 actual, TestRecord1 expected) =>
        FluentVerifier.VerifyFluentAssertion(() =>
                                             {
                                                 actual.BoolProperty.Should()
                                                       .Be(expected.BoolProperty);
                                                 actual.IntProperty.Should().Be(expected.IntProperty);
                                                 actual.StringProperty.Should().Be(expected.StringProperty);
                                             });

    private static bool Verify(TestRecord2 actual, TestRecord2 expected) =>
        FluentVerifier.VerifyFluentAssertion(() =>
                                             {
                                                 actual.RecordProperty1.BoolProperty.Should()
                                                       .Be(expected.RecordProperty1.BoolProperty);
                                                 actual.RecordProperty1.IntProperty.Should().Be(expected.RecordProperty1.IntProperty);
                                                 actual.RecordProperty1.StringProperty.Should().Be(expected.RecordProperty1.StringProperty);

                                                 actual.RecordProperty2.BoolProperty.Should()
                                                       .Be(expected.RecordProperty2.BoolProperty);
                                                 actual.RecordProperty2.IntProperty.Should().Be(expected.RecordProperty2.IntProperty);
                                                 actual.RecordProperty2.StringProperty.Should().Be(expected.RecordProperty2.StringProperty);
                                             });
}

public interface IMyService1
{
    void DoSomething(TestRecord1 inputParameter1, TestRecord2 inputParameter2);

    Task DoSomethingAsync(TestRecord1 inputParameter1, TestRecord2 inputParameter2);

    int DoSomethingWithResult(TestRecord1 inputParameter1, TestRecord2 inputParameter2);

    Task<int> DoSomethingAsyncWithResultAsync(TestRecord1 inputParameter1, TestRecord2 inputParameter2);
}

public record TestRecord1(int IntProperty, string StringProperty, bool BoolProperty);

public record TestRecord2(TestRecord1 RecordProperty1, TestRecord1 RecordProperty2);
