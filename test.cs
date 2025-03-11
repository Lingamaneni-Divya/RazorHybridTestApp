using System;
using System.Data;
using System.Threading;
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

        // **Create a mock SqlConnection**
        _mockConnection = new Mock<SqlConnection>("FakeConnectionString") { CallBase = true };

        // ✅ Mock OpenAsync()
        _mockConnection.As<IDisposable>().Setup(c => c.Dispose());
        _mockConnection.Setup(c => c.OpenAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        // **Mock BeginTransaction**
        _mockTransaction = new Mock<SqlTransaction>();
        _mockConnection.Setup(c => c.BeginTransaction()).Returns(_mockTransaction.Object);

        // **Mock SqlCommand**
        _mockCommand = new Mock<SqlCommand>();
        _mockCommand.Setup(c => c.ExecuteNonQueryAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

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
        _mockCommand.Verify(c => c.ExecuteNonQueryAsync(It.IsAny<CancellationToken>()), Times.Once);
        _mockTransaction.Verify(t => t.Commit(), Times.Once);
    }
}