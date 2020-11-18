using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DSServer
{
    public class Config
    {
        [JsonProperty]
        public static string DBConnectionString { get; set; }
        [JsonProperty]
        public static bool UseMariaDB { get; set; }
        [JsonProperty]
        public static string DiscordSecret { get; set; }
        [JsonProperty]
        public static int ServerPort { get; set; }

        public static bool Load(string file)
        {
            if (!File.Exists(file))
                return false;

            string cfg = File.ReadAllText(file);

            if (string.IsNullOrEmpty(cfg))
                return false;

            _ = JsonConvert.DeserializeObject(cfg);

            return true;
        }

        public static void Save(string file)
        {
            if (File.Exists(file))
                File.Move(file, file + ".old");

            string cfg = JsonConvert.SerializeObject(new Config(), Formatting.Indented);
            File.WriteAllText(file, cfg);

            return;
        }

        public static void CreateAndSaveDefault(string file)
        {
            DBConnectionString = "A";
            DiscordSecret = "A";
            ServerPort = -1;

            Save(file);
        }
    }
}
