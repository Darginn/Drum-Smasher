using Drum_Smasher_Mono.DSGame.Screens;
using Drum_Smasher_Mono.DSGame.Sound;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Drum_Smasher_Mono
{
    public class GameClient : Game
    {
        public static GameClient Instance { get; private set; }
        public static ScreenManager ScreenManager { get; private set; }
        public static SoundConductor Sound { get; private set; }

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
