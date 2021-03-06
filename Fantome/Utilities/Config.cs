﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MaterialDesignColors;
using MaterialDesignThemes.Wpf;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

namespace Fantome.Utilities
{
    public static class Config
    {
        public const string CONFIG_FILE = "CONFIG.json";

        private static readonly Dictionary<string, object> _defaultConfig = new Dictionary<string, object>
        {
            { "LeagueLocation", "" },
            { "LoggingPattern", "{Timestamp:dd-MM-yyyy HH:mm:ss.fff} | [{Level}] |  {Message:lj}{NewLine}{Exception}" },
            { "ModListType", 0 },
            { "ParallelWadInstallation", false },
            { "PackWadFolders", true },
            { "InstallAddedMods", true },
            { "LaunchOnStartup", false },
            { "IsDarkTheme", true },
            { "PrimaryColor", PrimaryColor.Teal },
            { "SecondaryColor", SecondaryColor.Lime }
        };
        private static Dictionary<string, object> _config = new Dictionary<string, object>();

        public static T Get<T>(string key)
        {
            if (!_config.ContainsKey(key))
            {
                return GetDefault<T>(key);
            }

            if (typeof(T).BaseType == typeof(Enum))
            {
                return (T)Enum.Parse(typeof(T), _config[key].ToString());
            }

            return (T)Convert.ChangeType(_config[key], typeof(T));
        }
        public static T GetDefault<T>(string key)
        {
            return (T)_config[key];
        }
        public static void Set(string key, object value)
        {
            if (_config.ContainsKey(key))
            {
                _config[key] = value;
            }
            else
            {
                _config.Add(key, value);
            }

            Write();
        }

        public static void Load(string fileLocation = CONFIG_FILE)
        {
            if (File.Exists(CONFIG_FILE))
            {
                Deserialize(File.ReadAllText(fileLocation));

                //Check if config is outdated
                foreach (KeyValuePair<string, object> configEntry in _defaultConfig)
                {
                    if (!_config.ContainsKey(configEntry.Key))
                    {
                        _config.Add(configEntry.Key, configEntry.Value);
                    }
                }
            }
            else
            {
                _config = _defaultConfig;
            }
        }
        public static void Write(string fileLocation = CONFIG_FILE)
        {
            File.WriteAllText(fileLocation, Serialize());
        }
        public static string Serialize()
        {
            return JsonConvert.SerializeObject(_config, Formatting.Indented);
        }
        public static void Deserialize(string json)
        {
            _config = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
        }
    }
}
