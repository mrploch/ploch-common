using System.Diagnostics.CodeAnalysis;

#pragma warning disable CS8603 // Possible null reference return.

namespace Ploch.Common.Tests.TestTypes.TestingTypes;

public class TestTypeWithMixedSettersAndGetter
{
    public string? _stringPropNoGetter;

#pragma warning disable 649 //  [CS0649] Field 'TestTypes.TestTypeWithMixedSettersAndGetter._stringPropNoSetter' is never assigned to, and will always have its default value null
    [SuppressMessage("ReSharper", "UnassignedField.Global")]
    public string? _stringPropNoSetter;
#pragma warning restore 649

    public int IntProp { get; set; }

    public string? StringProp { get; set; }

    public int IntProp2 { get; set; }

    public string? StringProp2 { get; set; }

    public string StringPropNoSetter => _stringPropNoSetter;

#pragma warning disable S2376 // Write-only properties should not be used
    public string StringPropNoGetter
#pragma warning restore S2376 // Write-only properties should not be used
    {
        set => _stringPropNoGetter = value;
    }
}
