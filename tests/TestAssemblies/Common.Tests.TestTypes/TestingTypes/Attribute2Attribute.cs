using System;

namespace Ploch.Common.Tests.TestTypes.TestingTypes;

[AttributeUsage(AttributeTargets.Class)]
public class Attribute2Attribute(string name) : Attribute
{
    public string Name { get; } = name;

    public int PropInt { get; set; }

    public string Test() => nameof(Test);
}
