using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

public class AppConstantsTests
{
    [Fact]
    public void Initialize_SetsAllValuesCorrectly()
    {
        // Arrange
        var configValues = new Dictionary<string, string>
        {
            { "APIEndpoints:BaseUrl", "https://api.example.com" },
            { "APIEndpoints:DeviceOverview", "/devices/overview" },
            { "APIEndpoints:ManagedDevices", "/devices/managed" },
            { "APIEndpoints:ManagedDeviceFullData", "/devices/fullData" },
            { "APIEndpoints:ManagedDeviceUsers", "/devices/users" },
            { "APIEndpoints:ManagedDeviceDetectedApps", "/devices/detectedApps" },
            { "APIEndpoints:ManagedDevicePoliciesComplianceReport", "/devices/policiesCompliance" },
            { "APIEndpoints:ManagedDevicePolicySettingsComplianceReport", "/devices/policySettingsCompliance" },
            { "APIEndpoints:ManagedDeviceConfigurationPoliciesComplianceReport", "/devices/configPoliciesCompliance" },
            { "APIEndpoints:ManagedDeviceConfigurationSettingComplianceReport", "/devices/configSettingCompliance" },
            { "APIEndpoints:ManagedDeviceConfigurationSettingStates", "/devices/configSettingStates" },
            { "APIEndpoints:BatchRequest", "/batch" },
            { "ProxySettings:ProxyAddress", "http://proxy.example.com" },
            { "ProxySettings:ProxyBypassList:0", "localhost" },
            { "ProxySettings:ProxyBypassList:1", "127.0.0.1" }
        };

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(configValues)
            .Build();

        // Act
        AppConstants.Initialize(configuration);

        // Assert - Check all properties are correctly initialized
        Assert.Equal("https://api.example.com", AppConstants.GraphAPIBaseurl);
        Assert.Equal("/devices/overview", AppConstants.DevicesOverview);
        Assert.Equal("/devices/managed", AppConstants.ManagedDevices);
        Assert.Equal("/devices/fullData", AppConstants.ManagedDeviceFullData);
        Assert.Equal("/devices/users", AppConstants.ManagedDeviceUsers);
        Assert.Equal("/devices/detectedApps", AppConstants.ManagedDeviceDetectedApps);
        Assert.Equal("/devices/policiesCompliance", AppConstants.ManagedDevicePoliciesComplianceReport);
        Assert.Equal("/devices/policySettingsCompliance", AppConstants.ManagedDevicePolicySettingsComplianceReport);
        Assert.Equal("/devices/configPoliciesCompliance", AppConstants.ManagedDeviceConfigurationPoliciesComplianceReport);
        Assert.Equal("/devices/configSettingCompliance", AppConstants.ManagedDeviceConfigurationSettingComplianceReport);
        Assert.Equal("/devices/configSettingStates", AppConstants.ManagedDeviceConfigurationSettingStates);
        Assert.Equal("/batch", AppConstants.BatchRequest);

        // ProxySettings assertions
        Assert.NotNull(AppConstants.ProxySettings);
        Assert.Equal("http://proxy.example.com", AppConstants.ProxySettings.ProxyAddress);
        Assert.Contains("localhost", AppConstants.ProxySettings.ProxyBypassList);
        Assert.Contains("127.0.0.1", AppConstants.ProxySettings.ProxyBypassList);
    }

    [Fact]
    public void Initialize_SetsProxySettingsToNull_WhenMissing()
    {
        // Arrange
        var configValues = new Dictionary<string, string>
        {
            { "APIEndpoints:BaseUrl", "https://api.example.com" }
        };

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(configValues)
            .Build();

        // Act
        AppConstants.Initialize(configuration);

        // Assert
        Assert.Null(AppConstants.ProxySettings);
    }

    [Fact]
    public void Initialize_ThrowsException_WhenMandatoryFieldsAreMissing()
    {
        // Arrange
        var configValues = new Dictionary<string, string>(); // Empty configuration

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(configValues)
            .Build();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => AppConstants.Initialize(configuration));
    }
}