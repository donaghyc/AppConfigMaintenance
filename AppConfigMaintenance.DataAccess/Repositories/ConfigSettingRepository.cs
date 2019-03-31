using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppConfigMaintenance.DataAccess.Entities;
using AppConfigMaintenance.DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace AppConfigMaintenance.DataAccess.Repositories
{
    public class ConfigSettingRepository : IConfigSettingRepository, IDisposable
    {
        private readonly PaymentProcessingContext _context;

        public ConfigSettingRepository(PaymentProcessingContext context)
        {
            _context = context;
        }

        public IQueryable<ConfigSettingModel> GetConfigSettings()
        {
            return _context.ConfigSettings.Select(x => new ConfigSettingModel
            {
                Name = x.Name,
                Value = x.Value
            });
        }

        public bool TryGetConfigSetting(string name, out ConfigSettingModel configSetting)
        {
            var configSettingFromDatabase = _context.ConfigSettings.SingleOrDefault(x => x.Name == name);

            if (configSettingFromDatabase != null)
            {
                configSetting = new ConfigSettingModel
                {
                    Name = configSettingFromDatabase.Name,
                    Value = configSettingFromDatabase.Value
                };

                return true;
            }

            configSetting = new ConfigSettingModel();
            return false;
        }

        public async Task<int> AddConfigSettingAsync(ConfigSettingModel configSetting)
        {
            int rowsAffected = 0;

            try
            {
                _context.ConfigSettings.Add(new ConfigSetting
                {
                    Name = configSetting.Name,
                    Value = configSetting.Value
                });

                rowsAffected = await _context.SaveChangesAsync();                
            }
            catch (Exception ex)
            {
                //TODO log this exception
                throw;
            }

            return rowsAffected;
        }

        public async Task<int> UpdateConfigSettingByNameAsync(string name, string value)
        {
            int rowsAffected = 0;

            try
            {
                var configSettingFromDatabase = await _context.ConfigSettings.SingleAsync(x => x.Name == name);
                configSettingFromDatabase.Value = value;

                rowsAffected = await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                var c = ex.Message;
            }

            return rowsAffected;
        }

        public async Task<int> DeleteConfigSettingByNameAsync(string name)
        {
            int rowsAffected = 0;

            try
            {
                _context.ConfigSettings.Remove(_context.ConfigSettings.Single(x => x.Name == name));

                rowsAffected = await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                var c = ex.Message;
            }

            return rowsAffected;
        }

        public void Dispose()
        {
            // Clean up DbContext
            _context.Dispose();
        }
    }
}
