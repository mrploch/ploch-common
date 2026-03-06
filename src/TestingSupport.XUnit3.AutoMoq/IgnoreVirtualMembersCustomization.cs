using AutoFixture;
using Ploch.Common.ArgumentChecking;

namespace Ploch.TestingSupport.XUnit3.AutoMoq;

/// <summary>
///     An AutoFixture <see cref="ICustomization" /> that omits values for virtual properties
///     when generating specimens. This is useful when the object under test exposes
///     Moq-virtualized members or lazy-loading proxies that you do not want AutoFixture to populate.
/// </summary>
public class IgnoreVirtualMembersCustomization : ICustomization
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="IgnoreVirtualMembersCustomization" /> class
    ///     that applies to all types.
    /// </summary>
    public IgnoreVirtualMembersCustomization()
        : this(null)
    { }

    /// <summary>
    ///     Initializes a new instance of the <see cref="IgnoreVirtualMembersCustomization" /> class.
    /// </summary>
    /// <param name="reflectedType">
    ///     Optional type filter. When provided, only virtual properties whose <see cref="System.Reflection.MemberInfo.ReflectedType" />
    ///     matches this value will be omitted. When <c>null</c>, virtual properties on any type are omitted.
    /// </param>
    public IgnoreVirtualMembersCustomization(Type? reflectedType) => ReflectedType = reflectedType;

    /// <summary>
    ///     Gets the optional type that limits where virtual members are ignored.
    /// </summary>
    public Type? ReflectedType { get; }

    /// <summary>
    ///     Adds an <see cref="IgnoreVirtualMembersSpecimenBuilder" /> to the fixture to omit values for matching virtual properties.
    /// </summary>
    /// <param name="fixture">The fixture to customize.</param>
    public void Customize(IFixture fixture)
    {
        fixture.NotNull(nameof(fixture)).Customizations.Add(new IgnoreVirtualMembersSpecimenBuilder(ReflectedType));
    }
}
