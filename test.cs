using System;
using System.Collections.Generic;
using System.Data;
using System.Text.Json;
using Xunit;

public class TypeConversionsTests
{
    [Theory]
    [InlineData("123", typeof(int), 123)]
    [InlineData("45.67", typeof(double), 45.67)]
    [InlineData("true", typeof(bool), true)]
    [InlineData("A", typeof(char), 'A')]
    [InlineData("100000000000", typeof(long), 100000000000L)]
    [InlineData("3.14", typeof(float), 3.14f)]
    [InlineData("2025-03-05", typeof(DateTime), "2025-03-05")] // Date format may vary
    public void ConvertToType_ShouldConvertPrimitives(string input, Type targetType, object expected)
    {
        var result = TypeConversions.ConvertToType(input, targetType);
        Assert.Equal(Convert.ChangeType(expected, targetType), result);
    }

    [Fact]
    public void ConvertToType_ShouldConvertNullableTypes()
    {
        Assert.Equal(123, TypeConversions.ConvertToType("123", typeof(int?)));
        Assert.Equal(3.14, TypeConversions.ConvertToType("3.14", typeof(double?)));
        Assert.Equal(true, TypeConversions.ConvertToType("true", typeof(bool?)));
        Assert.Null(TypeConversions.ConvertToType(null, typeof(int?)));
    }

    [Fact]
    public void ConvertToType_ShouldConvertEnums()
    {
        Assert.Equal(DayOfWeek.Monday, TypeConversions.ConvertToType("Monday", typeof(DayOfWeek)));
        Assert.Equal(DayOfWeek.Friday, TypeConversions.ConvertToType("5", typeof(DayOfWeek)));
        Assert.Null(TypeConversions.ConvertToType("InvalidDay", typeof(DayOfWeek)));
    }

    [Fact]
    public void ConvertToType_ShouldConvertGuids()
    {
        string guidStr = "4a5d6f7e-8a90-1234-5678-abcdef123456";
        Guid expectedGuid = Guid.Parse(guidStr);

        var result = TypeConversions.ConvertToType(guidStr, typeof(Guid));
        Assert.Equal(expectedGuid, result);

        Assert.Null(TypeConversions.ConvertToType("invalid-guid", typeof(Guid)));
    }

    [Fact]
    public void ConvertToType_ShouldConvertComplexObjects()
    {
        var obj = new { Name = "Divya", Age = 26 };
        string json = JsonSerializer.Serialize(obj);

        var result = TypeConversions.ConvertToType(json, obj.GetType());
        Assert.NotNull(result);
        Assert.Equal(obj.Name, result.GetType().GetProperty("Name")?.GetValue(result));
        Assert.Equal(obj.Age, result.GetType().GetProperty("Age")?.GetValue(result));
    }

    [Fact]
    public void ConvertToType_ShouldReturnNullForInvalidConversion()
    {
        Assert.Null(TypeConversions.ConvertToType("NotANumber", typeof(int)));
        Assert.Null(TypeConversions.ConvertToType("XYZ", typeof(double)));
        Assert.Null(TypeConversions.ConvertToType("InvalidEnumValue", typeof(DayOfWeek)));
    }

    [Fact]
    public void ConvertToType_ShouldReturnSameValueIfAlreadyCorrectType()
    {
        int number = 42;
        string text = "Hello";
        Assert.Equal(number, TypeConversions.ConvertToType(number, typeof(int)));
        Assert.Equal(text, TypeConversions.ConvertToType(text, typeof(string)));
    }
}