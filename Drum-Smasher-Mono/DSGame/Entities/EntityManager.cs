//#define ALLOW_THREADED

using Microsoft.Xna.Framework;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace Drum_Smasher_Mono.DSGame.Entities
{
#if ALLOW_THREADED
    /// <summary>
    /// Threadsafe entity manager
    /// </summary>
#endif
    public class EntityManager
    {
        Dictionary<Guid, IEntity> _entities;

#if ALLOW_THREADED
        readonly object _syncroot;
#endif

        public EntityManager()
        {
#if ALLOW_THREADED
            _syncroot = new object();
#endif
            _entities = new Dictionary<Guid, IEntity>();
        }

        public void RegisterEntity(IEntity ent)
        {
#if ALLOW_THREADED
            lock (_syncroot)
            {
#endif
                if (ent == null || _entities.ContainsKey(ent.Id))
                    return;

                _entities.Add(ent.Id, ent);
#if ALLOW_THREADED
            }
#endif
        }

        public void DeregisterEntity(IEntity ent)
        {
#if ALLOW_THREADED
            lock (_syncroot)
            {
#endif
                if (ent == null)
                    return;

                _entities.Remove(ent.Id);
#if ALLOW_THREADED
            }
#endif
        }

        public void DrawEntities(GameTime time)
        {
#if ALLOW_THREADED
            lock (_syncroot)
            {
#endif

            foreach (IEntity ent in _entities.Values)
                {
                    if (ent.IsDisabled || !ent.CanDraw)
                        continue;

                    ent.Draw(time);
                }

#if ALLOW_THREADED
            }
#endif
        }

        public void UpdateEntities(GameTime time)
        {
#if ALLOW_THREADED
            lock (_syncroot)
            {
#endif

                foreach (IEntity ent in _entities.Values)
                {
                    if (ent.IsDisabled || !ent.CanUpdate)
                        continue;

                    ent.Update(time);
                }

#if ALLOW_THREADED
            }
#endif

        }

        public void DestroyAllEntities()
        {
#if ALLOW_THREADED
            lock (_syncroot)
            {
#endif
                foreach (IEntity ent in _entities.Values)
                {
                    ent.Unload();
                }

                _entities.Clear();
#if ALLOW_THREADED
            }
#endif
        }
    }
}
