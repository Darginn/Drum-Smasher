using Drum_Smasher_Mono.DSGame.Config;
using Drum_Smasher_Mono.DSGame.Sound;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Reflection;
using Nez;

namespace Drum_Smasher_Mono
{
    public class GameClient : Core
    {
        public static SoundConductor Sound { get; private set; }

        /// <summary>
        /// Unique identifier of the client's assembly version.
        /// </summary>
        protected AssemblyName AssemblyName => Assembly.GetEntryAssembly()?.GetName() ?? new AssemblyName { Version = new System.Version() };

        /// <summary>
        /// Determines if the build is deployed/an official release.
        /// By default, it's 0.0.0.0 - Anything else is considered deployed.
        /// </summary>
        public bool IsDeployedBuild => AssemblyName.Version.Major != 0 || AssemblyName.Version.Minor != 0 || AssemblyName.Version.Revision != 0 || AssemblyName.Version.Build != 0;

        /// <summary>
        /// Stringified version name of the client.
        /// </summary>
        public string Version
        {
            get
            {
                if (!IsDeployedBuild)
                    return "Local Development Build";

                var assembly = AssemblyName;
                return $@"{assembly.Version.Major}.{assembly.Version.Minor}.{assembly.Version.Build}";
            }
        }

        /// <summary>
        /// The amount of time the game has been running, as a double so it doesn't break above 1000 FPS and maintains precision.
        /// </summary>
        static double TimeRunningPrecise { get; set; }

        /// <summary>
        /// The amount of time the game has been running.
        /// </summary>
        public static long TimeRunning => (long)TimeRunningPrecise;

        public GameClient()
        {
        }

        protected override void Initialize()
        {
            base.Initialize();

            Sound = new SoundConductor();
            ConfigManager.Initialize();
            Scene = new Scenes.MenuScene();
        }

        protected override void LoadContent()
        {
            base.LoadContent();

        }

        protected override void Update(GameTime gameTime)
        {
            // Update game clock
            TimeRunningPrecise += gameTime.ElapsedGameTime.TotalMilliseconds;

            if (HotKeyTable.IsKeyDown(HotKey.ExitGame))
            {
                Exit();
                return;
            }

            base.Update(gameTime);
        }
    }
}
