using System;

namespace Ploch.Common.Tests.TestTypes.TestingTypes;

public class Attribute1Attribute(string name) : Attribute
{
    public string Name { get; } = name;

    public int PropInt { get; set; }
}
