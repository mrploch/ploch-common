using System;

namespace Ploch.Common.Tests.TestTypes.TestingTypes;

public class ClassWithFieldsAndProperties
{
    public bool BoolField = true;

    public DateTime DateTimeField = DateTime.Now;

    public decimal DecimalField = 19.99m;

    public double DoubleField = 3.14;

    public Guid GuidField = Guid.NewGuid();
    public int IntField = 42;

    public string StringField = "Hello, World!";

    public bool BoolFieldProperty
    {
        get => BoolField;
        set => BoolField = value;
    }

    public DateTime DateTimeFieldProperty
    {
        get => DateTimeField;
        set => DateTimeField = value;
    }

    public decimal DecimalFieldProperty
    {
        get => DecimalField;
        set => DecimalField = value;
    }

    public double DoubleFieldProperty
    {
        get => DoubleField;
        set => DoubleField = value;
    }

    public Guid GuidFieldProperty
    {
        get => GuidField;
        set => GuidField = value;
    }

    public int IntFieldProperty
    {
        get => IntField;
        set => IntField = value;
    }

    public string StringFieldProperty
    {
        get => StringField;
        set => StringField = value;
    }
}
