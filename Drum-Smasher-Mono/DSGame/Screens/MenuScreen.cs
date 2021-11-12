using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;
using Myra;
using Myra.Graphics2D.UI;

namespace Drum_Smasher_Mono.DSGame.Screens
{
    public class MenuScreen : Screen
    {
        SpriteFont _defaultFont;

        Desktop _desktop;
        

        public MenuScreen(SpriteBatch sprites, GraphicsDevice graphics) : base(sprites, graphics)
        {

        }

        public override void Load()
        {
            _defaultFont = GameClient.Instance.Content.Load<SpriteFont>("Fonts/DefaultFont");
            _desktop = new Desktop();

            Panel panel = new Panel();
 
            panel.AddChild(new Label()
            {
                Text = "Drum-Smasher",
                TextColor = Color.Cyan,
                HorizontalAlignment = HorizontalAlignment.Center,
                Top = 30
            });

            _desktop.Root = panel;
        }

        protected override void OnDraw(GameTime time)
        {
            _desktop.Render();
        }

        protected override void OnUpdate(GameTime time)
        {

        }
    }
}
