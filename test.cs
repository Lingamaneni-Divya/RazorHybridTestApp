public class UnrecognizedType
{
    public int A { get; set; }
    public string B { get; set; }
}

[Fact]
public void ConvertToType_ShouldSerializeAndDeserialize_WhenObjectIsUnrecognizedType()
{
    // Arrange
    var obj = new UnrecognizedType { A = 10, B = "Hello" };

    // Act
    var result = TypeConversions.ConvertToType(obj, typeof(UnrecognizedType)) as UnrecognizedType;

    // Assert
    Assert.NotNull(result);
    Assert.Equal(obj.A, result!.A);
    Assert.Equal(obj.B, result.B);
}