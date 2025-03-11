using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;
using Xunit;
using IntuneMobilityViolationJob.Repository.Command.BaseRepository;

public class CommandRepositoryTests
{
    private readonly CommandRepository _commandRepository;
    private readonly string _testConnectionString = "DataSource=:memory:";

    public CommandRepositoryTests()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "ConnectionStrings:MobilityViolenceWriteDB", _testConnectionString }
            })
            .Build();

        _commandRepository = new CommandRepository(configuration);
    }

    [Fact]
    public async Task ExecuteAsync_Should_Execute_Command_Successfully()
    {
        // Arrange
        string createTableQuery = "CREATE TABLE SampleTable (Id INTEGER PRIMARY KEY, Name TEXT)";
        string insertQuery = "INSERT INTO SampleTable (Name) VALUES ('TestName')";

        using (var connection = new SqliteConnection(_testConnectionString))
        {
            await connection.OpenAsync();
            using (var command = new SqliteCommand(createTableQuery, connection))
            {
                await command.ExecuteNonQueryAsync();
            }
        }

        // Act
        await _commandRepository.ExecuteAsync<object>(insertQuery, CommandType.Text);

        // Assert
        using (var connection = new SqliteConnection(_testConnectionString))
        {
            await connection.OpenAsync();
            using (var command = new SqliteCommand("SELECT COUNT(*) FROM SampleTable", connection))
            {
                var count = (long)await command.ExecuteScalarAsync();
                Assert.Equal(1, count);
            }
        }
    }

    [Fact]
    public async Task ExecuteBatchAsync_Should_Insert_Data()
    {
        // Arrange
        string createTableQuery = "CREATE TABLE SampleTable (Id INTEGER PRIMARY KEY, Name TEXT)";
        string insertQuery = "INSERT INTO SampleTable (Name) VALUES (@Name)";
        var dataList = new List<string> { "Alice", "Bob", "Charlie" };

        using (var connection = new SqliteConnection(_testConnectionString))
        {
            await connection.OpenAsync();
            using (var command = new SqliteCommand(createTableQuery, connection))
            {
                await command.ExecuteNonQueryAsync();
            }
        }

        // Act
        await _commandRepository.ExecuteBatchAsync(insertQuery, CommandType.Text, dataList);

        // Assert
        using (var connection = new SqliteConnection(_testConnectionString))
        {
            await connection.OpenAsync();
            using (var command = new SqliteCommand("SELECT COUNT(*) FROM SampleTable", connection))
            {
                var count = (long)await command.ExecuteScalarAsync();
                Assert.Equal(dataList.Count, count);
            }
        }
    }
}