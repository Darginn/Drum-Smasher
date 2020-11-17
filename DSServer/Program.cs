using System;
using System.Threading.Tasks;

namespace DSServer
{
    class Program
    {
        public const int SERVER_PORT = 40010;

        static Network.Server _server;

        static void Main(string[] args)
            => MainTask(args).ConfigureAwait(false).GetAwaiter().GetResult();

        static async Task MainTask(string[] args)
        {
            try
            {
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
