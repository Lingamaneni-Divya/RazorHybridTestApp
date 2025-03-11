using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Moq;
using Xunit;

public class SqlConnectionMockTest
{
    private readonly Mock<IDbConnectionFactory> _mockConnectionFactory;
    private readonly Mock<SqlConnection> _mockConnection;

    public SqlConnectionMockTest()
    {
        _mockConnectionFactory = new Mock<IDbConnectionFactory>();
        _mockConnection = new Mock<SqlConnection>();

        // Setup mock factory to return our fake connection
        _mockConnectionFactory.Setup(f => f.CreateConnection()).Returns(_mockConnection.Object);

        // ✅ Mock OpenAsync() to simulate successful connection
        _mockConnection.Setup(c => c.OpenAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
    }

    [Fact]
    public async Task SqlConnection_Should_OpenAsync_Successfully()
    {
        // Arrange
        var connection = _mockConnectionFactory.Object.CreateConnection();

        // Act
        await connection.OpenAsync();

        // Assert: Verify OpenAsync() was called once
        _mockConnection.Verify(c => c.OpenAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}