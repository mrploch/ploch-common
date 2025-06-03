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
        act.Should().Throw<InvalidOperationException>().WithMessage($"*{nameof(WmiInvalidTestClass)}*WindowsManagementClass*attribute*");
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
                             ("Value 1", TestEnumWithMappings2.Value1), ("value 1", TestEnumWithMappings2.Value1),
                             ("value1", TestEnumWithMappings2.Value1), ("VALUE1", TestEnumWithMappings2.Value1),
                             ("Value 2", TestEnumWithMappings2.Value2), ("value 2", TestEnumWithMappings2.Value2),
                             ("value2", TestEnumWithMappings2.Value2), ("VALUE2", TestEnumWithMappings2.Value2),
                             (null, TestEnumWithMappings2.Value3MappedToNull), ("", TestEnumWithMappings2.Value3MappedToNull)
                         };
        for (var i = 0; i < 10; i++)
        {
            mockObjects.Add(new MockWmiObject(("Name", $"TestObject{i}"),
                                              ("IntValue", i),
                                              ("DateTimeValue",
                                               ManagementDateTimeConverter.ToDmtfDateTime(new DateTime(2000 + i, 1, 1, 12, 0, 0, DateTimeKind.Utc))),
                                              (nameof(WmiTestClass.NullableDateTimeValue),
                                               ManagementDateTimeConverter.ToDmtfDateTime(new DateTime(2001 + i, 1, 1, 12, 0, 0, DateTimeKind.Utc))),
                                              (nameof(WmiTestClass.DateTimeOffsetValue),
                                               ManagementDateTimeConverter.ToDmtfDateTime(new DateTime(2002 + i, 1, 1, 12, 0, 0, DateTimeKind.Utc))),
                                              (nameof(WmiTestClass.NullableDateTimeOffsetValue),
                                               ManagementDateTimeConverter.ToDmtfDateTime(new DateTime(2003 + i, 1, 1, 12, 0, 0, DateTimeKind.Utc))),
                                              ("TestStringValue", $"Test{i}"),
                                              ("TestPropertyWithoutAttribute", $"TestPropertyWithoutAttribute{i}"),
                                              (nameof(WmiTestClass.TestEnumValue) + 1, enumValues[i].Item1)));
        }

        mockQuery.Setup(query => query.Execute(It.Is<string>(s => s == "SELECT * FROM Win32_TestClass"))).Returns(mockObjects);

        var mockFactory = new Mock<IWmiObjectQueryFactory>();
        mockFactory.Setup(factory => factory.Create()).Returns(mockQuery.Object);

        var wmiObjects = mockQuery.Object.GetAll<WmiTestClass>().ToList();

        for (var i = 0; i < 10; i++)
        {
            wmiObjects[i].Name.Should().Be($"TestObject{i}");
            wmiObjects[i].IntValue.Should().Be(i);
            var expectedDateTime0 = GetDate(0, i);
            DateTime? expectedDateTime1 = GetDate(1, i);
            DateTimeOffset expectedDateTimeOffset1 = GetDate(2, i);
            DateTimeOffset? expectedDateTimeOffset2 = GetDate(3, i);

            wmiObjects[i].DateTimeValue.ToUniversalTime().Should().BeCloseTo(expectedDateTime0, TimeSpan.FromMilliseconds(1));

            wmiObjects[i].NullableDateTimeValue.Should().HaveValue();
            wmiObjects[i].NullableDateTimeValue!.Value.Should().BeCloseTo(expectedDateTime1.Value, TimeSpan.FromMilliseconds(1));

            wmiObjects[i].DateTimeOffsetValue.Should().BeCloseTo(expectedDateTimeOffset1, TimeSpan.FromMilliseconds(1));

            wmiObjects[i].NullableDateTimeOffsetValue.Should().HaveValue();
            wmiObjects[i].NullableDateTimeOffsetValue!.Value.Should().BeCloseTo(expectedDateTimeOffset2.Value, TimeSpan.FromMilliseconds(1));

            wmiObjects[i].StringPropertyWithDifferentName.Should().Be($"Test{i}");
            wmiObjects[i].TestPropertyWithoutAttribute.Should().Be($"TestPropertyWithoutAttribute{i}");
            wmiObjects[i].TestEnumValue.Should().Be(enumValues[i].Item2);
        }
    }

    private static DateTime GetDate(int year, int index) => new(2000 + year + index, 1, 1, 12, 0, 0, DateTimeKind.Utc);

    // [Fact]
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