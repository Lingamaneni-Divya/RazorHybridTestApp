public class TypeConversionsTests
{
    [Theory]
    [InlineData(typeof(byte), SqlDbType.TinyInt)]
    [InlineData(typeof(short), SqlDbType.SmallInt)]
    [InlineData(typeof(int), SqlDbType.Int)]
    [InlineData(typeof(long), SqlDbType.BigInt)]
    [InlineData(typeof(float), SqlDbType.Real)]
    [InlineData(typeof(double), SqlDbType.Float)]
    [InlineData(typeof(decimal), SqlDbType.Decimal)]
    [InlineData(typeof(Guid), SqlDbType.UniqueIdentifier)]
    [InlineData(typeof(DateTime), SqlDbType.DateTime)]
    [InlineData(typeof(TimeSpan), SqlDbType.Time)]
    [InlineData(typeof(bool), SqlDbType.Bit)]
    [InlineData(typeof(string), SqlDbType.VarChar)]
    [InlineData(typeof(byte[]), SqlDbType.VarBinary)]
    [InlineData(typeof(char), SqlDbType.Char)]
    [InlineData(typeof(DateTimeOffset), SqlDbType.DateTimeOffset)]
    [InlineData(typeof(XmlDocument), SqlDbType.Xml)]
    public void GetSqlDbType_ShouldReturnExpectedSqlDbType_WhenGivenKnownType(Type inputType, SqlDbType expected)
    {
        // Act
        var result = TypeConversions.GetSqlDbType(inputType);

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void GetSqlDbType_ShouldHandleNullableTypes()
    {
        // Arrange
        Type nullableType = typeof(int?);

        // Act
        var result = TypeConversions.GetSqlDbType(nullableType);

        // Assert
        Assert.Equal(SqlDbType.Int, result);  // Should unwrap Nullable<int> to int
    }

    [Fact]
    public void GetSqlDbType_ShouldThrowException_ForUnsupportedType()
    {
        // Arrange
        Type unsupportedType = typeof(List<int>);

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => TypeConversions.GetSqlDbType(unsupportedType));
        Assert.Contains("Unsupported type", exception.Message);
    }
}