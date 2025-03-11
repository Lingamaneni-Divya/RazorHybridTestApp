using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;
using IntuneMobilityViolationJob.Repository.Command.BaseRepository;
using GuardAgainstLib;

public class CommandRepositoryTests
{
    private readonly Mock<IConfiguration> _mockConfig;
    private readonly Mock<SqlConnection> _mockSqlConnection;
    private readonly Mock<SqlCommand> _mockSqlCommand;
    private readonly Mock<SqlTransaction> _mockTransaction;
    private readonly CommandRepository _commandRepository;
    private readonly string _validConnectionString = "Server=myServer;Database=myDB;User Id=myUser;Password=myPassword;";

    public CommandRepositoryTests()
    {
        _mockConfig = new Mock<IConfiguration>();
        _mockConfig.Setup(c => c["ConnectionStrings:MobilityViolenceWriteDB"]).Returns(_validConnectionString);

        _mockSqlConnection = new Mock<SqlConnection>();
        _mockSqlCommand = new Mock<SqlCommand>();
        _mockTransaction = new Mock<SqlTransaction>();

        _commandRepository = new CommandRepository(_mockConfig.Object);
    }

    private SqlException CreateSqlException(string message = "A database error occurred")
    {
        var sqlExceptionType = typeof(SqlException);
        var instance = (SqlException)Activator.CreateInstance(sqlExceptionType, BindingFlags.Instance | BindingFlags.NonPublic, null, new object[] { message }, null);
        return instance;
    }

    [Fact]
    public async Task ExecuteAsync_Should_Rollback_On_SqlException()
    {
        // Arrange
        var commandText = "INSERT INTO SampleTable VALUES ('test')";
        _mockSqlConnection.Setup(c => c.OpenAsync()).Returns(Task.CompletedTask);
        _mockSqlConnection.Setup(c => c.BeginTransaction()).Returns(_mockTransaction.Object);
        _mockSqlCommand.Setup(c => c.ExecuteNonQueryAsync()).ThrowsAsync(CreateSqlException());

        // Act
        await _commandRepository.ExecuteAsync<object>(commandText, CommandType.Text);

        // Assert
        _mockTransaction.Verify(t => t.Rollback(), Times.Once);
    }

    [Fact]
    public async Task ExecuteBatchAsync_Should_Retry_On_SqlException_And_Rollback()
    {
        // Arrange
        var commandText = "INSERT INTO SampleTable VALUES (@param)";
        var dataList = new List<string> { "value1", "value2", "value3" };
        int retryCount = 0;

        _mockSqlConnection.Setup(c => c.OpenAsync()).Returns(Task.CompletedTask);
        _mockSqlConnection.Setup(c => c.BeginTransaction()).Returns(_mockTransaction.Object);
        _mockSqlCommand.Setup(c => c.ExecuteNonQueryAsync()).ReturnsAsync(() =>
        {
            retryCount++;
            if (retryCount < 3) throw CreateSqlException();
            return 3;
        });

        // Act
        await _commandRepository.ExecuteBatchAsync(commandText, CommandType.Text, dataList);

        // Assert
        _mockTransaction.Verify(t => t.Rollback(), Times.Exactly(2)); // Rolled back twice before success
        _mockTransaction.Verify(t => t.Commit(), Times.Once); // Committed on final retry
    }
}