using Xunit;
using Moq;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using IntuneMobilityViolationJob.Common;
using System.Text.Json;

public class AppConstantsTests
{
    private readonly Mock<IConfiguration> _mockConfiguration;
    private readonly Mock<IConfigurationSection> _mockSection;

    public AppConstantsTests()
    {
        _mockConfiguration = new Mock<IConfiguration>();
        _mockSection = new Mock<IConfigurationSection>();
    }

    [Fact]
    public void Initialize_ShouldAssignCorrectValues_WhenConfigurationIsValid()
    {
        // Arrange
        var validConfig = new Dictionary<string, string>
        {
            {"APIEndpoints:BaseUrl", "https://example.com"},
            {"APIEndpoints:DeviceOverview", "/devices/overview"},
            {"APIEndpoints:ManagedDevices", "/devices/managed"},
            {"APIEndpoints:ManagedDeviceFullData", "/devices/full"},
            {"APIEndpoints:RanagedDeviceUsers", "/devices/users"},
            {"APIEndpoints:ManagedDeviceDetectedApps", "/devices/detectedApps"},
            {"APIEndpoints:ManagedDevicePoliciesComplianceReport", "/devices/policies"},
            {"APIEndpoints:ManagedDevicePolicySettingsComplianceReport", "/devices/policySettings"},
            {"APIEndpoints:ManagedDeviceConfigurationPoliciesComplianceReport", "/devices/configPolicies"},
            {"APIEndpoints:ManagedDeviceConfigurationSettingComplianceReport", "/devices/configSettings"},
            {"APIEndpoints:ManagedDeviceConfigurationSettingStates", "/devices/configStates"},
            {"APIEndpoints:BatchRequest", "/batch/request"}
        };

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(validConfig)
            .Build();

        // Mock ProxySettings as a JSON serialized string
        var mockProxySettings = new ProxySettings { ProxyUrl = "http://proxy.com", UseProxy = true };
        _mockSection.Setup(s => s.Value).Returns(JsonSerializer.Serialize(mockProxySettings));
        _mockConfiguration.Setup(c => c.GetSection("ProxySettings")).Returns(_mockSection.Object);

        // Act
        AppConstants.Initialize(configuration);

        // Assert
        Assert.Equal("https://example.com", AppConstants.GraphAPIBaseurl);
        Assert.Equal("/devices/overview", AppConstants.DevicesOverview);
        Assert.Equal("/devices/managed", AppConstants.ManagedDevices);
        Assert.Equal("/devices/full", AppConstants.ManagedDeviceFullData);
        Assert.Equal("/devices/users", AppConstants.ManagedDeviceusers);
        Assert.Equal("/devices/detectedApps", AppConstants.ManagedDeviceDetectedApps);
        Assert.Equal("/devices/policies", AppConstants.ManagedDevicePoliciesComplianceReport);
        Assert.Equal("/devices/policySettings", AppConstants.ManagedDevicePolicySettingsComplianceReport);
        Assert.Equal("/devices/configPolicies", AppConstants.ManagedDeviceConfigurationPoliciesComplianceReport);
        Assert.Equal("/devices/configSettings", AppConstants.ManagedDeviceConfigurationSettingComplianceReport);
        Assert.Equal("/devices/configStates", AppConstants.ManagedDeviceConfigurationSettingStates);
        Assert.Equal("/batch/request", AppConstants.BatchRequest);

        // Ensure ProxySettings is correctly deserialized
        Assert.NotNull(AppConstants.ProxySettings);
        Assert.Equal("http://proxy.com", AppConstants.ProxySettings.ProxyUrl);
        Assert.True(AppConstants.ProxySettings.UseProxy);

        // Constant Value Assertion
        Assert.Equal("(Notes eq 'bc3e5c73-0224-4063-9b2b-0c36784b7e80') and (((deviceType eq 'iPad') or (deviceType eq 'iPhone') or (deviceType eq 'iPod')))", AppConstants.IOSDeviceFilter);
    }

    [Fact]
    public void Initialize_ShouldThrowException_WhenRequiredConfigMissing()
    {
        // Arrange
        var configuration = new ConfigurationBuilder().Build(); // Empty configuration

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => AppConstants.Initialize(configuration));
    }

    [Fact]
    public void Initialize_ShouldAssignNull_WhenProxySettingsMissing()
    {
        // Arrange
        var validConfig = new Dictionary<string, string>
        {
            {"APIEndpoints:BaseUrl", "https://example.com"},
            {"APIEndpoints:DeviceOverview", "/devices/overview"}
        };

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(validConfig)
            .Build();

        // Act
        AppConstants.Initialize(configuration);

        // Assert
        Assert.Null(AppConstants.ProxySettings);
    }
}