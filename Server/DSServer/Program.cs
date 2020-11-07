using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using DSServer.Chat;
using DSServer.Database;
using DSServer.Server;
using DSServerCommon;
using DSServerCommon.ChatSystem;

namespace DSServer
{
    class Program
    {
        public static ChatChannel MainChat;
        public static DSServer Server;
        public static ILogger Logger;
        public static Discord.DiscordChatClient DiscordClient;

        static void Main(string[] args)
            => MainTask(args).ConfigureAwait(false).GetAwaiter().GetResult();

        static async Task MainTask(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler((o, a) =>
            {
                Console.WriteLine(a.ExceptionObject);
                File.WriteAllText("error.txt", a.ExceptionObject.ToString());
                Task.Delay(-1).Wait();
            });

            try
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Logger = new Logger(true);


                Logger.Log("Starting DS Server on port 40020");
                Server = new DSServer(IPAddress.Any, 40020, Logger);
                Logger.Log("Loading SQL Connection");

                if (!SetSQLString())
                {
                    Logger.Log("Failed to load SQL Connection, check your 'dbconnection.string' file!", LogLevel.Error);

                    await Task.Delay(-1);
                    return;
                }

                Logger.Log("Loaded SQL Connection");

                if (!SetDiscordToken())
                {
                    Logger.Log("Failed to set discord token!", LogLevel.Error);

                    await Task.Delay(-1);
                    return;
                }

                Logger.Log("Loaded Discord Secret");

                Logger.Log("Initializing chat channels");
                InitializeChats();
                Logger.Log("Chat channels initialized");

                Logger.Log("Starting discord");
                DiscordClient = new Discord.DiscordChatClient();
                await DiscordClient.ConnectAsync();
                Logger.Log("Discord started");

                Logger.Log("Starting server");
                Server.Start();
                Logger.Log("Server started");
            }
            catch (Exception ex)
            {
                Logger.Log(ex.ToString(), LogLevel.Error);
            }

            await Task.Delay(-1);
        }

        private static void InitializeChats()
        {
            using DSContext c = new DSContext();
            long oldestChat = c.ChatChannel.Min(cc => cc.Id);

            var chatChannel = c.ChatChannel.First(cc => cc.Id == oldestChat);

            MainChat = new MainChat(chatChannel.Id, chatChannel.Name, Logger);
        }

        private static bool SetDiscordToken()
        {
            if (!File.Exists("discord.secret"))
                return false;

            string secret = File.ReadAllText("discord.secret");

            if (string.IsNullOrEmpty(secret))
                return false;

            Environment.SetEnvironmentVariable("DiscordSecret", secret, EnvironmentVariableTarget.Process);

            return true;
        }

        private static bool SetSQLString()
        {
            if (!File.Exists("dbconnection.string"))
                return false;

            string dbConStr = File.ReadAllText("dbconnection.string");

            if (string.IsNullOrEmpty(dbConStr))
                return false;

            Environment.SetEnvironmentVariable("DBConnectionString", dbConStr, EnvironmentVariableTarget.Process);

            return true;
        }
    }
}
