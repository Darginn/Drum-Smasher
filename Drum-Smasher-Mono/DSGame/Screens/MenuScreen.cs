using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Drum_Smasher_Mono.DSGame.Screens
{
    public class MenuScreen : Screen
    {
        SpriteFont _defaultFont;

        public MenuScreen(SpriteBatch sprites, GraphicsDevice graphics) : base(sprites, graphics)
        {

        }

        public override void Load()
        {
            _defaultFont = GameClient.Instance.Content.Load<SpriteFont>("Fonts/DefaultFont");
        }

        protected override void OnDraw(GameTime time)
        {

        }

        protected override void OnUpdate(GameTime time)
        {

        }
    }
}
