using System;
using System.Linq;
using System.Threading.Tasks;

namespace DSServer
{
    class Program
    {
        public const int SERVER_PORT = 40010;
        public static bool Debug { get; private set; }

        static Network.Server _server;

        static void Main(string[] args)
            => MainTask(args).ConfigureAwait(false).GetAwaiter().GetResult();

        static async Task MainTask(string[] args)
        {
            try
            {
                if (args.Length > 0 &&
                    args.Any(arg => arg.Equals("-d", StringComparison.CurrentCultureIgnoreCase)))
                    Debug = true;

                ChatSystem.ChatRoom.InitializeChatRooms();

                _server = new Network.Server(System.Net.IPAddress.Any.ToString(), SERVER_PORT);
                _server.Start();

                Discord.DClient.InitializeDiscord();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            await Task.Delay(-1).ConfigureAwait(false);
        }
    }
}
