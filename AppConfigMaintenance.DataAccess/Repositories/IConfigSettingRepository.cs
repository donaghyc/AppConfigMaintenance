using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppConfigMaintenance.DataAccess.Entities;
using AppConfigMaintenance.DataAccess.Models;

namespace AppConfigMaintenance.DataAccess.Repositories
{
    public interface IConfigSettingRepository
    {
        IQueryable<ConfigSettingModel> GetConfigSettings();

        bool TryGetConfigSetting(string name, out ConfigSettingModel configSetting);

        Task<int> AddConfigSettingAsync(ConfigSettingModel configSetting);

        Task<int> UpdateConfigSettingByNameAsync(string name, string value);

        Task<int> DeleteConfigSettingByNameAsync(string name);
    }
}