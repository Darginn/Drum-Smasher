

namespace Assets.Scripts.Database.Maps
{
    public class MapUpdatedEventArgs
    {
        public Map Original { get; }

        public Map Updated { get; }

        public MapUpdatedEventArgs(Map original, Map updated)
        {
            Original = original;
            Updated = updated;
        }
    }
}