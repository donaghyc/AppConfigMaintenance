using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AppConfigMaintenance.DataAccess.Entities;
using AppConfigMaintenance.DataAccess.Models;

namespace AppConfigMaintenance.DataAccess.Services
{
    public interface IConfigSettingService
    {
        Task<int> AddConfigSettingAsync(ConfigSettingModel configSetting, int? optionalDelay = null);

        Task<int> UpdateConfigSettingByNameAsync(string name, string value, int? optionalDelay = null);

    //    /// <summary>
    //    /// Gets a configSetting by identifier
    //    /// </summary>
    //    /// <param name="configSettingId">ConfigSetting identifier</param>
    //    /// <returns>ConfigSetting</returns>
    //    ConfigSetting GetConfigSettingById(int configSettingId);

    //    /// <summary>
    //    /// Deletes a configSetting
    //    /// </summary>
    //    /// <param name="configSetting">ConfigSetting</param>
    //    void DeleteConfigSetting(ConfigSetting configSetting);

    //    /// <summary>
    //    /// Deletes configSettings
    //    /// </summary>
    //    /// <param name="configSettings">ConfigSettings</param>
    //    void DeleteConfigSettings(IList<ConfigSetting> configSettings);

    //    /// <summary>
    //    /// Get configSetting by key
    //    /// </summary>
    //    /// <param name="key">Key</param>
    //    /// <param name="storeId">Store identifier</param>
    //    /// <param name="loadSharedValueIfNotFound">A value indicating whether a shared (for all stores) value should be loaded if a value specific for a certain is not found</param>
    //    /// <returns>ConfigSetting</returns>
    //    ConfigSetting GetConfigSetting(string key, int storeId = 0, bool loadSharedValueIfNotFound = false);

    //    /// <summary>
    //    /// Get configSetting value by key
    //    /// </summary>
    //    /// <typeparam name="T">Type</typeparam>
    //    /// <param name="key">Key</param>
    //    /// <param name="storeId">Store identifier</param>
    //    /// <param name="defaultValue">Default value</param>
    //    /// <param name="loadSharedValueIfNotFound">A value indicating whether a shared (for all stores) value should be loaded if a value specific for a certain is not found</param>
    //    /// <returns>ConfigSetting value</returns>
    //    T GetConfigSettingByKey<T>(string key, T defaultValue = default(T),
    //        int storeId = 0, bool loadSharedValueIfNotFound = false);

    //    /// <summary>
    //    /// Set configSetting value
    //    /// </summary>
    //    /// <typeparam name="T">Type</typeparam>
    //    /// <param name="key">Key</param>
    //    /// <param name="value">Value</param>
    //    /// <param name="storeId">Store identifier</param>
    //    /// <param name="clearCache">A value indicating whether to clear cache after configSetting update</param>
    //    void SetConfigSetting<T>(string key, T value, int storeId = 0, bool clearCache = true);

    //    /// <summary>
    //    /// Gets all configSettings
    //    /// </summary>
    //    /// <returns>ConfigSettings</returns>
    //    IList<ConfigSetting> GetAllConfigSettings();

    //    /// <summary>
    //    /// Determines whether a configSetting exists
    //    /// </summary>
    //    /// <typeparam name="T">Entity type</typeparam>
    //    /// <typeparam name="TPropType">Property type</typeparam>
    //    /// <param name="configSettings">ConfigSettings</param>
    //    /// <param name="keySelector">Key selector</param>
    //    /// <param name="storeId">Store identifier</param>
    //    /// <returns>true -configSetting exists; false - does not exist</returns>
    //    bool ConfigSettingExists<T, TPropType>(T configSettings,
    //        Expression<Func<T, TPropType>> keySelector, int storeId = 0);

    //    /// <summary>
    //    /// Save configSettings object
    //    /// </summary>
    //    /// <typeparam name="T">Type</typeparam>
    //    /// <param name="storeId">Store identifier</param>
    //    /// <param name="configSettings">ConfigSetting instance</param>
    //    void SaveConfigSetting<T>(T configSettings, int storeId = 0);

    //    /// <summary>
    //    /// Clear cache
    //    /// </summary>
    //    void ClearCache();
    }
}