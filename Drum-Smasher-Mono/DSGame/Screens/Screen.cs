using Drum_Smasher_Mono.DSGame.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Drum_Smasher_Mono.DSGame.Screens
{
    public abstract class Screen
    {
        /// <summary>
        /// Screen Id
        /// </summary>
        public Guid Id { get; }
        public SpriteBatch Sprites { get; set; }
        public GraphicsDevice Graphics { get; set; }
        public EntityManager Entities { get; }

        /// <summary>
        /// If true does not call <see cref="OnUpdate(GameTime)"/>
        /// </summary>
        public bool DisableUpdating { get; set; }
        /// <summary>
        /// If true does not call <see cref="OnDraw(GameTime)"/>
        /// </summary>
        public bool DisableDrawing { get; set; }

        /// <summary>
        /// Background color
        /// </summary>
        public Color ClearColor { get; set; } = Color.Black;

        public Screen(SpriteBatch sprites, GraphicsDevice graphics)
        {
            Id = Guid.NewGuid();
            Sprites = sprites;
            Graphics = graphics;
            Entities = new EntityManager();
        }

        /// <summary>
        /// Sets this screen as the current active screen
        /// </summary>
        public void SwitchToScreen()
        {
            GameClient.ScreenManager.SwitchScreen(this);
        }

        public void Update(GameTime time)
        {
            if (DisableUpdating)
                return;

            OnUpdate(time);
            Entities.UpdateEntities(time);
        }

        public void Draw(GameTime time)
        {
            // do not draw anything if our spritebatch or graphicsdevice is null
            if (DisableDrawing || Sprites == null || Graphics == null)
                return;

            Graphics.Clear(ClearColor);
            OnDraw(time);
            Entities.DrawEntities(time);
        }

        /// <summary>
        /// Called when the current screen is switched out by another screen
        /// </summary>
        public virtual void OnScreenSwitch(Screen newScreen)
        {

        }

        /// <summary>
        /// Load/Initialize the screen
        /// </summary>
        public abstract void Load();

        /// <summary>
        /// Destroy/Unload the screen
        /// </summary>
        public virtual void Destroy()
        {
            Entities.DestroyAllEntities();
        }

        /// <summary>
        /// Called when the screen is updated
        /// </summary>
        protected abstract void OnUpdate(GameTime time);
        
        /// <summary>
        /// Called when the screen is drawn
        /// </summary>
        protected abstract void OnDraw(GameTime time);
    }
}
