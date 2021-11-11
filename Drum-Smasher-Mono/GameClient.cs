using Drum_Smasher_Mono.DSGame.Config;
using Drum_Smasher_Mono.DSGame.Screens;
using Drum_Smasher_Mono.DSGame.Sound;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Reflection;

namespace Drum_Smasher_Mono
{
    public class GameClient : Game
    {
        public static GameClient Instance { get; private set; }
        public static ScreenManager ScreenManager { get; private set; }
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
        private static double TimeRunningPrecise { get; set; }

        /// <summary>
        /// The amount of time the game has been running.
        /// </summary>
        public static long TimeRunning => (long)TimeRunningPrecise;




        public GraphicsDevice Graphics => GraphicsDevice;
        public SpriteBatch Sprites => _spriteBatch;

        GraphicsDeviceManager _graphics;
        SpriteBatch _spriteBatch;
         
        public GameClient()
        {
            Instance = this;
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        /// <summary>
        /// Toggles fullscreen, do not call this inside the constructor
        /// </summary>
        public void ToggleFullScreen()
        {
            _graphics.IsFullScreen = !_graphics.IsFullScreen;
            _graphics.ApplyChanges();
        }

        /// <summary>
        /// Resizes the screen, do not call this inside the constructor
        /// </summary>
        public void ResizeScreen(int width, int height)
        {
            _graphics.PreferredBackBufferWidth = width;
            _graphics.PreferredBackBufferHeight = height;
            _graphics.ApplyChanges();
        }

        protected override void Initialize()
        {
            ScreenManager = new ScreenManager();
            Sound = new SoundConductor();

            ConfigManager.Initialize();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            ScreenManager.SwitchScreen(new MenuScreen(Sprites, Graphics));
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
            else if (HotKeyTable.IsKeyDown(HotKey.FullScreen))
            {
                ToggleFullScreen();
            }

            ScreenManager.Update(gameTime);
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            ScreenManager.Draw(gameTime);
            base.Draw(gameTime);
        }
    }
}
