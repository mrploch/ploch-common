using System.Diagnostics.CodeAnalysis;

namespace Ploch.Common.Tests.TestTypes.TestingTypes;

[SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Test fixture - intentional shape for reflection/serialization tests")]
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "Test fixture - intentional shape for reflection/serialization tests")]
[SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Test fixture - intentional shape for reflection/serialization tests")]
[SuppressMessage("ReSharper", "NotAccessedField.Local", Justification = "Test fixture - intentional shape for reflection/serialization tests")]
[SuppressMessage("ReSharper", "PropertyCanBeMadeInitOnly.Global", Justification = "Test fixture - intentional shape for reflection/serialization tests")]
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global", Justification = "Test fixture - intentional shape for reflection/serialization tests")]
public enum ByteEnum : byte
{
    Value1 = 1,
    Value2 = 2,
    Value3 = 3
}
