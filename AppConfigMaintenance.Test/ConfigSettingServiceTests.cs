using AppConfigMaintenance.DataAccess.Models;
using AppConfigMaintenance.DataAccess.Repositories;
using AppConfigMaintenance.DataAccess.Services;
using Moq;
using System;
using System.Threading.Tasks;
using Xunit;

namespace AppConfigMaintenance.Test
{
    /// <summary>
    /// All tests to follow the Arrange - Act - Assert convention.
    /// Method names to follow the convention: MethodName_ConditionUnderTest_ExpectedOutcome
    /// </summary>
    public class ConfigSettingServiceTests
    {
        private readonly Mock<IConfigSettingRepository> _configSettingRepository;

        public ConfigSettingServiceTests()
        {
            _configSettingRepository = new Mock<IConfigSettingRepository>();
        }

        [Fact]
        public async Task AddConfigSettingAsync_ParallelExecution_TimeoutExceptionThrownDueToLock()
        {
            // Arrange
            var exceptionThrown = false;

            var testConfigSetting = new ConfigSettingModel
            {
                Name = "ShowLevelUp",
                Value = "6"
            };

            var configSettingService = new ConfigSettingService(_configSettingRepository.Object);

            // Act
            var aggregateTask = Task.WhenAll(
                configSettingService.AddConfigSettingAsync(testConfigSetting, 5000),
                configSettingService.AddConfigSettingAsync(testConfigSetting, 5000));

            try
            {
                await aggregateTask;
            }
            catch (TimeoutException ex)
            {
                if (ex.Message.Contains("Another caller is already updating this setting."))
                {
                    exceptionThrown = true;
                }
            }

            // Assert                
            Assert.True(exceptionThrown);
        }
    }
}
