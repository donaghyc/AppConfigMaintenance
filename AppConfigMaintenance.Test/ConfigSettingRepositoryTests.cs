using AppConfigMaintenance.DataAccess;
using AppConfigMaintenance.DataAccess.Entities;
using AppConfigMaintenance.DataAccess.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppConfigMaintenance.DataAccess.Models;
using Xunit;

namespace AppConfigMaintenance.Test
{
    /// <summary>
    /// All tests to follow the Arrange - Act - Assert convention.
    /// Method names to follow the convention: MethodName_ConditionUnderTest_ExpectedOutcome
    /// </summary>
    public class ConfigSettingRepositoryTests : IDisposable
    {
        private readonly PaymentProcessingContext _context;

        public ConfigSettingRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<PaymentProcessingContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .EnableSensitiveDataLogging()
                .Options;

            _context = new PaymentProcessingContext(options);

            _context.Database.EnsureDeleted();

            var testConfigSettingList = new List<ConfigSetting>
            {
                new ConfigSetting
                {
                    Name = "ShowLevelUp",
                    Value = "6"
                },
                new ConfigSetting
                {
                    Name = "ShowDebit",
                    Value = "2"
                },
                new ConfigSetting
                {
                    Name = "ShowEBT",
                    Value = "3"
                }
            };

            _context.ConfigSettings.AddRange(testConfigSettingList);
            _context.SaveChanges();
        }

        [Fact]
        public void GetConfigSettings_NoFilter_GetsAllConfigSettings()
        {
            // Arrange
            var configSettingRepository = new ConfigSettingRepository(_context);

            // Act
            var result = configSettingRepository.GetConfigSettings();

            // Assert                
            Assert.Equal(3, result.Count());
        }

        [Fact]
        public void GetConfigSettings_FilterByName_GetsAllConfigSettings()
        {
            // Arrange
            var configSettingRepository = new ConfigSettingRepository(_context);

            // Act
            var result = configSettingRepository.GetConfigSettings().Where(x => x.Name == "ShowLevelUp");

            // Assert                
            Assert.Equal(1, result.Count());
        }

        [Fact]
        public void TryGetConfigSetting_GetByNameExists_GetsSingleConfigSettings()
        {
            // Arrange
            var configSettingRepository = new ConfigSettingRepository(_context);
            var configSettingModelOut = new ConfigSettingModel();

            // Act
            var result = configSettingRepository.TryGetConfigSetting("ShowDebit",out configSettingModelOut);

            // Assert                
            Assert.True(result);
            Assert.Equal("ShowDebit",configSettingModelOut.Name);
        }

        [Fact]
        public void TryGetConfigSetting_GetByNameNotExists_GetsNoConfigSettings()
        {
            // Arrange
            var configSettingRepository = new ConfigSettingRepository(_context);
            var configSettingModelOut = new ConfigSettingModel();

            // Act
            var result = configSettingRepository.TryGetConfigSetting("NameDoesNotExist", out configSettingModelOut);

            // Assert                
            Assert.False(result);
            Assert.Null(configSettingModelOut.Name);
        }

        [Fact]
        public async Task AddConfigSettingAsync_AddNewConfigSetting_AddIsSuccessful()
        {
            // Arrange
            var configSettingRepository = new ConfigSettingRepository(_context);

            // Act
            var result = await configSettingRepository.AddConfigSettingAsync(new ConfigSettingModel
            {
                Name = "ShowGWallet",
                Value = "7"
            });
            
            // Assert                
            Assert.Equal(1,result);
        }

        [Fact]
        public async Task AddConfigSettingAsync_TryAddDuplicateConfigSetting_AddIsNotSuccessful()
        {
            // Arrange
            var configSettingRepository = new ConfigSettingRepository(_context);

            // Act
            Action act = async () => await configSettingRepository.AddConfigSettingAsync(new ConfigSettingModel
            {
                Name = "ShowLevelUp",
                Value = "7"
            });

            // Assert                
            Assert.Throws<Exception>(act);
        }


        public void Dispose()
        {
            // Clean up DbContext
            _context.Dispose();
        }
    }
}
