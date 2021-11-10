using System;

namespace Drum_Smasher_Mono
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using (var game = new GameClient())
                game.Run();
        }
    }
}
