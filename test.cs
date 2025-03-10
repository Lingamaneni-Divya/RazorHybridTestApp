public class TypeConversionsTests
{
    [Theory]
    [InlineData(null, typeof(int?), null)] // ✅ Null value
    [InlineData("123", typeof(int), 123)] // ✅ String to Int
    [InlineData("true", typeof(bool), true)] // ✅ String to Bool
    [InlineData("3.14", typeof(double), 3.14)] // ✅ String to Double
    [InlineData("2024-03-05", typeof(DateTime), "2024-03-05")] // ✅ String to DateTime
    public void ConvertToType_ShouldConvertSimpleTypes(object input, Type targetType, object expected)
    {
        // Act
        var result = TypeConversions.ConvertToType(input, targetType);

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void ConvertToType_ShouldConvertNullableType()
    {
        // Arrange
        object input = "42";

        // Act
        var result = TypeConversions.ConvertToType(input, typeof(int?));

        // Assert
        Assert.NotNull(result);
        Assert.IsType<int>(result);
        Assert.Equal(42, result);
    }

    [Fact]
    public void ConvertToType_ShouldConvertEnum()
    {
        // Arrange
        object input = "Friday";

        // Act
        var result = TypeConversions.ConvertToType(input, typeof(DayOfWeek));

        // Assert
        Assert.NotNull(result);
        Assert.IsType<DayOfWeek>(result);
        Assert.Equal(DayOfWeek.Friday, result);
    }

    [Fact]
    public void ConvertToType_InvalidEnumValue_ShouldReturnNull()
    {
        // Arrange
        object input = "InvalidDay";

        // Act
        var result = TypeConversions.ConvertToType(input, typeof(DayOfWeek));

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void ConvertToType_ShouldConvertGuid()
    {
        // Arrange
        object input = "3F2504E0-4F89-11D3-9A0C-0305E82C3301";

        // Act
        var result = TypeConversions.ConvertToType(input, typeof(Guid));

        // Assert
        Assert.NotNull(result);
        Assert.IsType<Guid>(result);
        Assert.Equal(Guid.Parse("3F2504E0-4F89-11D3-9A0C-0305E82C3301"), result);
    }

    [Fact]
    public void ConvertToType_InvalidGuid_ShouldReturnNull()
    {
        // Arrange
        object input = "Invalid-Guid";

        // Act
        var result = TypeConversions.ConvertToType(input, typeof(Guid));

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void ConvertToType_ShouldHandleConvertibleType()
    {
        // Arrange
        object input = "456";

        // Act
        var result = TypeConversions.ConvertToType(input, typeof(int));

        // Assert
        Assert.NotNull(result);
        Assert.IsType<int>(result);
        Assert.Equal(456, result);
    }

    [Fact]
    public void ConvertToType_ShouldReturnSameType_WhenAlreadyMatching()
    {
        // Arrange
        object input = 789;

        // Act
        var result = TypeConversions.ConvertToType(input, typeof(int));

        // Assert
        Assert.Equal(789, result);
    }

    [Fact]
    public void ConvertToType_ShouldSerializeAndDeserializeComplexType()
    {
        // Arrange
        var input = new { Name = "Alice", Age = 30 };

        // Act
        var result = TypeConversions.ConvertToType(input, input.GetType());

        // Assert
        Assert.NotNull(result);
        Assert.Equal(JsonSerializer.Serialize(input), JsonSerializer.Serialize(result));
    }

    [Fact]
    public void ConvertToType_ShouldReturnNull_WhenConversionFails()
    {
        // Arrange
        object input = "NotANumber";

        // Act
        var result = TypeConversions.ConvertToType(input, typeof(int));

        // Assert
        Assert.Null(result);
    }
}