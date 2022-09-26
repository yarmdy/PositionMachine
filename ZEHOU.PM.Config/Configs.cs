using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace ZEHOU.PM.Config
{
    public static class Configs
    {
        private static ConfigsNoNull _config = null;
        public static ConfigsNoNull Settings
        {
            get
            {
                if( _config == null) { _config = new ConfigsNoNull(); }
                return _config;
            }
        }
    }

    public class ConfigsNoNull { 
        public string this[string key] {
            get { 
                return ConfigurationManager.AppSettings[key]+"";
            }
            set
            {
                ConfigurationManager.AppSettings[key] = value+"";
                var cfg = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                if (cfg.AppSettings.Settings[key] == null)
                {
                    AppSettingsSection ass = (AppSettingsSection)cfg.GetSection("appSettings");
                    ass.Settings.Add(key, value+"");
                }
                else
                {
                    cfg.AppSettings.Settings[key].Value = value+"";
                }
                cfg.Save(ConfigurationSaveMode.Modified);
            }
        }
    }
}
