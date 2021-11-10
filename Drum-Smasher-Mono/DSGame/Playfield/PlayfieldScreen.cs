using Drum_Smasher_Mono.DSGame.Screens;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Drum_Smasher_Mono.DSGame.Playfield
{
    public class PlayfieldScreen : Screen
    {
        public PlayfieldScreen(SpriteBatch sprites, GraphicsDevice graphics) : base(sprites, graphics)
        {

        }

        public override void Load()
        {
            //Load and add note to our entities
            //Note n = new Note(Entities);
            //Entities.RegisterEntity(n);
        }

        protected override void OnDraw(GameTime time)
        {

        }

        protected override void OnUpdate(GameTime time)
        {

        }

        public override void Destroy()
        {
            // destroy stuff here

            base.Destroy();
        }
    }
}
