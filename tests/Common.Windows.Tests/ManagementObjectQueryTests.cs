using System.Management;
using System.Reflection;
using FluentAssertions;
using Moq;
using Ploch.Common.Windows.Tests.Wmi.ManagementObjects.TestClasses;
using Ploch.Common.Windows.Wmi;
using Ploch.Common.Windows.Wmi.ManagementObjects.TypeConversion;

namespace Ploch.Common.Windows.Tests;

// Class to test ManagementObjectQuery
public class ManagementObjectQueryTests
{
    [Fact]
    public void GetAll_ShouldThrowInvalidOperationException_WhenTypeLacksWindowsManagementClassAttribute()
    {
        var mockQuery = new Mock<IWmiQuery>();
        // Act
        Action act = () => mockQuery.Object.GetAll<WmiInvalidTestClass>().ToList();

        // Assert
        act.Should().Throw<InvalidOperationException>().WithMessage($"Type {nameof(WmiInvalidTestClass)} does not have the WindowsManagementClass attribute.");
    }

    [Fact]
    public void GetAll_ShouldReturnCorrectResults_WhenValidTypeProvided()
    {
        // Arrange
        var mockQuery = new Mock<IWmiQuery>();
        var mockObjects = new List<MockWmiObject>();
        for (var i = 0; i < 10; i++)
        {
            mockObjects.Add(new MockWmiObject(("Name", $"TestObject{i}"),
                                              ("IntValue", i),
                                              ("DateTimeValue", new DateTime(2000 + i, 1, 1, 12, 0, 0, DateTimeKind.Utc)),
                                              ("TestStringValue", $"Test{i}"),
                                              ("TestPropertyWithoutAttribute", $"TestPropertyWithoutAttribute{i}")));
        }

        mockQuery.Setup(query => query.Execute(It.Is<string>(s => s == "SELECT * FROM Win32_TestClass"))).Returns(mockObjects);

        var mockFactory = new Mock<IWmiObjectQueryFactory>();
        mockFactory.Setup(factory => factory.Create()).Returns(mockQuery.Object);

        var wmiObjects = mockQuery.Object.GetAll<WmiTestClass>().ToList();

        for (var i = 0; i < 10; i++)
        {
            wmiObjects[i].Name.Should().Be($"TestObject{i}");
            wmiObjects[i].IntValue.Should().Be(i);
            wmiObjects[i].DateTimeValue.Should().Be(new DateTime(2000 + i, 1, 1, 12, 0, 0, DateTimeKind.Utc));
            wmiObjects[i].StringPropertyWithDifferentName.Should().Be($"Test{i}");
            wmiObjects[i].TestPropertyWithoutAttribute.Should().Be($"TestPropertyWithoutAttribute{i}");
        }
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnCorrectResults_WhenValidTypeProvided()
    {
        // Arrange
        var mockQuery = new Mock<IWmiQuery>();
        var mockObjects = new List<MockWmiObject>();
        var enumValues = new[]
                         {
                             "Value 1", "value 1", "value1", "VALUE1", "Value 2", "value 2", "value2", "VALUE2", null, ""
                         };
        for (var i = 0; i < 10; i++)
        {
            mockObjects.Add(new MockWmiObject(("Name", $"TestObject{i}"),
                                              ("IntValue", i),
                                              ("DateTimeValue",
                                               ManagementDateTimeConverter.ToDmtfDateTime(new DateTime(2000 + i, 1, 1, 12, 0, 0, DateTimeKind.Utc))),
                                              ("TestStringValue", $"Test{i}"),
                                              ("TestPropertyWithoutAttribute", $"TestPropertyWithoutAttribute{i}"),
                                              (nameof(WmiTestClass.TestEnumValue), enumValues[i])));
        }

        mockQuery.Setup(query => query.Execute(It.Is<string>(s => s == "SELECT * FROM Win32_TestClass"))).Returns(mockObjects);

        var mockFactory = new Mock<IWmiObjectQueryFactory>();
        mockFactory.Setup(factory => factory.Create()).Returns(mockQuery.Object);

        var wmiObjects = mockQuery.Object.GetAll<WmiTestClass>().ToList();

        for (var i = 0; i < 10; i++)
        {
            wmiObjects[i].Name.Should().Be($"TestObject{i}");
            wmiObjects[i].IntValue.Should().Be(i);
            var expectedDateTime = new DateTime(2000 + i, 1, 1, 12, 0, 0);

            wmiObjects[i].DateTimeValue.ToUniversalTime().Should().BeCloseTo(expectedDateTime, TimeSpan.FromMilliseconds(1));
            wmiObjects[i].StringPropertyWithDifferentName.Should().Be($"Test{i}");
            wmiObjects[i].TestPropertyWithoutAttribute.Should().Be($"TestPropertyWithoutAttribute{i}");
            wmiObjects[i].TestEnumValue.Should().Be((TestEnum)i);
        }
    }

    [Fact]
    public void MyMethod()
    {
        var targetType = typeof(TestEnum);
        var enumType = Nullable.GetUnderlyingType(targetType) ?? targetType;
        var fields = enumType.GetFields(BindingFlags.Public | BindingFlags.Static);

        var fieldMap = new Dictionary<string, FieldInfo>(StringComparer.OrdinalIgnoreCase);
        foreach (var fieldInfo in fields)
        {
            var enumMappingAttribute = fieldInfo.GetCustomAttribute<WindowsManagementObjectEnumMappingAttribute>();

            foreach (var name in enumMappingAttribute.Names)
            {
                if (name == null)
                {
                    fieldMap[string.Empty] = fieldInfo;
                }
                else
                {
                    fieldMap[name] = fieldInfo;
                }
            }
        }

        var enumValues = new[]
                         {
                             "Value 1", "value 1", "value1", "VALUE1", "Value 2", "value 2", "value2", "VALUE2", null, ""
                         };

        foreach (var enumValue in enumValues)
        {
            var enVal = fieldMap[enumValue == null ? string.Empty : enumValue].GetValue(null)!;

            var en = (TestEnum)enVal;
        }
    }


    // A test class with the required WindowsManagementClass attribute
}

public class MockWmiObject : IWmiObject
{
    private readonly IDictionary<string, object?> _properties;

    public MockWmiObject(params IEnumerable<(string, object?)> properties) => _properties = properties.ToDictionary(x => x.Item1, x => x.Item2);

    public object? this[string propertyName] => _properties[propertyName];

    public object? GetPropertyValue(string propertyName) => _properties[propertyName];

    public IEnumerable<string> GetPropertyNames() => _properties.Keys;

    public IEnumerable<(string, object?)> GetProperties() => _properties.Select(pair => (pair.Key, pair.Value));
}
