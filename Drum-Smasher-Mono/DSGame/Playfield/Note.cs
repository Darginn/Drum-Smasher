using Drum_Smasher_Mono.DSGame.Entities;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Drum_Smasher_Mono.DSGame.Playfield
{
    public class Note : IEntity
    {
        public Guid Id { get; }

        public bool CanUpdate { get; set; }
        public bool CanDraw { get; set; }
        public bool IsDisabled { get; set; }

        public EntityManager EntityManager { get; }

        public Note(EntityManager manager)
        {
            Id = Guid.NewGuid();
            EntityManager = manager;
        }

        public void Draw(GameTime time)
        {

        }

        public void Load()
        {

        }

        public void Unload()
        {
            ((IEntity)this).Unload();
        }

        public void Update(GameTime time)
        {
            throw new NotImplementedException();
        }
    }
}
