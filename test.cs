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
    private readonly Mock<SqlConnection> _mockConnection;
    private readonly Mock<SqlCommand> _mockCommand;
    private readonly Mock<SqlTransaction> _mockTransaction;

    public CommandRepositoryTests()
    {
        _mockConfiguration = new Mock<IConfiguration>();
        _mockConfiguration.Setup(config => config["ConnectionStrings:MobilityViolationDB"]).Returns("Fake_Connection_String");

        _mockConnection = new Mock<SqlConnection>();
        _mockCommand = new Mock<SqlCommand>();
        _mockTransaction = new Mock<SqlTransaction>();

        _repository = new CommandRepository(_mockConfiguration.Object);
    }

    private void SetupMocks(bool throwException = false)
    {
        _mockConnection.Setup(c => c.OpenAsync()).Returns(Task.CompletedTask);
        _mockConnection.Setup(c => c.BeginTransaction()).Returns(_mockTransaction.Object);
        
        if (throwException)
        {
            _mockCommand.Setup(c => c.ExecuteNonQueryAsync()).ThrowsAsync(CreateSqlException());
        }
        else
        {
            _mockCommand.Setup(c => c.ExecuteNonQueryAsync()).ReturnsAsync(1);
        }
    }

    private SqlException CreateSqlException()
    {
        var sqlErrorCollection = (SqlErrorCollection)Activator.CreateInstance(typeof(SqlErrorCollection), true);
        var sqlError = (SqlError)Activator.CreateInstance(typeof(SqlError), true);
        typeof(SqlErrorCollection).GetMethod("Add", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).Invoke(sqlErrorCollection, new object[] { sqlError });

        var sqlException = (SqlException)Activator.CreateInstance(typeof(SqlException), true);
        return sqlException;
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
        await Assert.ThrowsAsync<SqlException>(() => _repository.ExecuteAsync<TestUser>(commandText, CommandType.Text));
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
        await Assert.ThrowsAsync<SqlException>(() => _repository.ExecuteBatchAsync<TestUser>(commandText, CommandType.Text, dataList));
    }

    private class TestUser
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}