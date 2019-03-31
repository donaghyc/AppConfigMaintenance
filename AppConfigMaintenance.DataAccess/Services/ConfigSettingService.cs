using AppConfigMaintenance.DataAccess.Models;
using AppConfigMaintenance.DataAccess.Repositories;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace AppConfigMaintenance.DataAccess.Services
{
    public class ConfigSettingService : IConfigSettingService
    {
        private const int AcquireLockTimeoutInMilliseconds = 1000;

        private readonly SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(1, 1);

        private readonly IConfigSettingRepository _configSettingRepository;
        //private readonly ICacheManager _cacheManager;

        public ConfigSettingService(IConfigSettingRepository configSettingRepository)
        //ICacheManager cacheManager)
        {
            this._configSettingRepository = configSettingRepository;
            //this._cacheManager = cacheManager;
        }

        public async Task<int> AddConfigSettingAsync(ConfigSettingModel configSetting, int? optionalDelay = null)
        {
            var acquiredLock = await _semaphoreSlim.WaitAsync(AcquireLockTimeoutInMilliseconds);

            if (!acquiredLock)
            {
                throw new TimeoutException("Another caller is already updating this setting.");
            }

            try
            {
                if (optionalDelay != null)
                {
                    await Task.Delay(optionalDelay.Value);
                }

                return await _configSettingRepository.AddConfigSettingAsync(configSetting);
            }
            finally
            {
                _semaphoreSlim.Release();
            }
        }

        public async Task<int> UpdateConfigSettingByNameAsync(string name, string value, int? optionalDelay = null)
        {
            var acquiredLock = await _semaphoreSlim.WaitAsync(AcquireLockTimeoutInMilliseconds);

            if (!acquiredLock)
            {
                throw new TimeoutException("Another caller is already updating this setting.");
            }

            try
            {
                if (optionalDelay != null)
                {
                    Thread.Sleep(optionalDelay.Value);
                }
                return await _configSettingRepository.UpdateConfigSettingByNameAsync(name, value);
            }
            finally
            {
                _semaphoreSlim.Release();
            }
        }
    }
}

// TODO: Ideally we would cache these settings in a Dictionary<string, string> and use the cache for reads. Keep the cache in sync with the database. 
// See the NopCommerce implementation for a good pattern to follow.

//        /// <summary>
//        /// Gets all configConfigSettings
//        /// </summary>
//        /// <returns>ConfigSettings</returns>
//        protected virtual IDictionary<string, IList<ConfigSettingModel>> GetAllConfigSettingsCached()
//        {
//            //cache
//            return _cacheManager.Get(NopConfigurationDefaults.ConfigSettingsAllCacheKey, () =>
//            {
//                //we use no tracking here for performance optimization
//                //anyway records are loaded only for read-only operations
//                var query = from s in _configConfigSettingRepository.TableNoTracking
//                            orderby s.Name, s.StoreId
//                            select s;
//                var configConfigSettings = query.ToList();
//                var dictionary = new Dictionary<string, IList<ConfigSettingModel>>();
//                foreach (var s in configConfigSettings)
//                {
//                    var resourceName = s.Name.ToLowerInvariant();
//                    var configConfigSettingModel = new ConfigSettingModel
//                    {
//                        Id = s.Id,
//                        Name = s.Name,
//                        Value = s.Value,
//                        StoreId = s.StoreId
//                    };
//                    if (!dictionary.ContainsKey(resourceName))
//                    {
//                        //first configConfigSetting
//                        dictionary.Add(resourceName, new List<ConfigSettingModel>
//                        {
//                            configConfigSettingModel
//                        });
//                    }
//                    else
//                    {
//                        //already added
//                        //most probably it's the configConfigSetting with the same name but for some certain store (storeId > 0)
//                        dictionary[resourceName].Add(configConfigSettingModel);
//                    }
//                }

//                return dictionary;
//            });
//        }

//        /// <summary>
//        /// Set configConfigSetting value
//        /// </summary>
//        /// <param name="type">Type</param>
//        /// <param name="key">Key</param>
//        /// <param name="value">Value</param>
//        /// <param name="storeId">Store identifier</param>
//        /// <param name="clearCache">A value indicating whether to clear cache after configConfigSetting update</param>
//        protected virtual void SetConfigSetting(Type type, string key, object value, int storeId = 0, bool clearCache = true)
//        {
//            if (key == null)
//                throw new ArgumentNullException(nameof(key));
//            key = key.Trim().ToLowerInvariant();
//            var valueStr = TypeDescriptor.GetConverter(type).ConvertToInvariantString(value);

//            var allConfigSettings = GetAllConfigSettingsCached();
//            var configConfigSettingModel = allConfigSettings.ContainsKey(key) ?
//                allConfigSettings[key].FirstOrDefault(x => x.StoreId == storeId) : null;
//            if (configConfigSettingModel != null)
//            {
//                //update
//                var configConfigSetting = GetConfigSettingById(configConfigSettingModel.Id);
//                configConfigSetting.Value = valueStr;
//                UpdateConfigSetting(configConfigSetting, clearCache);
//            }
//            else
//            {
//                //insert
//                var configConfigSetting = new ConfigSetting
//                {
//                    Name = key,
//                    Value = valueStr,
//                    StoreId = storeId
//                };
//                InsertConfigSetting(configConfigSetting, clearCache);
//            }
//        }

//        #endregion

//        #region Methods

//        /// <summary>
//        /// Adds a configConfigSetting
//        /// </summary>
//        /// <param name="configConfigSetting">ConfigSetting</param>
//        /// <param name="clearCache">A value indicating whether to clear cache after configConfigSetting update</param>
//        public virtual void InsertConfigSetting(ConfigSetting configConfigSetting, bool clearCache = true)
//        {
//            if (configConfigSetting == null)
//                throw new ArgumentNullException(nameof(configConfigSetting));

//            _configConfigSettingRepository.Insert(configConfigSetting);

//            //cache
//            if (clearCache)
//                _cacheManager.RemoveByPattern(NopConfigurationDefaults.ConfigSettingsPatternCacheKey);

//            //event notification
//            _eventPublisher.EntityInserted(configConfigSetting);
//        }

//        /// <summary>
//        /// Updates a configConfigSetting
//        /// </summary>
//        /// <param name="configConfigSetting">ConfigSetting</param>
//        /// <param name="clearCache">A value indicating whether to clear cache after configConfigSetting update</param>
//        public virtual void UpdateConfigSetting(ConfigSetting configConfigSetting, bool clearCache = true)
//        {
//            if (configConfigSetting == null)
//                throw new ArgumentNullException(nameof(configConfigSetting));

//            _configConfigSettingRepository.Update(configConfigSetting);

//            //cache
//            if (clearCache)
//                _cacheManager.RemoveByPattern(NopConfigurationDefaults.ConfigSettingsPatternCacheKey);

//            //event notification
//            _eventPublisher.EntityUpdated(configConfigSetting);
//        }

//        /// <summary>
//        /// Deletes a configConfigSetting
//        /// </summary>
//        /// <param name="configConfigSetting">ConfigSetting</param>
//        public virtual void DeleteConfigSetting(ConfigSetting configConfigSetting)
//        {
//            if (configConfigSetting == null)
//                throw new ArgumentNullException(nameof(configConfigSetting));

//            _configConfigSettingRepository.Delete(configConfigSetting);

//            //cache
//            _cacheManager.RemoveByPattern(NopConfigurationDefaults.ConfigSettingsPatternCacheKey);

//            //event notification
//            _eventPublisher.EntityDeleted(configConfigSetting);
//        }

//        /// <summary>
//        /// Deletes configConfigSettings
//        /// </summary>
//        /// <param name="configConfigSettings">ConfigSettings</param>
//        public virtual void DeleteConfigSettings(IList<ConfigSetting> configConfigSettings)
//        {
//            if (configConfigSettings == null)
//                throw new ArgumentNullException(nameof(configConfigSettings));

//            _configConfigSettingRepository.Delete(configConfigSettings);

//            //cache
//            _cacheManager.RemoveByPattern(NopConfigurationDefaults.ConfigSettingsPatternCacheKey);

//            //event notification
//            foreach (var configConfigSetting in configConfigSettings)
//            {
//                _eventPublisher.EntityDeleted(configConfigSetting);
//            }
//        }

//        /// <summary>
//        /// Gets a configConfigSetting by identifier
//        /// </summary>
//        /// <param name="configConfigSettingId">ConfigSetting identifier</param>
//        /// <returns>ConfigSetting</returns>
//        public virtual ConfigSetting GetConfigSettingById(int configConfigSettingId)
//        {
//            if (configConfigSettingId == 0)
//                return null;

//            return _configConfigSettingRepository.GetById(configConfigSettingId);
//        }

//        /// <summary>
//        /// Get configConfigSetting by key
//        /// </summary>
//        /// <param name="key">Key</param>
//        /// <param name="storeId">Store identifier</param>
//        /// <param name="loadSharedValueIfNotFound">A value indicating whether a shared (for all stores) value should be loaded if a value specific for a certain is not found</param>
//        /// <returns>ConfigSetting</returns>
//        public virtual ConfigSetting GetConfigSetting(string key, int storeId = 0, bool loadSharedValueIfNotFound = false)
//        {
//            if (string.IsNullOrEmpty(key))
//                return null;

//            var configConfigSettings = GetAllConfigSettingsCached();
//            key = key.Trim().ToLowerInvariant();
//            if (!configConfigSettings.ContainsKey(key))
//                return null;

//            var configConfigSettingsByKey = configConfigSettings[key];
//            var configConfigSetting = configConfigSettingsByKey.FirstOrDefault(x => x.StoreId == storeId);

//            //load shared value?
//            if (configConfigSetting == null && storeId > 0 && loadSharedValueIfNotFound)
//                configConfigSetting = configConfigSettingsByKey.FirstOrDefault(x => x.StoreId == 0);

//            return configConfigSetting != null ? GetConfigSettingById(configConfigSetting.Id) : null;
//        }

//        /// <summary>
//        /// Get configConfigSetting value by key
//        /// </summary>
//        /// <typeparam name="T">Type</typeparam>
//        /// <param name="key">Key</param>
//        /// <param name="defaultValue">Default value</param>
//        /// <param name="storeId">Store identifier</param>
//        /// <param name="loadSharedValueIfNotFound">A value indicating whether a shared (for all stores) value should be loaded if a value specific for a certain is not found</param>
//        /// <returns>ConfigSetting value</returns>
//        public virtual T GetConfigSettingByKey<T>(string key, T defaultValue = default(T),
//            int storeId = 0, bool loadSharedValueIfNotFound = false)
//        {
//            if (string.IsNullOrEmpty(key))
//                return defaultValue;

//            var configConfigSettings = GetAllConfigSettingsCached();
//            key = key.Trim().ToLowerInvariant();
//            if (!configConfigSettings.ContainsKey(key))
//                return defaultValue;

//            var configConfigSettingsByKey = configConfigSettings[key];
//            var configConfigSetting = configConfigSettingsByKey.FirstOrDefault(x => x.StoreId == storeId);

//            //load shared value?
//            if (configConfigSetting == null && storeId > 0 && loadSharedValueIfNotFound)
//                configConfigSetting = configConfigSettingsByKey.FirstOrDefault(x => x.StoreId == 0);

//            return configConfigSetting != null ? CommonHelper.To<T>(configConfigSetting.Value) : defaultValue;
//        }

//        /// <summary>
//        /// Set configConfigSetting value
//        /// </summary>
//        /// <typeparam name="T">Type</typeparam>
//        /// <param name="key">Key</param>
//        /// <param name="value">Value</param>
//        /// <param name="storeId">Store identifier</param>
//        /// <param name="clearCache">A value indicating whether to clear cache after configConfigSetting update</param>
//        public virtual void SetConfigSetting<T>(string key, T value, int storeId = 0, bool clearCache = true)
//        {
//            SetConfigSetting(typeof(T), key, value, storeId, clearCache);
//        }

//        /// <summary>
//        /// Gets all configConfigSettings
//        /// </summary>
//        /// <returns>ConfigSettings</returns>
//        public virtual IList<ConfigSetting> GetAllConfigSettings()
//        {
//            var query = from s in _configConfigSettingRepository.Table
//                        orderby s.Name, s.StoreId
//                        select s;
//            var configConfigSettings = query.ToList();
//            return configConfigSettings;
//        }

//        /// <summary>
//        /// Determines whether a configConfigSetting exists
//        /// </summary>
//        /// <typeparam name="T">Entity type</typeparam>
//        /// <typeparam name="TPropType">Property type</typeparam>
//        /// <param name="configConfigSettings">Entity</param>
//        /// <param name="keySelector">Key selector</param>
//        /// <param name="storeId">Store identifier</param>
//        /// <returns>true -configConfigSetting exists; false - does not exist</returns>
//        public virtual bool ConfigSettingExists<T, TPropType>(T configConfigSettings,
//            Expression<Func<T, TPropType>> keySelector, int storeId = 0)
//            where T : IConfigSettings, new()
//        {
//            var key = GetConfigSettingKey(configConfigSettings, keySelector);

//            var configConfigSetting = GetConfigSettingByKey<string>(key, storeId: storeId);
//            return configConfigSetting != null;
//        }

//        /// <summary>
//        /// Load configConfigSettings
//        /// </summary>
//        /// <typeparam name="T">Type</typeparam>
//        /// <param name="storeId">Store identifier for which configConfigSettings should be loaded</param>
//        public virtual T LoadConfigSetting<T>(int storeId = 0) where T : IConfigSettings, new()
//        {
//            return (T)LoadConfigSetting(typeof(T), storeId);
//        }

//        /// <summary>
//        /// Load configConfigSettings
//        /// </summary>
//        /// <param name="type">Type</param>
//        /// <param name="storeId">Store identifier for which configConfigSettings should be loaded</param>
//        public virtual IConfigSettings LoadConfigSetting(Type type, int storeId = 0)
//        {
//            var configConfigSettings = Activator.CreateInstance(type);

//            foreach (var prop in type.GetProperties())
//            {
//                // get properties we can read and write to
//                if (!prop.CanRead || !prop.CanWrite)
//                    continue;

//                var key = type.Name + "." + prop.Name;
//                //load by store
//                var configConfigSetting = GetConfigSettingByKey<string>(key, storeId: storeId, loadSharedValueIfNotFound: true);
//                if (configConfigSetting == null)
//                    continue;

//                if (!TypeDescriptor.GetConverter(prop.PropertyType).CanConvertFrom(typeof(string)))
//                    continue;

//                if (!TypeDescriptor.GetConverter(prop.PropertyType).IsValid(configConfigSetting))
//                    continue;

//                var value = TypeDescriptor.GetConverter(prop.PropertyType).ConvertFromInvariantString(configConfigSetting);

//                //set property
//                prop.SetValue(configConfigSettings, value, null);
//            }

//            return configConfigSettings as IConfigSettings;
//        }

//        /// <summary>
//        /// Save configConfigSettings object
//        /// </summary>
//        /// <typeparam name="T">Type</typeparam>
//        /// <param name="storeId">Store identifier</param>
//        /// <param name="configConfigSettings">ConfigSetting instance</param>
//        public virtual void SaveConfigSetting<T>(T configConfigSettings, int storeId = 0) where T : IConfigSettings, new()
//        {
//            /* We do not clear cache after each configConfigSetting update.
//             * This behavior can increase performance because cached configConfigSettings will not be cleared 
//             * and loaded from database after each update */
//            foreach (var prop in typeof(T).GetProperties())
//            {
//                // get properties we can read and write to
//                if (!prop.CanRead || !prop.CanWrite)
//                    continue;

//                if (!TypeDescriptor.GetConverter(prop.PropertyType).CanConvertFrom(typeof(string)))
//                    continue;

//                var key = typeof(T).Name + "." + prop.Name;
//                var value = prop.GetValue(configConfigSettings, null);
//                if (value != null)
//                    SetConfigSetting(prop.PropertyType, key, value, storeId, false);
//                else
//                    SetConfigSetting(key, string.Empty, storeId, false);
//            }

//            //and now clear cache
//            ClearCache();
//        }

//        /// <summary>
//        /// Save configConfigSettings object
//        /// </summary>
//        /// <typeparam name="T">Entity type</typeparam>
//        /// <typeparam name="TPropType">Property type</typeparam>
//        /// <param name="configConfigSettings">ConfigSettings</param>
//        /// <param name="keySelector">Key selector</param>
//        /// <param name="storeId">Store ID</param>
//        /// <param name="clearCache">A value indicating whether to clear cache after configConfigSetting update</param>
//        public virtual void SaveConfigSetting<T, TPropType>(T configConfigSettings,
//            Expression<Func<T, TPropType>> keySelector,
//            int storeId = 0, bool clearCache = true) where T : IConfigSettings, new()
//        {
//            if (!(keySelector.Body is MemberExpression member))
//            {
//                throw new ArgumentException(string.Format(
//                    "Expression '{0}' refers to a method, not a property.",
//                    keySelector));
//            }

//            var propInfo = member.Member as PropertyInfo;
//            if (propInfo == null)
//            {
//                throw new ArgumentException(string.Format(
//                       "Expression '{0}' refers to a field, not a property.",
//                       keySelector));
//            }

//            var key = GetConfigSettingKey(configConfigSettings, keySelector);
//            var value = (TPropType)propInfo.GetValue(configConfigSettings, null);
//            if (value != null)
//                SetConfigSetting(key, value, storeId, clearCache);
//            else
//                SetConfigSetting(key, string.Empty, storeId, clearCache);
//        }

//        /// <summary>
//        /// Save configConfigSettings object (per store). If the configConfigSetting is not overridden per store then it'll be delete
//        /// </summary>
//        /// <typeparam name="T">Entity type</typeparam>
//        /// <typeparam name="TPropType">Property type</typeparam>
//        /// <param name="configConfigSettings">ConfigSettings</param>
//        /// <param name="keySelector">Key selector</param>
//        /// <param name="overrideForStore">A value indicating whether to configConfigSetting is overridden in some store</param>
//        /// <param name="storeId">Store ID</param>
//        /// <param name="clearCache">A value indicating whether to clear cache after configConfigSetting update</param>
//        public virtual void SaveConfigSettingOverridablePerStore<T, TPropType>(T configConfigSettings,
//            Expression<Func<T, TPropType>> keySelector,
//            bool overrideForStore, int storeId = 0, bool clearCache = true) where T : IConfigSettings, new()
//        {
//            if (overrideForStore || storeId == 0)
//                SaveConfigSetting(configConfigSettings, keySelector, storeId, clearCache);
//            else if (storeId > 0)
//                DeleteConfigSetting(configConfigSettings, keySelector, storeId);
//        }

//        /// <summary>
//        /// Delete all configConfigSettings
//        /// </summary>
//        /// <typeparam name="T">Type</typeparam>
//        public virtual void DeleteConfigSetting<T>() where T : IConfigSettings, new()
//        {
//            var configConfigSettingsToDelete = new List<ConfigSetting>();
//            var allConfigSettings = GetAllConfigSettings();
//            foreach (var prop in typeof(T).GetProperties())
//            {
//                var key = typeof(T).Name + "." + prop.Name;
//                configConfigSettingsToDelete.AddRange(allConfigSettings.Where(x => x.Name.Equals(key, StringComparison.InvariantCultureIgnoreCase)));
//            }

//            DeleteConfigSettings(configConfigSettingsToDelete);
//        }

//        /// <summary>
//        /// Delete configConfigSettings object
//        /// </summary>
//        /// <typeparam name="T">Entity type</typeparam>
//        /// <typeparam name="TPropType">Property type</typeparam>
//        /// <param name="configConfigSettings">ConfigSettings</param>
//        /// <param name="keySelector">Key selector</param>
//        /// <param name="storeId">Store ID</param>
//        public virtual void DeleteConfigSetting<T, TPropType>(T configConfigSettings,
//            Expression<Func<T, TPropType>> keySelector, int storeId = 0) where T : IConfigSettings, new()
//        {
//            var key = GetConfigSettingKey(configConfigSettings, keySelector);
//            key = key.Trim().ToLowerInvariant();

//            var allConfigSettings = GetAllConfigSettingsCached();
//            var configConfigSettingModel = allConfigSettings.ContainsKey(key) ?
//                allConfigSettings[key].FirstOrDefault(x => x.StoreId == storeId) : null;
//            if (configConfigSettingModel == null)
//                return;

//            //update
//            var configConfigSetting = GetConfigSettingById(configConfigSettingModel.Id);
//            DeleteConfigSetting(configConfigSetting);
//        }

//        /// <summary>
//        /// Clear cache
//        /// </summary>
//        public virtual void ClearCache()
//        {
//            _cacheManager.RemoveByPattern(NopConfigurationDefaults.ConfigSettingsPatternCacheKey);
//        }

//        /// <summary>
//        /// Get configConfigSetting key (stored into database)
//        /// </summary>
//        /// <typeparam name="TConfigSettings">Type of configConfigSettings</typeparam>
//        /// <typeparam name="T">Property type</typeparam>
//        /// <param name="configConfigSettings">ConfigSettings</param>
//        /// <param name="keySelector">Key selector</param>
//        /// <returns>Key</returns>
//        public virtual string GetConfigSettingKey<TConfigSettings, T>(TConfigSettings configConfigSettings, Expression<Func<TConfigSettings, T>> keySelector)
//            where TConfigSettings : IConfigSettings, new()
//        {
//            if (!(keySelector.Body is MemberExpression member))
//                throw new ArgumentException($"Expression '{keySelector}' refers to a method, not a property.");

//            if (!(member.Member is PropertyInfo propInfo))
//                throw new ArgumentException($"Expression '{keySelector}' refers to a field, not a property.");

//            var key = $"{typeof(TConfigSettings).Name}.{propInfo.Name}";

//            return key;
//        }
//        #endregion
//    }
//}