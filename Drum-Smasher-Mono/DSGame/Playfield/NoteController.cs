using Drum_Smasher_Mono.DSGame.Entities;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Drum_Smasher_Mono.DSGame.Playfield
{
    public sealed class NoteController : Entity
    {
        PlayfieldScreen _playfieldScreen;
        List<Note> _notes;

        public NoteController(EntityManager entityManager, PlayfieldScreen playfieldScreen) : base(entityManager)
        {
            _playfieldScreen = playfieldScreen;
            _notes = new List<Note>();

            CanUpdate = true;
        }

        public override void Draw(GameTime time)
        {

        }

        public override void Load()
        {

        }

        public override void Update(GameTime time)
        {
            CheckNotes();
            CheckNoteSpawn(time);
        }

        public void ClearNotes()
        {
            if (_notes.Count == 0)
                return;

            for (int i = 0; i < _notes.Count; i++)
            {
                Note n = _notes[i];
                n.Unload();
            }

            _notes.Clear();
        }

        void CheckNotes()
        {
            if (_notes.Count > 0)
            {
                Note n = _notes[0];

                switch (n.State)
                {
                    case NoteState.Missed:
                    case NoteState.Default:
                        break;

                    case NoteState.Hit:
                        switch (n.HitType)
                        {
                            case HitType.Good:

                                break;

                            case HitType.Bad:

                                break;

                            case HitType.Miss:

                                break;
                        }
                        goto case NoteState.ReachedEnd;

                    case NoteState.ReachedEnd:
                        _notes.RemoveAt(0);
                        n.Unload();
                        break;
                }
            }
        }

        void CheckNoteSpawn(GameTime time)
        {
            // TODO: check if we should spawn the next note
        }
    }

}
