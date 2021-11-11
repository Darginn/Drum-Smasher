using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Drum_Smasher_Mono.DSGame.Entities
{
    public abstract class Entity : IEntity
    {
        public Guid Id { get; }

        public virtual bool CanUpdate { get; set; }
        public virtual bool CanDraw { get; set; }
        public virtual bool IsDisabled { get; set; }

        public EntityManager EntityManager { get; }

        public Entity(EntityManager entityManager)
        {
            if (entityManager == null)
                throw new InvalidOperationException("EntityManager cannot be null");

            Id = Guid.NewGuid();
            EntityManager = entityManager;
        }

        public void Register()
        {
            EntityManager.RegisterEntity(this);
        }

        public void Deregister()
        {
            EntityManager.DeregisterEntity(this);
        }

        public abstract void Draw(GameTime time);

        public abstract void Load();

        public virtual void Unload()
        {
            ((IEntity)this).Unload();
        }

        public abstract void Update(GameTime time);
    }
}
