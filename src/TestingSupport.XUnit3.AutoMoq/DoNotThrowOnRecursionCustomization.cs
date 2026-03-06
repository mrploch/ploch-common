using AutoFixture;
using Ploch.Common.ArgumentChecking;

namespace Ploch.TestingSupport.XUnit3.AutoMoq;

/// <summary>
///     An AutoFixture <see cref="ICustomization" /> that removes the default
///     <see cref="ThrowingRecursionBehavior" /> so that recursive object graphs do not
///     immediately cause an exception during specimen creation.
/// </summary>
/// <remarks>
///     This customization is typically paired with <see cref="OmitOnRecursionCustomization" />
///     to omit recursive branches instead of throwing.
/// </remarks>
public class DoNotThrowOnRecursionCustomization : ICustomization
{
    /// <summary>
    ///     Removes all instances of <see cref="ThrowingRecursionBehavior" /> from the fixture.
    /// </summary>
    /// <param name="fixture">The fixture to customize.</param>
    public void Customize(IFixture fixture)
    {
        // Do not throw exception on circular references.
        fixture.NotNull(nameof(fixture))
               .Behaviors
               .OfType<ThrowingRecursionBehavior>()
               .ToList()
               .ForEach(b => fixture.Behaviors.Remove(b));
    }
}
