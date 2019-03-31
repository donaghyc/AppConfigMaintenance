using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppConfigMaintenance.DataAccess.Models
{
    [Serializable]
    public class ConfigSettingModel
    {
        public string Name { get; set; }

        public string Value { get; set; }
    }
}
