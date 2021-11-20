using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.TaikoGame.Notes
{
    public static class NotePool
    {
        public static int Capacity { get; private set; }
        public static int NotesInUse { get; private set; }
        public static int NotesAvailable => _pool.Count;

        static Queue<Note> _pool;

        static NotePool()
        {
            Capacity = 20;
            _pool = new Queue<Note>();
        }

        public static Note RentOne()
        {
            if (_pool.Count == 0)
                Grow(Capacity / 4);

            Note n = _pool.Dequeue();
            NotesInUse++;

            return n;
        }

        public static void Return(Note n)
        {
            n.gameObject.SetActive(false);
            _pool.Enqueue(n);
            NotesInUse--;
        }

        static void Grow(int count)
        {
            for (int i = 0; i < count; i++)
            {
                GameObject nobj = new GameObject();
                Note n = nobj.AddComponent<Note>();

                nobj.SetActive(false);
                _pool.Enqueue(n);
            }

            Capacity += count;
        }
    }
}
