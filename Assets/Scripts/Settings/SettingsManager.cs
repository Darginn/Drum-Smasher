using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace DrumSmasher.Settings
{
    public static class SettingsManager
    {
        public static IReadOnlyDictionary<string, ISetting> SettingsStorage
        {
            get
            {
                if (_settings == null)
                    _settings = new Dictionary<string, ISetting>();

                return _settings;
            }
        }

        private static Dictionary<string, ISetting> _settings;

        public static ISetting AddOrUpdate(ISetting setting)
        {
            if (SettingsStorage.ContainsKey(setting.Name))
                _settings[setting.Name] = setting;
            else
                _settings.Add(setting.Name, setting);

            return setting;
        }

        public static ISetting Remove(ISetting setting)
        {
            if (!SettingsStorage.ContainsKey(setting.Name))
                return setting;

            ISetting result = _settings[setting.Name];
            _settings.Remove(setting.Name);

            return result;
        }

        public static T LoadSetting<T>(string name)
        {
            FileInfo settingsFile = new FileInfo(Path.Combine(Application.dataPath + "/", "Settings/", name + ".setting"));

            if (!settingsFile.Exists)
                return default(T);

            string json = File.ReadAllText(settingsFile.FullName);

            T val = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(json);
            
            return val;
        }

        public static void SaveSettings()
        {
            List<ISetting> settings = SettingsStorage.Values.ToList();
           
            foreach(ISetting setting in settings)
            {
                string json = setting.Save();

                if (string.IsNullOrEmpty(json))
                    continue;

                string path = Path.Combine(Application.dataPath + "/", "Settings/", setting.Name + ".setting");

                if (File.Exists(path))
                    File.Delete(path);

                File.WriteAllText(path, json);
            }
        }

        private class SaveData
        {
            public string Name { get; set; }
            public string Data { get; set; }

            public SaveData(string name, string data)
            {
                Name = name;
                Data = data;
            }

            public SaveData()
            {
            }
        }
    }

    public interface ISetting
    {
        string Name { get; set; }

        string Save();
        void Load();
    }
}
