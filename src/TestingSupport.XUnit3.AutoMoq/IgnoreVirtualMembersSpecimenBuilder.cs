using System.Reflection;
using AutoFixture.Kernel;
using Ploch.Common.ArgumentChecking;

namespace Ploch.TestingSupport.XUnit3.AutoMoq;

/// <summary>
///     An <see cref="ISpecimenBuilder" /> that instructs AutoFixture to omit values for
///     virtual properties. This prevents AutoFixture from trying to populate members that
///     are intended to be provided by a mocking framework or dynamic proxy.
/// </summary>
public class IgnoreVirtualMembersSpecimenBuilder : ISpecimenBuilder
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="IgnoreVirtualMembersSpecimenBuilder" /> class
    ///     that applies to properties on any type.
    /// </summary>
    public IgnoreVirtualMembersSpecimenBuilder()
        : this(null)
    { }

    /// <summary>
    ///     Initializes a new instance of the <see cref="IgnoreVirtualMembersSpecimenBuilder" /> class.
    /// </summary>
    /// <param name="reflectedType">
    ///     Optional filter limiting omission to properties whose <see cref="MemberInfo.ReflectedType" /> equals this type.
    ///     When <c>null</c>, virtual properties across all types are omitted.
    /// </param>
    public IgnoreVirtualMembersSpecimenBuilder(Type? reflectedType) => ReflectedType = reflectedType;

    /// <summary>
    ///     Gets the optional type filter that restricts omission to a particular <see cref="MemberInfo.ReflectedType" />.
    /// </summary>
    public Type? ReflectedType { get; }

    /// <summary>
    ///     Creates a specimen based on the supplied <paramref name="request" />.
    ///     Returns <see cref="OmitSpecimen" /> for virtual properties that match the optional <see cref="ReflectedType" />,
    ///     and <see cref="NoSpecimen" /> otherwise so that the pipeline can continue.
    /// </summary>
    /// <param name="request">The request that describes what to create; often a <see cref="PropertyInfo" />.</param>
    /// <param name="context">The context (not used).</param>
    /// <returns>
    ///     <see cref="OmitSpecimen" /> when the request is a virtual property matching the filter; otherwise <see cref="NoSpecimen" />.
    /// </returns>
    public object Create(object request, ISpecimenContext context)
    {
        if (request is PropertyInfo pi) //// is a property
        {
            if (ReflectedType is not null && //// is hosted anywhere
                ReflectedType != pi.ReflectedType) //// is hosted in a defined type
            {
                return new NoSpecimen();
            }

            if (pi.GetGetMethod().NotNull(nameof(request)).IsVirtual)
            {
                return new OmitSpecimen();
            }
        }

        return new NoSpecimen();
    }
}
