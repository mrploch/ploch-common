using AutoFixture;
using AutoFixture.AutoMoq;
using Ploch.Common.ArgumentChecking;

namespace Ploch.TestingSupport.XUnit3.AutoMoq;

/// <summary>
///     Groups common AutoFixture customizations typically used with AutoMoq in unit tests.
/// </summary>
/// <remarks>
///     This customization applies:
///     - <see cref="AutoMoqCustomization" /> to create Moq mocks for abstract/interface dependencies.
///     - <see cref="DoNotThrowOnRecursionCustomization" /> to remove the default throwing recursion behavior.
///     - <see cref="OmitOnRecursionCustomization" /> to omit specimens on recursion instead of throwing.
///     Optionally, it can also add <see cref="IgnoreVirtualMembersCustomization" /> to skip virtual properties.
/// </remarks>
public class AutoDataCommonCustomization : ICustomization
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="AutoDataCommonCustomization" /> class.
    /// </summary>
    /// <param name="ignoreVirtualMembers">Whether to ignore virtual members during specimen generation.</param>
    public AutoDataCommonCustomization(bool ignoreVirtualMembers) => IgnoreVirtualMembers = ignoreVirtualMembers;

    /// <summary>
    ///     Gets a value indicating whether virtual members should be ignored.
    /// </summary>
    public bool IgnoreVirtualMembers { get; }

    /// <summary>
    ///     Applies the configured customizations to the provided <paramref name="fixture" />.
    /// </summary>
    /// <param name="fixture">The fixture to customize.</param>
    public void Customize(IFixture fixture)
    {
        var adaptedFixture = fixture.NotNull(nameof(fixture))
                                    .Customize(new AutoMoqCustomization())
                                    .Customize(new DoNotThrowOnRecursionCustomization())
                                    .Customize(new OmitOnRecursionCustomization());

        if (IgnoreVirtualMembers)
        {
            adaptedFixture.Customize(new IgnoreVirtualMembersCustomization());
        }
    }
}
