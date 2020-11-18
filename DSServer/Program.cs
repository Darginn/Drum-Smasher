using System;
using System.Linq;
using System.Threading.Tasks;

namespace DSServer
{
    class Program
    {
        public static bool Debug { get; private set; }
        public static Random Random { get; } = new Random();

        static Network.Server _server;

        const string _CONFIG_FILE = "config.json";


        static void Main(string[] args)
            => MainTask(args).ConfigureAwait(false).GetAwaiter().GetResult();

        static async Task MainTask(string[] args)
        {
            try
            {
                Logger.Log("Starting DSServer", LogLevel.Info);

                if (args.Length > 0 &&
                    args.Any(arg => arg.Equals("-d", StringComparison.CurrentCultureIgnoreCase)))
                    Debug = true;

                Logger.Log("Loading Config", LogLevel.Info);

                if (!Config.Load(_CONFIG_FILE))
                {
                    Logger.Log("Config not found, creating default config file", LogLevel.Warning);
                    Config.CreateAndSaveDefault(_CONFIG_FILE);

                    Logger.Log("Default config file created, press any key to exit...", LogLevel.Info);
                    Console.ReadKey();

                    Environment.Exit(0);
                }

                //Logger.Log("Config Loaded, starting chatrooms...", LogLevel.Info);
                //ChatSystem.ChatRoom.InitializeChatRooms();

                //Logger.Log("ChatRooms started, starting server...", LogLevel.Info);
                //_server = new Network.Server(System.Net.IPAddress.Any.ToString(), Config.ServerPort <= 0 ? 40010 : Config.ServerPort);
                //_server.Start();

                //Logger.Log("Server started, starting discord...", LogLevel.Info);
                //Discord.DClient.InitializeDiscord();

                //Logger.Log("Discord started, server is up and running!", LogLevel.Info);
            }
            catch (Exception ex)
            {
                Logger.Log(ex, LogLevel.Error);
            }

            await Task.Delay(-1).ConfigureAwait(false);
        }
    }
}
