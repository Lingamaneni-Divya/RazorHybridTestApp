using System;
using System.Collections.Generic;
using IntuneMobilityViolationJob.Models.DataTableModels;

public static class InputDataMock
{
    public static List<DeviceActionResults> GetDeviceActionResultsMock()
    {
        return new List<DeviceActionResults>
        {
            new DeviceActionResults { DeviceId = "123", Action = "Wipe", Status = "Success" },
            new DeviceActionResults { DeviceId = "456", Action = "Lock", Status = "Pending" }
        };
    }

    public static List<Devices> GetDevicesMock()
    {
        return new List<Devices>
        {
            new Devices { DeviceId = "Device1", OS = "iOS", Model = "iPhone 12" },
            new Devices { DeviceId = "Device2", OS = "Android", Model = "Samsung S21" }
        };
    }

    public static List<DeviceUsersLogons> GetDeviceUsersLogonsMock()
    {
        return new List<DeviceUsersLogons>
        {
            new DeviceUsersLogons { UserId = "User1", DeviceId = "Device1", LastLoginTime = DateTime.UtcNow },
            new DeviceUsersLogons { UserId = "User2", DeviceId = "Device2", LastLoginTime = DateTime.UtcNow.AddDays(-1) }
        };
    }

    public static List<Users> GetUsersMock()
    {
        return new List<Users>
        {
            new Users { UserId = "User1", UserName = "John Doe", Email = "john@example.com" },
            new Users { UserId = "User2", UserName = "Jane Smith", Email = "jane@example.com" }
        };
    }

    public static List<UserIdentities> GetUserIdentitiesMock()
    {
        return new List<UserIdentities>
        {
            new UserIdentities { UserId = "User1", IdentityType = "AzureAD", IdentityValue = "john@company.com" },
            new UserIdentities { UserId = "User2", IdentityType = "OnPrem", IdentityValue = "jane@company.com" }
        };
    }

    public static List<DeviceHardwareInformation> GetDeviceHardwareInformationMock()
    {
        return new List<DeviceHardwareInformation>
        {
            new DeviceHardwareInformation { DeviceId = "Device1", Processor = "A14 Bionic", RAM = "4GB" },
            new DeviceHardwareInformation { DeviceId = "Device2", Processor = "Snapdragon 888", RAM = "8GB" }
        };
    }

    public static List<UserAssignedPlans> GetUserAssignedPlansMock()
    {
        return new List<UserAssignedPlans>
        {
            new UserAssignedPlans { UserId = "User1", PlanName = "Office 365", AssignedDate = DateTime.UtcNow },
            new UserAssignedPlans { UserId = "User2", PlanName = "Microsoft E3", AssignedDate = DateTime.UtcNow.AddDays(-5) }
        };
    }

    public static List<UserOnPremisesSipInfo> GetUserOnPremisesSipInfoMock()
    {
        return new List<UserOnPremisesSipInfo>
        {
            new UserOnPremisesSipInfo { UserId = "User1", SipAddress = "sip:john@example.com" },
            new UserOnPremisesSipInfo { UserId = "User2", SipAddress = "sip:jane@example.com" }
        };
    }
}
using System.Threading.Tasks;
using Moq;
using Xunit;
using IntuneMobilityViolationJob.Repository.Command;
using IntuneMobilityViolationJob.Repository.Command.Interfaces;
using System.Data;
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
        var deviceActionResults = InputDataMock.GetDeviceActionResultsMock();

        _commandRepositoryMock
            .Setup(repo => repo.ExecuteAsync("[IntuneMobilityViolation].[SaveOrUpdateDeviceActionResults]", 
                CommandType.StoredProcedure, It.IsAny<SqlParameter[]>()))
            .Returns(Task.CompletedTask);

        await _repository.SaveDeviceActionResults(deviceActionResults);

        _commandRepositoryMock.Verify(repo =>
            repo.ExecuteAsync("[IntuneMobilityViolation].[SaveOrUpdateDeviceActionResults]",
            CommandType.StoredProcedure, It.IsAny<SqlParameter[]>()),
            Times.Once);
    }

    [Fact]
    public async Task SaveDeviceData_ShouldCallExecuteAsync_WithCorrectParameters()
    {
        var devices = InputDataMock.GetDevicesMock();

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
    public async Task SaveUserData_ShouldCallExecuteAsync_WithCorrectParameters()
    {
        var users = InputDataMock.GetUsersMock();

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
        var userIdentities = InputDataMock.GetUserIdentitiesMock();

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
        var hardwareInfo = InputDataMock.GetDeviceHardwareInformationMock();

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
}