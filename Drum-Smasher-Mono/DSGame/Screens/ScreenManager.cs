using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Drum_Smasher_Mono.DSGame.Screens
{
    public class ScreenManager
    {
        /// <summary>
        /// The currently loaded screen
        /// </summary>
        public Screen ActiveScreen { get; private set; }

        /// <summary>
        /// Switches the current screen
        /// </summary>
        /// <param name="screen">New screen, can be null</param>
        /// <param name="destroyOld">Destroy the old screen</param>
        /// <param name="loadNew">Load the new screen</param>
        public void SwitchScreen(Screen screen, bool destroyOld, bool loadNew)
        {
            if (ActiveScreen != null)
            {
                ActiveScreen.OnScreenSwitch(screen);

                if (destroyOld)
                    ActiveScreen.Destroy();
            }

            ActiveScreen = screen;

            if (loadNew)
                ActiveScreen?.Load();
        }

        /// <summary>
        /// Switches the current screen
        /// </summary>
        public void SwitchScreen(Screen screen)
        {
            SwitchScreen(screen, false, false);
        }

        public void Update(GameTime time)
        {
            ActiveScreen?.Update(time);
        }

        public void Draw(GameTime time)
        {
            if (ActiveScreen == null)
            {
                GameClient.Instance.Graphics.Clear(Color.Black);
                return;
            }

            ActiveScreen.Draw(time);
        }
    }
}
