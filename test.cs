using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Moq;
using Xunit;
using IntuneMobilityViolationJob.Repository.Command;
using IntuneMobilityViolationJob.Repository.Command.Interfaces;
using IntuneMobilityViolationJob.Models.DataTableModels;
using Microsoft.Data.SqlClient;

public class IOSIntuneDeviceComplianceRepositoryTests
{
    private readonly IOSIntuneDeviceComplianceRepository _repository;
    private readonly Mock<ICommandRepository> _commandRepositoryMock;

    public IOSIntuneDeviceComplianceRepositoryTests()
    {
        _commandRepositoryMock = new Mock<ICommandRepository>();
        _repository = new IOSIntuneDeviceComplianceRepository(_commandRepositoryMock.Object);
    }

    [Fact]
    public async Task SaveDeviceActionResults_ShouldCallExecuteAsync_WithCorrectParameters()
    {
        // Arrange
        var deviceActionResults = new List<DeviceActionResults>
        {
            new DeviceActionResults { DeviceId = "123", Action = "Wipe", Status = "Success" }
        };

        _commandRepositoryMock
            .Setup(repo => repo.ExecuteAsync("[IntuneMobilityViolation].[SaveOrUpdateDeviceActionResults]", 
                CommandType.StoredProcedure, It.IsAny<SqlParameter[]>()))
            .Returns(Task.CompletedTask);

        // Act
        await _repository.SaveDeviceActionResults(deviceActionResults);

        // Assert
        _commandRepositoryMock.Verify(repo =>
            repo.ExecuteAsync("[IntuneMobilityViolation].[SaveOrUpdateDeviceActionResults]",
            CommandType.StoredProcedure, It.IsAny<SqlParameter[]>()),
            Times.Once);
    }

    [Fact]
    public async Task SaveDeviceData_ShouldCallExecuteAsync_WithCorrectParameters()
    {
        var devices = new List<Devices>
        {
            new Devices { DeviceId = "Device1", OS = "iOS", Model = "iPhone 12" }
        };

        _commandRepositoryMock
            .Setup(repo => repo.ExecuteAsync("[IntuneMobilityViolation].[SaveOrUpdateDevices]", 
                CommandType.StoredProcedure, It.IsAny<SqlParameter[]>()))
            .Returns(Task.CompletedTask);

        await _repository.SaveDeviceData(devices);

        _commandRepositoryMock.Verify(repo =>
            repo.ExecuteAsync("[IntuneMobilityViolation].[SaveOrUpdateDevices]",
            CommandType.StoredProcedure, It.IsAny<SqlParameter[]>()),
            Times.Once);
    }

    [Fact]
    public async Task SaveDeviceUsersLogons_ShouldCallExecuteAsync_WithCorrectParameters()
    {
        var deviceUsersLogons = new List<DeviceUsersLogons>
        {
            new DeviceUsersLogons { UserId = "User1", DeviceId = "Device1", LastLoginTime = System.DateTime.UtcNow }
        };

        _commandRepositoryMock
            .Setup(repo => repo.ExecuteAsync("[IntuneMobilityViolation].[SaveOrUpdateDeviceUsersLogons]", 
                CommandType.StoredProcedure, It.IsAny<SqlParameter[]>()))
            .Returns(Task.CompletedTask);

        await _repository.SaveDeviceUsersLogons(deviceUsersLogons);

        _commandRepositoryMock.Verify(repo =>
            repo.ExecuteAsync("[IntuneMobilityViolation].[SaveOrUpdateDeviceUsersLogons]",
            CommandType.StoredProcedure, It.IsAny<SqlParameter[]>()),
            Times.Once);
    }

    [Fact]
    public async Task SaveUserData_ShouldCallExecuteAsync_WithCorrectParameters()
    {
        var users = new List<Users>
        {
            new Users { UserId = "User1", UserName = "John Doe", Email = "john@example.com" }
        };

        _commandRepositoryMock
            .Setup(repo => repo.ExecuteAsync("[IntuneMobilityViolation].[SaveOrUpdateUsers]", 
                CommandType.StoredProcedure, It.IsAny<SqlParameter[]>()))
            .Returns(Task.CompletedTask);

        await _repository.SaveUserData(users);

        _commandRepositoryMock.Verify(repo =>
            repo.ExecuteAsync("[IntuneMobilityViolation].[SaveOrUpdateUsers]",
            CommandType.StoredProcedure, It.IsAny<SqlParameter[]>()),
            Times.Once);
    }

    [Fact]
    public async Task SaveUserIdentities_ShouldCallExecuteAsync_WithCorrectParameters()
    {
        var userIdentities = new List<UserIdentities>
        {
            new UserIdentities { UserId = "User1", IdentityType = "AzureAD", IdentityValue = "john@company.com" }
        };

        _commandRepositoryMock
            .Setup(repo => repo.ExecuteAsync("[IntuneMobilityViolation].[SaveOrUpdateUserIdentities]", 
                CommandType.StoredProcedure, It.IsAny<SqlParameter[]>()))
            .Returns(Task.CompletedTask);

        await _repository.SaveUserIdentities(userIdentities);

        _commandRepositoryMock.Verify(repo =>
            repo.ExecuteAsync("[IntuneMobilityViolation].[SaveOrUpdateUserIdentities]",
            CommandType.StoredProcedure, It.IsAny<SqlParameter[]>()),
            Times.Once);
    }

    [Fact]
    public async Task SaveDeviceHardwareInformation_ShouldCallExecuteAsync_WithCorrectParameters()
    {
        var hardwareInfo = new List<DeviceHardwareInformation>
        {
            new DeviceHardwareInformation { DeviceId = "Device1", Processor = "A14 Bionic", RAM = "4GB" }
        };

        _commandRepositoryMock
            .Setup(repo => repo.ExecuteAsync("[IntuneMobilityViolation].[SaveOrUpdateDeviceHardwareInformation]", 
                CommandType.StoredProcedure, It.IsAny<SqlParameter[]>()))
            .Returns(Task.CompletedTask);

        await _repository.SaveDeviceHardwareInformation(hardwareInfo);

        _commandRepositoryMock.Verify(repo =>
            repo.ExecuteAsync("[IntuneMobilityViolation].[SaveOrUpdateDeviceHardwareInformation]",
            CommandType.StoredProcedure, It.IsAny<SqlParameter[]>()),
            Times.Once);
    }

    [Fact]
    public async Task SaveUserAssignedPlans_ShouldCallExecuteAsync_WithCorrectParameters()
    {
        var assignedPlans = new List<UserAssignedPlans>
        {
            new UserAssignedPlans { UserId = "User1", PlanName = "Office 365", AssignedDate = System.DateTime.UtcNow }
        };

        _commandRepositoryMock
            .Setup(repo => repo.ExecuteAsync("[IntuneMobilityViolation].[SaveOrUpdateUserAssignedPlans]", 
                CommandType.StoredProcedure, It.IsAny<SqlParameter[]>()))
            .Returns(Task.CompletedTask);

        await _repository.SaveUserAssignedPlans(assignedPlans);

        _commandRepositoryMock.Verify(repo =>
            repo.ExecuteAsync("[IntuneMobilityViolation].[SaveOrUpdateUserAssignedPlans]",
            CommandType.StoredProcedure, It.IsAny<SqlParameter[]>()),
            Times.Once);
    }

    [Fact]
    public async Task SaveUserOnPremisesSipInfo_ShouldCallExecuteAsync_WithCorrectParameters()
    {
        var sipInfo = new List<UserOnPremisesSipInfo>
        {
            new UserOnPremisesSipInfo { UserId = "User1", SipAddress = "sip:john@example.com" }
        };

        _commandRepositoryMock
            .Setup(repo => repo.ExecuteAsync("[IntuneMobilityViolation].[SaveOrUpdateUserOnPremisesSipInfo]", 
                CommandType.StoredProcedure, It.IsAny<SqlParameter[]>()))
            .Returns(Task.CompletedTask);

        await _repository.SaveUserOnPremisesSipInfo(sipInfo);

        _commandRepositoryMock.Verify(repo =>
            repo.ExecuteAsync("[IntuneMobilityViolation].[SaveOrUpdateUserOnPremisesSipInfo]",
            CommandType.StoredProcedure, It.IsAny<SqlParameter[]>()),
            Times.Once);
    }
}