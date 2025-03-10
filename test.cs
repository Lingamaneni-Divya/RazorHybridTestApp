using IntuneMobilityViolationJob.Repository.Command.BaseRepository;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using Moq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Xunit;

public class CommandRepositoryTests
{
    private readonly CommandRepository _repository;
    private readonly Mock<IConfiguration> _mockConfiguration;
    private readonly string _connectionString = "Server=myServer;Database=TestDB;Trusted_Connection=True;";

    public CommandRepositoryTests()
    {
        _mockConfiguration = new Mock<IConfiguration>();
        _mockConfiguration.Setup(config => config["ConnectionStrings:MobilityViolationDB"]).Returns(_connectionString);

        _repository = new CommandRepository(_mockConfiguration.Object);
    }

    [Fact]
    public async Task ExecuteAsync_Should_Execute_Command_Successfully()
    {
        // Arrange
        string commandText = "UPDATE Users SET Name='Test' WHERE Id=1";
        SqlParameter[] parameters = { new SqlParameter("@Id", 1) };

        // Act
        await _repository.ExecuteAsync(commandText, CommandType.Text, parameters);

        // Assert - No exceptions mean success
        Assert.True(true);
    }

    [Fact]
    public async Task ExecuteAsync_Should_Handle_Exception_And_Rollback()
    {
        // Arrange
        string commandText = "INVALID SQL SYNTAX"; 

        // Act & Assert
        await Assert.ThrowsAsync<SqlException>(() => _repository.ExecuteAsync(commandText, CommandType.Text));
    }

    [Fact]
    public async Task ExecuteBatchAsync_Should_Execute_Batches_Successfully()
    {
        // Arrange
        string commandText = "INSERT INTO Users (Id, Name) VALUES (@Id, @Name)";
        List<TestUser> dataList = new List<TestUser>
        {
            new TestUser { Id = 1, Name = "Alice" },
            new TestUser { Id = 2, Name = "Bob" },
            new TestUser { Id = 3, Name = "Charlie" }
        };

        // Act
        await _repository.ExecuteBatchAsync(commandText, CommandType.Text, dataList);

        // Assert - No exceptions mean success
        Assert.True(true);
    }

    [Fact]
    public async Task ExecuteBatchAsync_Should_Handle_Exception_And_Retry()
    {
        // Arrange
        string commandText = "INVALID SQL SYNTAX"; 
        List<TestUser> dataList = new List<TestUser> { new TestUser { Id = 1, Name = "Alice" } };

        // Act & Assert
        await Assert.ThrowsAsync<SqlException>(() => _repository.ExecuteBatchAsync(commandText, CommandType.Text, dataList));
    }

    private class TestUser
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}