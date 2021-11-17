

namespace Assets.Scripts.Database.Maps
{
    public class MapDeletedEventArgs
    {
        public Map Map { get; }

        public int Index { get; }

        public MapDeletedEventArgs(Map map, int index)
        {
            Map = map;
            Index = index;
        }
    }
}