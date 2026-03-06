namespace Ploch.Common.Tests.TestTypes.TestingTypes;

public sealed class Attribute1_1Attribute : Attribute1Attribute
{
    /// <inheritdoc />
    public Attribute1_1Attribute() : base(nameof(Attribute1_1Attribute))
    { }

    public int PropInt2 { get; set; }
}
