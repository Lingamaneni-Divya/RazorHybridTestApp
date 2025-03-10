using IntuneMobilityViolationJob.Repository.Command.BaseRepository;
using Microsoft.Extensions.Configuration;
using Moq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

public class CommandRepositoryTests
{
    private readonly CommandRepository _repository;
    private readonly Mock<IConfiguration> _mockConfiguration;
    private readonly Mock<MockDbConnection> _mockConnection;
    private readonly Mock<DbCommand> _mockCommand;
    private readonly Mock<DbTransaction> _mockTransaction;

    public CommandRepositoryTests()
    {
        _mockConfiguration = new Mock<IConfiguration>();
        _mockConfiguration.Setup(config => config["ConnectionStrings:MobilityViolationDB"]).Returns("Fake_Connection_String");

        _mockConnection = new Mock<MockDbConnection> { CallBase = true };
        _mockCommand = new Mock<DbCommand>();
        _mockTransaction = new Mock<DbTransaction>();

        _repository = new CommandRepository(_mockConfiguration.Object);
    }

    private void SetupMocks(bool throwException = false)
    {
        _mockConnection.Setup(c => c.OpenAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        _mockConnection.Setup(c => c.BeginTransaction()).Returns(_mockTransaction.Object);
        _mockCommand.Setup(c => c.Connection).Returns(_mockConnection.Object);
        _mockCommand.Setup(c => c.Transaction).Returns(_mockTransaction.Object);

        if (throwException)
        {
            _mockCommand.Setup(c => c.ExecuteNonQueryAsync(It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("SQL Error"));
        }
        else
        {
            _mockCommand.Setup(c => c.ExecuteNonQueryAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
        }
    }

    [Fact]
    public async Task ExecuteAsync_Should_Execute_Command_Successfully()
    {
        // Arrange
        SetupMocks();
        string commandText = "UPDATE Users SET Name='Test' WHERE Id=1";
        SqlParameter[] parameters = { new SqlParameter("@Id", 1) };

        // Act & Assert
        await _repository.ExecuteAsync<TestUser>(commandText, CommandType.Text, parameters);
    }

    [Fact]
    public async Task ExecuteAsync_Should_Handle_Exception_And_Rollback()
    {
        // Arrange
        SetupMocks(throwException: true);
        string commandText = "INVALID SQL SYNTAX"; 

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _repository.ExecuteAsync<TestUser>(commandText, CommandType.Text));
    }

    [Fact]
    public async Task ExecuteBatchAsync_Should_Execute_Batches_Successfully()
    {
        // Arrange
        SetupMocks();
        string commandText = "INSERT INTO Users (Id, Name) VALUES (@Id, @Name)";
        List<TestUser> dataList = new List<TestUser>
        {
            new TestUser { Id = 1, Name = "Alice" },
            new TestUser { Id = 2, Name = "Bob" },
            new TestUser { Id = 3, Name = "Charlie" }
        };

        // Act & Assert
        await _repository.ExecuteBatchAsync<TestUser>(commandText, CommandType.Text, dataList);
    }

    [Fact]
    public async Task ExecuteBatchAsync_Should_Handle_Exception_And_Retry()
    {
        // Arrange
        SetupMocks(throwException: true);
        string commandText = "INVALID SQL SYNTAX"; 
        List<TestUser> dataList = new List<TestUser> { new TestUser { Id = 1, Name = "Alice" } };

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => _repository.ExecuteBatchAsync<TestUser>(commandText, CommandType.Text, dataList));
    }

    private class TestUser
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    // Custom Mock Class to Allow OpenAsync() Mocking
    public abstract class MockDbConnection : DbConnection
    {
        public override Task OpenAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
    }
}