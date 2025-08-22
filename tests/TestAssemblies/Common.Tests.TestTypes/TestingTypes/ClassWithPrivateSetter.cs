using System;

namespace Ploch.Common.Tests.TestTypes.TestingTypes;

public class ClassWithPrivateSetter
{
    public static readonly string DefaultValue = Guid.NewGuid().ToString();

    public string PropertyWithPrivateSetter { get; private set; } = DefaultValue;
}
