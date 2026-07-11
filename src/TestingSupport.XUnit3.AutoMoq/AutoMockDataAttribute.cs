using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Xunit3;

namespace Ploch.TestingSupport.XUnit3.AutoMoq;

/// <summary>
///     An <see cref="AutoDataAttribute" /> preconfigured with <see cref="AutoMoqCustomization" />
///     so that constructor and method parameters are automatically populated and all
///     interface and abstract dependencies are provided as Moq mocks.
/// </summary>
/// <remarks>
///     Apply this attribute to xUnit.net 3 test methods to enable auto-generated data and mocks.
///     It creates a new <see cref="Fixture" /> instance per test invocation and customizes it with
///     <see cref="AutoMoqCustomization" />. You can combine it with <c>[InlineData]</c> or
///     other AutoFixture attributes as needed.
/// </remarks>
/// <param name="ignoreVirtualMembers">Whether to ignore virtual members during specimen generation.</param>
/// <example>
///     <code>
///     [Theory]
///     [AutoMockData]
///     public void Service_Uses_Repository(IMyService sut, Mock&lt;IMyRepository&gt; repo)
///     {
///         // arrange via AutoFixture + Moq; use repo.Verify(...) to assert interactions
///         // act/assert ...
///     }
///     </code>
/// </example>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public sealed class AutoMockDataAttribute(bool ignoreVirtualMembers = false)
    : AutoDataAttribute(() => new Fixture().Customize(new AutoDataCommonCustomization(ignoreVirtualMembers)))
{
}
