using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Ploch.Common.Reflection;

namespace Ploch.Common.Tests.Reflection;

public class ObjectReflectionExtensionsTests
{
    [Theory]
    [AutoData]
    public void GetFieldValue_Generic_should_return_field_private_field_value(string privateFieldValue,
                                                                              int protectedFieldValue,
                                                                              Guid publicFieldValue,
                                                                              string privateStaticFieldValue)
    {
        var testType = new TestType(privateFieldValue, protectedFieldValue, publicFieldValue, privateStaticFieldValue);

        testType.GetFieldValue<string>("_privateField").Should().Be(privateFieldValue);
        testType.GetFieldValue<int>("_protectedField").Should().Be(protectedFieldValue);
        testType.GetFieldValue<Guid>("PublicField").Should().Be(publicFieldValue);
        testType.GetFieldValue<string>("PrivateStaticField").Should().Be(privateStaticFieldValue);
    }

    [Theory]
    [AutoData]
    public void GetFieldValue_NonGeneric_should_return_field_private_field_value(string privateFieldValue,
                                                                                 int protectedFieldValue,
                                                                                 Guid publicFieldValue,
                                                                                 string privateStaticFieldValue)
    {
        var testType = new TestType(privateFieldValue, protectedFieldValue, publicFieldValue, privateStaticFieldValue);

        testType.GetFieldValue("_privateField").Should().Be(privateFieldValue);
        testType.GetFieldValue("_protectedField").Should().Be(protectedFieldValue);
        testType.GetFieldValue("PublicField").Should().Be(publicFieldValue);
        testType.GetFieldValue("PrivateStaticField").Should().Be(privateStaticFieldValue);
    }

    [Theory]
    [AutoData]
    public void GetFieldValues_should_return_dictionary_with_field_names_and_values(string privateFieldValue,
                                                                                    int protectedFieldValue,
                                                                                    Guid publicFieldValue,
                                                                                    string privateStaticFieldValue)
    {
        var testType = new TestType(privateFieldValue, protectedFieldValue, publicFieldValue, privateStaticFieldValue);

        var fieldValues = testType.GetFieldValues(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        fieldValues.Should().ContainKey("_privateField");
        fieldValues.Should().ContainKey("_protectedField");
        fieldValues.Should().ContainKey("PublicField");
        fieldValues["_privateField"].Should().Be(privateFieldValue);
        fieldValues["_protectedField"].Should().Be(protectedFieldValue);
        fieldValues["PublicField"].Should().Be(publicFieldValue);

        // Static field should not be included when using instance binding flags
        fieldValues.Should().NotContainKey("PrivateStaticField");
    }

    [Theory]
    [AutoData]
    public void GetFieldValues_should_include_only_public_instance_fields_by_default(string privateFieldValue,
                                                                                     int protectedFieldValue,
                                                                                     Guid publicFieldValue,
                                                                                     string privateStaticFieldValue)
    {
        var testType = new TestType(privateFieldValue, protectedFieldValue, publicFieldValue, privateStaticFieldValue);

        var fieldValues = testType.GetFieldValues();

        // Should only contain public instance fields by default
        fieldValues.Should().ContainKey("PublicField");
        fieldValues["PublicField"].Should().Be(publicFieldValue);

        // Should not contain private, protected or static fields
        fieldValues.Should().NotContainKey("_privateField");
        fieldValues.Should().NotContainKey("_protectedField");
        fieldValues.Should().NotContainKey("PrivateStaticField");

        // Should only have one entry (the public field)
        fieldValues.Should().HaveCount(1);
    }

    [Theory]
    [AutoData]
    public void GetFieldValues_should_include_private_fields_when_NonPublic_is_specified(string privateFieldValue,
                                                                                         int protectedFieldValue,
                                                                                         Guid publicFieldValue,
                                                                                         string privateStaticFieldValue)
    {
        var testType = new TestType(privateFieldValue, protectedFieldValue, publicFieldValue, privateStaticFieldValue);

        var fieldValues = testType.GetFieldValues(BindingFlags.Instance | BindingFlags.NonPublic);

        // Should contain private and protected instance fields
        fieldValues.Should().ContainKey("_privateField");
        fieldValues.Should().ContainKey("_protectedField");
        fieldValues["_privateField"].Should().Be(privateFieldValue);
        fieldValues["_protectedField"].Should().Be(protectedFieldValue);

        // Should not contain public fields when Public flag is not specified
        fieldValues.Should().NotContainKey("PublicField");

        // Should not contain static fields when only Instance is specified
        fieldValues.Should().NotContainKey("PrivateStaticField");
    }

    [Theory]
    [AutoData]
    public void GetFieldValues_should_include_static_fields_when_Static_is_specified(string privateFieldValue,
                                                                                     int protectedFieldValue,
                                                                                     Guid publicFieldValue,
                                                                                     string privateStaticFieldValue)
    {
        var testType = new TestType(privateFieldValue, protectedFieldValue, publicFieldValue, privateStaticFieldValue);

        var fieldValues = testType.GetFieldValues(BindingFlags.Static | BindingFlags.NonPublic);

        // Should contain private static field when Static flag is specified
        fieldValues.Should().ContainKey("PrivateStaticField");
        fieldValues["PrivateStaticField"].Should().Be(privateStaticFieldValue);

        // Should not contain instance fields when Instance flag is not specified
        fieldValues.Should().NotContainKey("_privateField");
        fieldValues.Should().NotContainKey("_protectedField");
        fieldValues.Should().NotContainKey("PublicField");
    }

    [Fact]
    public void GetFieldValues_should_return_empty_dictionary_for_null_input_and_no_public_static_properties()
    {
        // Arrange
        TestType? nullObject = null;

        // Act
        var fieldValues = nullObject.GetFieldValues();

        // Assert
        fieldValues.Should().NotBeNull();
        fieldValues.Should().BeEmpty();
    }

    [Fact]
    public void GetFieldValues_should_return_empty_dictionary_for_objects_with_no_fields()
    {
        // Arrange
        var objectWithNoFields = new ObjectWithNoFields();

        // Act
        var fieldValues = objectWithNoFields.GetFieldValues();

        // Assert
        fieldValues.Should().NotBeNull();
        fieldValues.Should().BeEmpty();
    }

    [Fact]
    public void GetFieldValues_should_handle_complex_field_types_correctly()
    {
        // Arrange
        var complexObject = new TypeWithComplexFields();

        // Act
        var fieldValues = complexObject.GetFieldValues(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        // Assert
        fieldValues.Should().ContainKey("_listField");
        fieldValues.Should().ContainKey("_dictionaryField");
        fieldValues.Should().ContainKey("_nestedObjectField");

        // Verify the list field
        var listField = fieldValues["_listField"] as List<int>;
        listField.Should().NotBeNull();
        listField.Should().BeEquivalentTo(new List<int> { 1, 2, 3 });

        // Verify the dictionary field
        var dictionaryField = fieldValues["_dictionaryField"] as Dictionary<string, string>;
        dictionaryField.Should().NotBeNull();
        dictionaryField.Should().ContainKey("key1");
        dictionaryField?["key1"].Should().Be("value1");

        // Verify the nested object field
        var nestedField = fieldValues["_nestedObjectField"] as TestType;
        nestedField.Should().NotBeNull();
        nestedField.PublicField.Should().NotBe(Guid.Empty);
    }

    [Fact]
    public void GetFieldValues_should_include_fields_from_parent_classes_when_appropriate_flags_are_set()
    {
        // Arrange
        var childObject = new ChildClass(42, "parentValue", "childValue");

        // Act
        var allFields = childObject.GetFieldValues(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        var publicFields = childObject.GetFieldValues(); // Default is Instance | Public

        // Assert
        // Should include fields from both child and parent classes
        allFields.Should().ContainKey("_childPrivateField");
        allFields.Should().ContainKey("_parentPrivateField");
        allFields.Should().ContainKey("ParentPublicField");

        allFields["_childPrivateField"].Should().Be("childValue");
        allFields["_parentPrivateField"].Should().Be("parentValue");
        allFields["ParentPublicField"].Should().Be(42);

        // Public fields only should only include the public field from parent
        publicFields.Should().ContainKey("ParentPublicField");
        publicFields.Should().NotContainKey("_childPrivateField");
        publicFields.Should().NotContainKey("_parentPrivateField");
        publicFields.Should().HaveCount(1);
    }

    [Fact(Skip = "TODO: Needs fixing")] //TODO: Fix this test
    public void GetFieldValues_should_throw_appropriate_exception_when_invalid_binding_flags_are_provided()
    {
        // Arrange
        var testType = new TestType("test", 42, Guid.NewGuid(), "static");

        // We'll use an invalid binding flag value
        const BindingFlags invalidFlags = (BindingFlags)999999;

        // Act & Assert
        // The method should throw an ArgumentException or similar when invalid flags are provided
        FluentActions.Invoking(() => testType.GetFieldValues(invalidFlags))
                     .Should()
                     .Throw<ArgumentException>()
                     .WithMessage("*binding*"); // The exception message should contain information about binding flags
    }

    [Fact]
    public void GetStaticFieldValues_should_return_only_static_fields()
    {
        var staticStrField1 = Guid.NewGuid().ToString();
        var staticStrField2 = Guid.NewGuid().ToString();
        var staticIntField1 = 1;
        var staticIntField2 = 2;

        ClassWithStaticMembers.StaticStrField1 = staticStrField1;
        ClassWithStaticMembers.StaticStrField2 = staticStrField2;
        ClassWithStaticMembers.StaticIntField1 = staticIntField1;
        ClassWithStaticMembers.StaticIntField2 = staticIntField2;

        var staticFieldValues = TypeHelper.GetStaticFieldValues<ClassWithStaticMembers>(BindingFlags.Public | BindingFlags.NonPublic);

        staticFieldValues.Should().HaveCount(6);
        staticFieldValues.Should()
                         .Contain(new KeyValuePair<string, object?>(nameof(ClassWithStaticMembers.StaticStrField1), staticStrField1),
                                  new KeyValuePair<string, object?>(nameof(ClassWithStaticMembers.StaticStrField2), staticStrField2),
                                  new KeyValuePair<string, object?>(nameof(ClassWithStaticMembers.StaticIntField1), staticIntField1),
                                  new KeyValuePair<string, object?>(nameof(ClassWithStaticMembers.StaticIntField2), staticIntField2),
                                  new KeyValuePair<string, object?>("StaticField", "static field value"),
                                  new KeyValuePair<string, object?>("<StaticProperty>k__BackingField", "static property value"));
    }

    [Fact]
    public void GetMemberValues_should_return_both_fields_and_properties_when_requested()
    {
        // Arrange
        var testObject = new ClassWithFieldsAndProperties { PublicProperty = "property value" };

        // Act
        var memberValues = testObject.GetMemberValues();

        // Assert
        memberValues.Should().ContainKey("PublicField");
        memberValues.Should().ContainKey("PublicProperty");
        memberValues["PublicField"].Should().Be("field value");
        memberValues["PublicProperty"].Should().Be("property value");

        // Verify it doesn't include private members with default flags
        memberValues.Should().NotContainKey("_privateField");
        memberValues.Should().NotContainKey("PrivateProperty");
    }

    [Fact]
    public void GetMemberValues_should_return_only_fields_when_MemberTypes_is_set_to_Field()
    {
        // Arrange
        var testObject = new ClassWithFieldsAndProperties { PublicProperty = "property value" };

        // Act
        var memberValues = testObject.GetMemberValues(BindingFlags.Instance | BindingFlags.Public, MemberTypes.Field);

        // Assert
        memberValues.Should().ContainKey("PublicField");
        memberValues["PublicField"].Should().Be("field value");

        // Verify properties are not included when MemberTypes.Field is specified
        memberValues.Should().NotContainKey("PublicProperty");

        // Verify it only contains the expected number of items (just the public field)
        memberValues.Should().HaveCount(1);
    }

    [Fact]
    public void GetMemberValues_should_return_only_properties_when_MemberTypes_is_set_to_Property()
    {
        // Arrange
        var testObject = new ClassWithFieldsAndProperties { PublicProperty = "property value" };

        // Act
        var memberValues = testObject.GetMemberValues(BindingFlags.Instance | BindingFlags.Public, MemberTypes.Property);

        // Assert
        memberValues.Should().ContainKey("PublicProperty");
        memberValues["PublicProperty"].Should().Be("property value");

        // Verify fields are not included when MemberTypes.Property is specified
        memberValues.Should().NotContainKey("PublicField");

        // Verify it only contains the expected number of items (just the public property)
        memberValues.Should().HaveCount(1);
    }

    [Fact]
    public void GetMemberValues_should_handle_null_input_and_return_only_static_members()
    {
        // Arrange
        ClassWithStaticMembers? nullObject = null;

        // Act
        var memberValues = nullObject.GetMemberValues(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);

        // Assert
        memberValues.Should().NotBeNull();
        memberValues.Should().ContainKey("StaticField");
        memberValues.Should().ContainKey("StaticProperty");
        memberValues["StaticField"].Should().Be("static field value");
        memberValues["StaticProperty"].Should().Be("static property value");

        // Should not contain instance members when object is null
        memberValues.Should().NotContainKey("PublicField");
        memberValues.Should().NotContainKey("PublicProperty");
    }

    [Fact]
    public void GetMemberValues_should_correctly_handle_mixed_fields_and_properties_from_parent_and_child_classes()
    {
        // Arrange
        var childObject = new ChildWithProperties("parent property", "child property", 42, "parent field", "child field");

        // Act
        var allMembers = childObject.GetMemberValues(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        var publicMembers = childObject.GetMemberValues(); // Default is Instance | Public

        // Assert
        // Should include fields and properties from both child and parent classes
        allMembers.Should().ContainKey("_childPrivateField");
        allMembers.Should().ContainKey("_parentPrivateField");
        allMembers.Should().ContainKey("ChildPublicProperty");
        allMembers.Should().ContainKey("ParentPublicProperty");
        allMembers.Should().ContainKey("_childPrivateProperty");
        allMembers.Should().ContainKey("_parentPrivateProperty");

        allMembers["_childPrivateField"].Should().Be("child field");
        allMembers["_parentPrivateField"].Should().Be("parent field");
        allMembers["ChildPublicProperty"].Should().Be("child property");
        allMembers["ParentPublicProperty"].Should().Be("parent property");
        allMembers["ChildPublicValue"].Should().Be(42);

        // Public members only should only include the public properties and fields
        publicMembers.Should().ContainKey("ChildPublicProperty");
        publicMembers.Should().ContainKey("ParentPublicProperty");
        publicMembers.Should().ContainKey("ChildPublicValue");

        // Should not contain private members
        publicMembers.Should().NotContainKey("_childPrivateField");
        publicMembers.Should().NotContainKey("_parentPrivateField");
        publicMembers.Should().NotContainKey("_childPrivateProperty");
        publicMembers.Should().NotContainKey("_parentPrivateProperty");
    }

    [Fact]
    public void GetMemberValues_should_correctly_return_indexed_properties()
    {
        //TODO
        // Arrange
        var testObject = new ClassWithIndexedProperty2();
        testObject["key1"] = "value1";
        testObject["key2"] = "value2";

        // Act
        var memberValues = testObject.GetMemberValues();

        // Assert
        memberValues.Should().ContainKey("Item");

        // Verify we can get the indexed property value
        var indexedProperty = memberValues["Item"] as PropertyInfo;
        indexedProperty.Should().NotBeNull();

        // Verify indexed property's GetValue works with index parameters
        var index0Value = indexedProperty?.GetValue(testObject, ["key1"]);
        var index1Value = indexedProperty?.GetValue(testObject, ["key2"]);

        index0Value.Should().Be("value1");
        index1Value.Should().Be("value2");
    }

    [SuppressMessage("ReSharper", "UnusedMember.Local", Justification = "Used for reflection tests")]
    private class ClassWithIndexedProperty
    {
        private readonly Dictionary<int, string> _storage = new();

        public string this[int index]
        {
            get => _storage.TryGetValue(index, out var value) ? value : string.Empty;
            set => _storage[index] = value;
        }
    }

    [SuppressMessage("ReSharper", "UnusedMember.Local", Justification = "Used for reflection tests")]
    private class ClassWithIndexedProperty2
    {
        private new readonly Dictionary<string, string> _dict = new();

        public string this[string key]
        {
            get => _dict[key];
            set => _dict[key] = value;
        }
    }

    [SuppressMessage("ReSharper", "UnusedMember.Local", Justification = "Used for reflection tests")]
    private class ClassWithIndexedWriteOnlyProperty
    {
        private readonly int[,] _numbers = new int[100, 100];
        private int[,] _itmWriteOnly = new int[100, 100];

        public int[,] Itm { get; set; } = new int[100, 100];

        public int[,] ItmWriteOnly
        {
            set => _itmWriteOnly = value;
        }

        public int this[int x, int y]
        {
            set => _numbers[x, y] = value;
        }
    }

    [SuppressMessage("ReSharper", "NotAccessedField.Local", Justification = "Fields are accessed via reflection")]
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:Fields should be private", Justification = "This is required for this test.")]
    private class ParentWithProperties
    {
        private readonly string _parentPrivateField;

        protected ParentWithProperties(string parentPublicProperty, int childPublicValue, string parentPrivateField)
        {
            ParentPublicProperty = parentPublicProperty;
            _parentPrivateField = parentPrivateField;
            _parentPrivateProperty = "parent private property";
        }

        private string _parentPrivateProperty { get; }

        public string ParentPublicProperty { get; }
    }

    [SuppressMessage("ReSharper", "NotAccessedField.Local", Justification = "Fields are accessed via reflection")]
    private class ChildWithProperties : ParentWithProperties
    {
        private readonly string _childPrivateField;

        public ChildWithProperties(string parentPublicProperty,
                                   string childPublicProperty,
                                   int childPublicValue,
                                   string parentPrivateField,
                                   string childPrivateField) : base(parentPublicProperty, childPublicValue, parentPrivateField)
        {
            ChildPublicProperty = childPublicProperty;
            ChildPublicValue = childPublicValue;
            _childPrivateField = childPrivateField;
            _childPrivateProperty = "child private property";
        }

        private string _childPrivateProperty { get; }

        public string ChildPublicProperty { get; }

        public int ChildPublicValue { get; }
    }

    [SuppressMessage("ReSharper", "NotAccessedField.Local", Justification = "Fields are accessed via reflection")]
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:Fields should be private", Justification = "This is required for this test.")]
    private class ClassWithStaticMembers
    {
        private static readonly string StaticField = "static field value";

        public static readonly string StaticStrField1 = nameof(StaticStrField1) + "Value";
        public static readonly string StaticStrField2 = nameof(StaticStrField2) + "Value";
        public static readonly int StaticIntField1 = 1;
        public static readonly int StaticIntField2 = 2;
        public readonly string PublicField = "instance field value";

        public static string StaticProperty { get; } = "static property value";

        public string PublicProperty { get; set; } = "instance property value";
    }

    [SuppressMessage("ReSharper", "NotAccessedField.Local", Justification = "Fields are accessed via reflection")]
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:Fields should be private", Justification = "This is required for this test.")]
    [SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1202:Elements should be ordered by access", Justification = "It doesn't matter for this test.")]
    private class TestType
    {
        private static string? PrivateStaticField;
        private readonly string _privateField;
        public readonly Guid PublicField;
        protected int _protectedField;

        public TestType(string privateField, int protectedField, Guid publicField, string privateStaticFieldValue)
        {
            _privateField = privateField;
            _protectedField = protectedField;
            PublicField = publicField;
            PrivateStaticField = privateStaticFieldValue;
        }
    }

    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Local", Justification = "Used for reflection tests")]
    private class ObjectWithNoFields
    {
        // This class has no fields, only a property
        public string SomeProperty { get; set; } = string.Empty;
    }

    [SuppressMessage("ReSharper", "NotAccessedField.Local", Justification = "Fields are accessed via reflection")]
    private class TypeWithComplexFields
    {
#pragma warning disable S1144
        private readonly List<int> _listField = [1, 2, 3];
        private readonly Dictionary<string, string> _dictionaryField = new() { { "key1", "value1" } };
        private readonly TestType _nestedObjectField = new("private value", 42, Guid.NewGuid(), "static value");
#pragma warning restore S1144
    }

    [SuppressMessage("ReSharper", "NotAccessedField.Local", Justification = "Fields are accessed via reflection")]
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:Fields should be private", Justification = "This is required for this test.")]
    private class ParentClass
    {
        private readonly string _parentPrivateField;
        public readonly int ParentPublicField;

        protected ParentClass(int publicValue, string privateValue)
        {
            ParentPublicField = publicValue;
            _parentPrivateField = privateValue;
        }
    }

    [SuppressMessage("ReSharper", "NotAccessedField.Local", Justification = "Fields are accessed via reflection")]
    private class ChildClass : ParentClass
    {
        private readonly string _childPrivateField;

        public ChildClass(int parentPublicValue, string parentPrivateValue, string childPrivateValue) : base(parentPublicValue, parentPrivateValue) =>
            _childPrivateField = childPrivateValue;
    }

    [SuppressMessage("ReSharper", "NotAccessedField.Local", Justification = "Fields are accessed via reflection")]
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:Fields should be private", Justification = "This is required for this test.")]
    private class ClassWithFieldsAndProperties
    {
        private readonly string _privateField = "private field value";
        public readonly string PublicField = "field value";

        public string PublicProperty { get; set; } = string.Empty;

        private string PrivateProperty { get; set; } = "private property value";
    }
}
