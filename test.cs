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

public class CommandRepositoryTests
{
    private readonly Mock<IConfiguration> _mockConfig;
    private readonly Mock<SqlConnection> _mockConnection;
    private readonly Mock<SqlCommand> _mockCommand;
    private readonly Mock<SqlTransaction> _mockTransaction;
    private readonly CommandRepository _commandRepository;

    public CommandRepositoryTests()
    {
        _mockConfig = new Mock<IConfiguration>();
        _mockConfig.Setup(c => c["ConnectionStrings:MobilityViolenceWriteDB"]).Returns("FakeConnectionString");

        _mockConnection = new Mock<SqlConnection>();
        _mockCommand = new Mock<SqlCommand>();
        _mockTransaction = new Mock<SqlTransaction>();

        // Mock connection behavior
        _mockConnection.Setup(c => c.OpenAsync(It.IsAny<System.Threading.CancellationToken>())).Returns(Task.CompletedTask);
        _mockConnection.Setup(c => c.BeginTransaction()).Returns(_mockTransaction.Object);

        // Mock command behavior
        _mockCommand.Setup(c => c.ExecuteNonQueryAsync(It.IsAny<System.Threading.CancellationToken>())).ReturnsAsync(1);
        _mockConnection.Setup(c => c.CreateCommand()).Returns(_mockCommand.Object);

        _commandRepository = new CommandRepository(_mockConfig.Object);
    }

    [Fact]
    public async Task ExecuteAsync_Should_Execute_Command_Successfully()
    {
        // Arrange
        string commandText = "INSERT INTO SampleTable VALUES ('TestName')";

        _mockCommand.SetupSet(c => c.Connection = _mockConnection.Object);
        _mockCommand.SetupSet(c => c.Transaction = _mockTransaction.Object);
        _mockCommand.SetupSet(c => c.CommandText = commandText);
        _mockCommand.SetupSet(c => c.CommandType = CommandType.Text);

        // Act
        await _commandRepository.ExecuteAsync<object>(commandText, CommandType.Text);

        // Assert
        _mockCommand.Verify(c => c.ExecuteNonQueryAsync(It.IsAny<System.Threading.CancellationToken>()), Times.Once);
        _mockTransaction.Verify(t => t.Commit(), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_Should_Rollback_On_SqlException()
    {
        // Arrange
        string commandText = "INSERT INTO SampleTable VALUES ('TestName')";
        
        // Generate a fake SqlException using reflection
        var sqlException = GenerateFakeSqlException();

        _mockCommand.Setup(c => c.ExecuteNonQueryAsync(It.IsAny<System.Threading.CancellationToken>())).ThrowsAsync(sqlException);

        _mockCommand.SetupSet(c => c.Connection = _mockConnection.Object);
        _mockCommand.SetupSet(c => c.Transaction = _mockTransaction.Object);
        _mockCommand.SetupSet(c => c.CommandText = commandText);
        _mockCommand.SetupSet(c => c.CommandType = CommandType.Text);

        // Act
        await _commandRepository.ExecuteAsync<object>(commandText, CommandType.Text);

        // Assert
        _mockTransaction.Verify(t => t.Rollback(), Times.Once);
    }

    // 🔥 Helper method to create a fake SqlException
    private static SqlException GenerateFakeSqlException()
    {
        var sqlErrorNumber = 50000; // Arbitrary SQL error number
        var sqlError = Activator.CreateInstance(
            typeof(SqlError),
            BindingFlags.Instance | BindingFlags.NonPublic,
            null,
            new object[] { sqlErrorNumber, (byte)0, (byte)0, "", "", "", 1 },
            null
        );

        var sqlErrorCollection = Activator.CreateInstance(
            typeof(SqlErrorCollection),
            BindingFlags.Instance | BindingFlags.NonPublic,
            null,
            null,
            null
        );

        var addMethod = typeof(SqlErrorCollection).GetMethod("Add", BindingFlags.Instance | BindingFlags.NonPublic);
        addMethod.Invoke(sqlErrorCollection, new[] { sqlError });

        var sqlException = Activator.CreateInstance(
            typeof(SqlException),
            BindingFlags.Instance | BindingFlags.NonPublic,
            null,
            new object[] { "Mock SQL Exception", sqlErrorCollection, null, Guid.NewGuid() },
            null
        ) as SqlException;

        return sqlException!;
    }
}