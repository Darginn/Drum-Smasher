using DrumSmasher.GameInput;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrumSmasher.Settings
{
    public class TitleScreenSettings : ISetting
    {
        public string Name { get; set; }
        


        public TitleScreenData Data;

        public TitleScreenSettings()
        {
            Name = "TitleScreen";
            Data = new TitleScreenData();
        }

        public void Load()
        {
            Data = SettingsManager.LoadSetting<TitleScreenData>("TitleScreen");

            if (Data == null)
                Data = new TitleScreenData();
        }

        public string Save()
        {
            if (string.IsNullOrEmpty(Data.DefaultConsoleMessage))
                return null;

            return Newtonsoft.Json.JsonConvert.SerializeObject(Data);
        }

        public class TitleScreenData
        {
            public string DefaultConsoleMessage { get; set; }
            public string ScreenWidth { get; set; }
            public string ScreenHeight { get; set; }
            public string RefreshRate { get; set; }
            public bool Fullscreen { get; set; }
            public bool Vsync { get; set; }
            public ButtonController LeftCenter { get; set; }
            public ButtonController RightCenter { get; set; }
            public ButtonController LeftRim { get; set; }
            public ButtonController RightRim { get; set; }
        }
    }
}
