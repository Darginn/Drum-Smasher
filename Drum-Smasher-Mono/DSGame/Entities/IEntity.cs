using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Drum_Smasher_Mono.DSGame.Entities
{
    public interface IEntity
    {
        public Guid Id { get; }
        public bool CanUpdate { get; set; }
        public bool CanDraw { get; set; }
        public bool IsDisabled { get; set; }
        public EntityManager EntityManager { get; }

        public void Draw(GameTime time);
        public void Update(GameTime time);

        public void Load();

        public void Unload()
        {
            EntityManager?.DeregisterEntity(this);
        }
    }
}
