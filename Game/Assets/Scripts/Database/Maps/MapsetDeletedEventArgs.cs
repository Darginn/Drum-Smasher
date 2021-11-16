using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Database.Maps
{
    public class MapsetDeletedEventArgs
    {
        public Mapset Mapset { get; }

        public int Index { get; }

        public MapsetDeletedEventArgs(Mapset m, int index)
        {
            Mapset = m;
            Index = index;
        }
    }
}