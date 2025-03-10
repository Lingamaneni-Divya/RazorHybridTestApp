using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using IntuneMobilityViolationJob.Common;
using IntuneMobilityViolationJob.Models.GraphAPIServiceModels;
using Microsoft.Data.SqlClient;
using Xunit;

public class TypeConversionsTests
{
    private class TestClass
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    [Fact]
    public void DeSerializeJsonToObject_ShouldDeserialize_ValidJson()
    {
        string json = "{\"Id\":1,\"Name\":\"Alice\"}";
        var result = TypeConversions.DeSerializeJsonToObject<TestClass>(json);

        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Alice", result.Name);
    }

    [Fact]
    public void ValuesToObject_ShouldReturnListOfObjects()
    {
        var schema = new List<Schema>
        {
            new Schema { Column = "Id" },
            new Schema { Column = "Name" }
        };
        var values = new List<List<object>>
        {
            new List<object> { 1, "Alice" },
            new List<object> { 2, "Bob" }
        };

        var result = TypeConversions.ValuesToObject<TestClass>(values, schema);

        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Equal("Alice", result[0].Name);
        Assert.Equal("Bob", result[1].Name);
    }

    [Fact]
    public void ConvertObjectToT_ShouldConvertObjectToType()
    {
        var source = new { Id = 1, Name = "Alice" };
        var result = TypeConversions.ConvertObjectToT<TestClass>(source);

        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Alice", result.Name);
    }

    [Fact]
    public void ConvertObjectListToListT_ShouldConvertObjectArrayToList()
    {
        var source = new object[]
        {
            new { Id = 1, Name = "Alice" },
            new { Id = 2, Name = "Bob" }
        };

        var result = TypeConversions.ConvertObjectListToListT<TestClass>(source);

        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Equal("Alice", result[0].Name);
        Assert.Equal("Bob", result[1].Name);
    }

    [Fact]
    public void ConvertToType_ShouldConvertValuesCorrectly()
    {
        object value = "123";
        var result = TypeConversions.ConvertToType(value, typeof(int));

        Assert.IsType<int>(result);
        Assert.Equal(123, result);
    }

    [Fact]
    public void ConvertIEnumerableToDataTable_ShouldConvertListToDataTable()
    {
        var dataList = new List<TestClass>
        {
            new TestClass { Id = 1, Name = "Alice" },
            new TestClass { Id = 2, Name = "Bob" }
        };

        var dataTable = TypeConversions.ConvertIEnumerableToDataTable(dataList);

        Assert.Equal(2, dataTable.Rows.Count);
        Assert.Equal("Alice", dataTable.Rows[0]["Name"]);
        Assert.Equal(2, dataTable.Rows[1]["Id"]);
    }

    [Fact]
    public void GenerateParameters_ShouldReturnSqlParameters()
    {
        var testObj = new TestClass { Id = 1, Name = "Alice" };
        var parameters = TypeConversions.GenerateParameters(testObj);

        Assert.NotNull(parameters);
        Assert.Equal(2, parameters.Length);
        Assert.Contains(parameters, p => p.ParameterName == "@Id" && (int)p.Value == 1);
        Assert.Contains(parameters, p => p.ParameterName == "@Name" && (string)p.Value == "Alice");
    }

    [Fact]
    public void GetSqlDbType_ShouldReturnCorrectType()
    {
        Assert.Equal(SqlDbType.Int, TypeConversions.GetSqlDbType(typeof(int)));
        Assert.Equal(SqlDbType.VarChar, TypeConversions.GetSqlDbType(typeof(string)));
        Assert.Equal(SqlDbType.DateTime, TypeConversions.GetSqlDbType(typeof(DateTime)));
    }

    [Fact]
    public async Task ConvertDataTableToIEnumerable_ShouldConvertDataTableToList()
    {
        var dataTable = new DataTable();
        dataTable.Columns.Add("Id", typeof(int));
        dataTable.Columns.Add("Name", typeof(string));

        dataTable.Rows.Add(1, "Alice");
        dataTable.Rows.Add(2, "Bob");

        var result = await TypeConversions.ConvertDataTableToIEnumerable<TestClass>(dataTable);

        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.Contains(result, r => r.Name == "Alice");
    }

    [Fact]
    public void BuildUrlWithParameters_ShouldReturnCorrectUrl()
    {
        string baseUrl = "https://example.com";
        var parameters = new Dictionary<string, string> { { "key1", "value1" }, { "key2", "value2" } };

        string result = TypeConversions.BuildUrlWithParameters(baseUrl, parameters);

        Assert.Contains("key1=value1", result);
        Assert.Contains("key2=value2", result);
    }
}