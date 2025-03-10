public class TypeConversionsTests
{
    [Fact]
    public void ConvertObjectToT_ValidConversion_ShouldReturnConvertedObject()
    {
        // Arrange
        var source = new SourceModel
        {
            Id = 1,
            Name = "Alice",
            Tags = new List<string> { "Tag1", "Tag2" }
        };

        // Act
        var result = TypeConversions.ConvertObjectToT<TargetModel>(source);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Alice", result.Name);
        Assert.Equal("Tag1,Tag2", result.Tags); // ✅ Should be converted to CSV format
    }

    [Fact]
    public void ConvertObjectToT_SourceHasMoreProperties_ShouldIgnoreExtraProperties()
    {
        // Arrange
        var source = new
        {
            Id = 1,
            Name = "Bob",
            ExtraField = "Ignore me"
        };

        // Act
        var result = TypeConversions.ConvertObjectToT<TargetModel>(source);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Bob", result.Name);
        Assert.Null(result.Tags); // ✅ Tags property is missing, so it should be null
    }

    [Fact]
    public void ConvertObjectToT_SourceHasMissingProperties_ShouldNotThrow()
    {
        // Arrange
        var source = new { Id = 42 }; // Missing Name and Tags

        // Act
        var result = TypeConversions.ConvertObjectToT<TargetModel>(source);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(42, result.Id);
        Assert.Null(result.Name);
        Assert.Null(result.Tags);
    }

    [Fact]
    public void ConvertObjectToT_NullSource_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => TypeConversions.ConvertObjectToT<TargetModel>(null));
    }

    [Fact]
    public void ConvertObjectToT_PropertyTypeMismatch_ShouldNotAssignValue()
    {
        // Arrange
        var source = new { Id = "NotAnInt", Name = "Test" };

        // Act & Assert
        Assert.Throws<InvalidCastException>(() => TypeConversions.ConvertObjectToT<TargetModel>(source));
    }

    [Fact]
    public void ConvertObjectToT_EnumerableToStringConversion_ShouldWork()
    {
        // Arrange
        var source = new SourceModel
        {
            Id = 5,
            Name = "Eve",
            Tags = new List<string> { "X", "Y", "Z" }
        };

        // Act
        var result = TypeConversions.ConvertObjectToT<TargetModel>(source);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("X,Y,Z", result.Tags); // ✅ List converted to CSV string
    }
}