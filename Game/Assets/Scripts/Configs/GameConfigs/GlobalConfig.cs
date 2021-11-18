using Assets.Scripts.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Configs.GameConfigs
{
    public class GlobalConfig : BaseConfig<GlobalConfig>
    {
        public string Name { get; set; }
        public string DefaultConsoleMessage { get; set; }
        public int ScreenWidth { get; set; }
        public int ScreenHeight { get; set; }
        public int RefreshRate { get; set; }
        public bool Fullscreen { get; set; }
        public bool Vsync { get; set; }
        public int ResolutionIndex { get; set; }
        public string SongDirectory { get; set; }
        public OrderMapsetsBy SelectOrderMapsetsBy { get; set; }
        public string GameDirectory { get; set; }
        public int FPSMenu { get; set; }
        public int FPSInGame { get; set; }
        public bool IsDeveloperMode { get; set; }

        public GlobalConfig()
        {
            Name = "GlobalConfig";
            DefaultConsoleMessage = "Hello world";
            ScreenWidth = Screen.currentResolution.width;
            ScreenHeight = Screen.currentResolution.height;
            RefreshRate = Screen.currentResolution.refreshRate;
            Fullscreen = true;
            Vsync = false;
            ResolutionIndex = 0;
            SongDirectory = Application.dataPath + "/../Charts";
            SelectOrderMapsetsBy = OrderMapsetsBy.Title;
            GameDirectory = Application.dataPath;

#if UNITY_EDITOR
            IsDeveloperMode = true;
#else
            IsDeveloperMode = false;
#endif

            FPSMenu = 0;
            FPSInGame = 0;
        }
    }
}
