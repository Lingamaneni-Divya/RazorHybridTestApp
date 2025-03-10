using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;
using IntuneMobilityViolationJob.Common;
using Microsoft.Data.SqlClient;

namespace IntuneMobilityViolationJob.Tests.CommonTests
{
    public class TypeConversionsTests
    {
        // 1️⃣ JSON Deserialization - Happy path
        [Fact]
        public void DeSerializeJsonToObject_ShouldReturnValidObject()
        {
            string json = "{\"Id\": 1, \"Name\": \"Test\"}";
            var result = TypeConversions.DeSerializeJsonToObject<TestModel>(json);
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal("Test", result.Name);
        }

        // 2️⃣ JSON Deserialization - Null input
        [Fact]
        public void DeSerializeJsonToObject_NullJson_ShouldReturnNull()
        {
            var result = TypeConversions.DeSerializeJsonToObject<TestModel>(null);
            Assert.Null(result);
        }

        // 3️⃣ Convert Object List to List<T>
        [Fact]
        public void ConvertObjectListToListT_ShouldConvertAllObjects()
        {
            var source = new object[]
            {
                new TestModel { Id = 1, Name = "Alice" },
                new TestModel { Id = 2, Name = "Bob" }
            };

            var result = TypeConversions.ConvertObjectListToListT<TestModel>(source);

            Assert.Equal(2, result.Count);
            Assert.Equal("Alice", result[0].Name);
            Assert.Equal("Bob", result[1].Name);
        }

        // 4️⃣ Convert IEnumerable to DataTable
        [Fact]
        public void ConvertIEnumerableToDataTable_ShouldCreateValidDataTable()
        {
            var list = new List<TestModel>
            {
                new TestModel { Id = 1, Name = "Alice" },
                new TestModel { Id = 2, Name = "Bob" }
            };

            DataTable result = TypeConversions.ConvertIEnumerableToDataTable(list);

            Assert.Equal(2, result.Rows.Count);
            Assert.Equal("Alice", result.Rows[0]["Name"]);
            Assert.Equal("Bob", result.Rows[1]["Name"]);
        }

        // 5️⃣ Convert DataTable to IEnumerable<T>
        [Fact]
        public async Task ConvertDataTableToIEnumerable_ShouldConvertCorrectly()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Id", typeof(int));
            dt.Columns.Add("Name", typeof(string));
            dt.Rows.Add(1, "Alice");
            dt.Rows.Add(2, "Bob");

            var result = await TypeConversions.ConvertDataTableToIEnumerable<TestModel>(dt);

            Assert.Equal(2, result.Count());
            Assert.Equal("Alice", result.First().Name);
            Assert.Equal("Bob", result.Last().Name);
        }

        // 6️⃣ Generate SQL Parameters
        [Fact]
        public void GenerateParameters_ShouldReturnValidSqlParameters()
        {
            var testModel = new TestModel { Id = 1, Name = "Alice" };

            SqlParameter[] parameters = TypeConversions.GenerateParameters(testModel);

            Assert.NotNull(parameters);
            Assert.Equal(2, parameters.Length);
            Assert.Equal("@Id", parameters[0].ParameterName);
            Assert.Equal("@Name", parameters[1].ParameterName);
        }

        // 7️⃣ Get SQL DbType Mapping
        [Fact]
        public void GetSqlDbType_ShouldReturnCorrectType()
        {
            Assert.Equal(SqlDbType.Int, TypeConversions.GetSqlDbType(typeof(int)));
            Assert.Equal(SqlDbType.VarChar, TypeConversions.GetSqlDbType(typeof(string)));
            Assert.Equal(SqlDbType.DateTime, TypeConversions.GetSqlDbType(typeof(DateTime)));
        }

        // 8️⃣ Build URL with Parameters - Happy Path
        [Fact]
        public void BuildUrlWithParameters_ShouldReturnCorrectUrl()
        {
            string baseUrl = "https://example.com/api";
            var parameters = new Dictionary<string, string>
            {
                { "key1", "value1" },
                { "key2", "value2" }
            };

            string result = TypeConversions.BuildUrlWithParameters(baseUrl, parameters);

            Assert.Contains("key1=value1", result);
            Assert.Contains("key2=value2", result);
        }

        // 9️⃣ Build URL - No Parameters
        [Fact]
        public void BuildUrlWithParameters_EmptyParameters_ShouldReturnBaseUrl()
        {
            string baseUrl = "https://example.com/api";
            string result = TypeConversions.BuildUrlWithParameters(baseUrl, null);
            Assert.Equal(baseUrl, result);
        }

        // 🔟 Convert Null Object to Type
        [Fact]
        public void ConvertToType_NullValue_ShouldReturnNull()
        {
            object value = null;
            var result = TypeConversions.ConvertToType(value, typeof(int?));
            Assert.Null(result);
        }

        // ✅ Edge case: Convert unsupported type
        [Fact]
        public void GetSqlDbType_UnsupportedType_ShouldThrowException()
        {
            Assert.Throws<ArgumentException>(() => TypeConversions.GetSqlDbType(typeof(Stream)));
        }
    }

    // ✅ Sample Model for Testing
    public class TestModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}