using AutoFixture;
using Ploch.Common.ArgumentChecking;

namespace Ploch.TestingSupport.XUnit3.AutoMoq;

/// <summary>
///     An AutoFixture <see cref="ICustomization" /> that adds <see cref="OmitOnRecursionBehavior" />
///     so that recursive graphs are truncated by omitting specimens instead of throwing.
/// </summary>
public class OmitOnRecursionCustomization : ICustomization
{
    /// <summary>
    ///     Adds <see cref="OmitOnRecursionBehavior" /> to the fixture so that recursion is handled by omission.
    /// </summary>
    /// <param name="fixture">The fixture to customize.</param>
    public void Customize(IFixture fixture)
    {
        // Omit recursion on the first level.
        fixture.NotNull(nameof(fixture))
               .Behaviors
               .Add(new OmitOnRecursionBehavior());
    }
}
