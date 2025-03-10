public class SampleClass
{
    public int Id { get; set; }
    public string Name { get; set; }
}

[Fact]
public void ConvertToType_ShouldSerializeAndDeserialize_WhenTargetTypeIsAClass()
{
    // Arrange
    var sampleObj = new SampleClass { Id = 1, Name = "Test" };

    // Act
    var result = TypeConversions.ConvertToType(sampleObj, typeof(SampleClass)) as SampleClass;

    // Assert
    Assert.NotNull(result);
    Assert.Equal(sampleObj.Id, result!.Id);
    Assert.Equal(sampleObj.Name, result.Name);
}

[Fact]
public void ConvertToType_ShouldSerializeAndDeserialize_WhenValueIsDictionary()
{
    // Arrange
    var dict = new Dictionary<string, int> { { "key", 42 } };

    // Act
    var result = TypeConversions.ConvertToType(dict, typeof(Dictionary<string, int>)) as Dictionary<string, int>;

    // Assert
    Assert.NotNull(result);
    Assert.True(result!.ContainsKey("key"));
    Assert.Equal(42, result["key"]);
}