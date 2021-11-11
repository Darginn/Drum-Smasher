using Drum_Smasher_Mono.DSGame.Entities;
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
        public TimeSpan Position => GameClient.Sound.CurrentPosition;
        public TimeSpan Length => GameClient.Sound.Length;

        NoteController _noteController;

        public PlayfieldScreen(SpriteBatch sprites, GraphicsDevice graphics) : base(sprites, graphics)
        {

        }

        public override void Load()
        {
            _noteController = new NoteController(Entities, this);
            _noteController.Load();
            _noteController.Register();
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
