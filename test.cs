[Fact]
public void ValuesToObject_ValidInput_ShouldReturnListOfObjects()
{
    // Arrange
    var values = new List<List<object>>
    {
        new() { 1, "Alice" },
        new() { 2, "Bob" }
    };
    
    var schema = new List<Schema>
    {
        new Schema { Column = "Id" },
        new Schema { Column = "Name" }
    };

    // Act
    var result = TypeConversions.ValuesToObject<TestModel>(values, schema);

    // Assert
    Assert.NotNull(result);
    Assert.Equal(2, result.Count);
    Assert.Equal(1, result[0].Id);
    Assert.Equal("Alice", result[0].Name);
    Assert.Equal(2, result[1].Id);
    Assert.Equal("Bob", result[1].Name);
}

[Fact]
public void ValuesToObject_NullSchema_ShouldReturnNull()
{
    // Arrange
    var values = new List<List<object>> { new() { 1, "Alice" } };

    // Act
    var result = TypeConversions.ValuesToObject<TestModel>(values, null);

    // Assert
    Assert.Null(result);
}

[Fact]
public void ValuesToObject_NullValues_ShouldReturnNull()
{
    // Arrange
    var schema = new List<Schema> { new Schema { Column = "Id" } };

    // Act
    var result = TypeConversions.ValuesToObject<TestModel>(null, schema);

    // Assert
    Assert.Null(result);
}

[Fact]
public void ValuesToObject_EmptyValues_ShouldReturnEmptyList()
{
    // Arrange
    var values = new List<List<object>>();
    var schema = new List<Schema> { new Schema { Column = "Id" } };

    // Act
    var result = TypeConversions.ValuesToObject<TestModel>(values, schema);

    // Assert
    Assert.NotNull(result);
    Assert.Empty(result);
}

[Fact]
public void ValuesToObject_MissingSchemaColumn_ShouldSkipProperty()
{
    // Arrange
    var values = new List<List<object>> { new() { 1, "Alice" } };
    var schema = new List<Schema> { new Schema { Column = "Id" } }; // Missing "Name"

    // Act
    var result = TypeConversions.ValuesToObject<TestModel>(values, schema);

    // Assert
    Assert.NotNull(result);
    Assert.Single(result);
    Assert.Equal(1, result[0].Id);
    Assert.Null(result[0].Name); // Name property is missing in schema
}