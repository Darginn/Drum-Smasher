using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrumSmasher.Settings
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
            public float ApproachRate { get; set; }
            public bool Autoplay { get; set; }
            public string Name { get; set; }

            public SettingsData()
            {
                Name = "Taiko";
                ApproachRate = 6;
                Autoplay = false;
            }
        }
    }
}
