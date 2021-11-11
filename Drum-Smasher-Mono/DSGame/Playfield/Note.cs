using Drum_Smasher_Mono.DSGame.Entities;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Drum_Smasher_Mono.DSGame.Playfield
{
    public class Note : Entity
    {
        public NoteState State { get; private set; }
        public HitType HitType { get; private set; }

        public Note(EntityManager entityManager) : base(entityManager)
        {

        }

        public override void Draw(GameTime time)
        {
            throw new NotImplementedException();
        }

        public override void Load()
        {
            throw new NotImplementedException();
        }

        public override void Update(GameTime time)
        {
            throw new NotImplementedException();
        }
    }
}
