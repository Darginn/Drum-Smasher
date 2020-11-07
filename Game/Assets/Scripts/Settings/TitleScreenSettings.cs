using DrumSmasher.GameInput;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

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

        public class TitleScreenData : IDataStorage
        {
            public string Name { get; set; }
            public string DefaultConsoleMessage { get; set; }
            public int ScreenWidth { get; set; }
            public int ScreenHeight { get; set; }
            public int RefreshRate { get; set; }
            public bool Fullscreen { get; set; }
            public bool Vsync { get; set; }
            public int ResolutionIndex { get; set; }
            //public ButtonController LeftCenter { get; set; }
            //public ButtonController RightCenter { get; set; }
            //public ButtonController LeftRim { get; set; }
            //public ButtonController RightRim { get; set; }
            public string ChartPath { get; set; }
            public int FPSMenu { get; set; }
            public int FPSInGame { get; set; }

            public TitleScreenData()
            {
                Name = "TitleScreen";
                DefaultConsoleMessage = "Hello world";
                ScreenWidth = Screen.currentResolution.width;
                ScreenHeight = Screen.currentResolution.height;
                RefreshRate = Screen.currentResolution.refreshRate;
                Fullscreen = true;
                Vsync = false;
                ResolutionIndex = 0;
                //LeftRim = 
                //LeftCenter = 
                //RightCenter = 
                //RightRim =
                ChartPath = Application.dataPath + "/../Charts";

                FPSMenu = 0;
                FPSInGame = 0;
            }
        }
    }
}
