using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Settings
{
    public class TaikoSettings : ISetting
    {
        public string Name { get; set; }

        public SettingsData Data;

        public TaikoSettings()
        {
            Name = "Taiko";
            Data = new SettingsData();
        }

        public void Load()
        {
            Data = SettingsManager.LoadSetting<SettingsData>(Name);

            if (Data == null)
                Data = new SettingsData();
        }

        public string Save()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(Data);
        }

        public class SettingsData : IDataStorage
        {
            public bool Autoplay { get; set; }
            public string Name { get; set; }
            public string Key1 { get; set; }
            public string Key2 { get; set; }
            public string Key3 { get; set; }
            public string Key4 { get; set; }

            public SettingsData()
            {
                Name = "Taiko";
                Autoplay = true;
                Key1 = "F";
                Key2 = "J";
                Key3 = "D";
                Key4 = "K";
            }
        }
    }
}
