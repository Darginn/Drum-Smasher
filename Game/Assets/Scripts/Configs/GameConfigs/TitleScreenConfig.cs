using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Configs.GameConfigs
{
    public class TitleScreenConfig : BaseConfig<TitleScreenConfig>
    {
        public string Name { get; set; }
        public string DefaultConsoleMessage { get; set; }
        public int ScreenWidth { get; set; }
        public int ScreenHeight { get; set; }
        public int RefreshRate { get; set; }
        public bool Fullscreen { get; set; }
        public bool Vsync { get; set; }
        public int ResolutionIndex { get; set; }
        public string ChartPath { get; set; }
        public int FPSMenu { get; set; }
        public int FPSInGame { get; set; }

        public TitleScreenConfig()
        {
            Name = "TitleScreen";
            DefaultConsoleMessage = "Hello world";
            ScreenWidth = Screen.currentResolution.width;
            ScreenHeight = Screen.currentResolution.height;
            RefreshRate = Screen.currentResolution.refreshRate;
            Fullscreen = true;
            Vsync = false;
            ResolutionIndex = 0;
            ChartPath = Application.dataPath + "/../Charts";

            FPSMenu = 0;
            FPSInGame = 0;
        }
    }
}
