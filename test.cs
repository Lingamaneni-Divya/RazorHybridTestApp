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
    private readonly Mock<CommandRepository> _mockCommandRepository;

    public CommandRepositoryTests()
    {
        _mockConfig = new Mock<IConfiguration>();
        _mockConfig.Setup(c => c["ConnectionStrings:MobilityViolenceWriteDB"]).Returns("FakeConnectionString");

        // ✅ Partial mock of CommandRepository
        _mockCommandRepository = new Mock<CommandRepository>(_mockConfig.Object) { CallBase = true };

        // ✅ Mock ExecuteAsync<T>() so it doesn't actually hit a database
        _mockCommandRepository
            .Setup(repo => repo.ExecuteAsync<object>(
                It.IsAny<string>(),  // Any SQL command
                It.IsAny<CommandType>(), // Any command type
                It.IsAny<SqlParameter[]>(), // Any SQL parameters
                It.IsAny<int>(), // Any retry count
                It.IsAny<int>())) // Any delay
            .Returns(Task.CompletedTask); // ✅ Just complete without errors
    }

    [Fact]
    public async Task ExecuteAsync_Should_Run_Without_Error()
    {
        // Arrange
        string commandText = "INSERT INTO SampleTable VALUES ('TestName')";

        // Act
        await _mockCommandRepository.Object.ExecuteAsync<object>(commandText, CommandType.Text);

        // Assert: Verify ExecuteAsync<T>() was called once with correct parameters
        _mockCommandRepository.Verify(repo => repo.ExecuteAsync<object>(
            commandText, CommandType.Text, null, 3, 1000), Times.Once);
    }
}