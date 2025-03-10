[Fact]
public void ConvertToType_ShouldReturnNull_WhenValueIsNull()
{
    // Act
    var result = TypeConversions.ConvertToType(null, typeof(int));

    // Assert
    Assert.Null(result);
}