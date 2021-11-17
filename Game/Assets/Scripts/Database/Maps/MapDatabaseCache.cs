using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Assets.Scripts.Configs;
using SQLite;

namespace Assets.Scripts.Database.Maps
{
    public static class MapDatabaseCache
    {
        /// <summary>
        /// The path of the local database
        /// </summary>
        public static readonly string DatabasePath = Application.dataPath + "/DSGame.db";

        /// <summary>
        /// List of maps to force update after editing them.
        /// </summary>
        public static List<Map> MapsToUpdate { get; } = new List<Map>();

        /// <summary>
        /// Loads all of the maps in the database and groups them into mapsets to use
        /// for gameplay
        /// </summary>
        public static void Load(bool fullSync)
        {
            if (fullSync)
            {
                if (File.Exists(DatabasePath))
                    File.Delete(DatabasePath);
            }

            CreateTable();

            // Fetch all of the .chart files inside of the song directory
            var chartFiles = Directory.GetFiles(Application.dataPath + "/../Charts", "*.chart", SearchOption.AllDirectories).ToList();
            Logger.Log($"Found {chartFiles.Count} .chart files inside the song directory", LogLevel.Info);

            SyncMissingOrUpdatedFiles(chartFiles);
            AddNonCachedFiles(chartFiles);

            OrderAndSetMapsets();
        }

        private static void CreateTable()
        {
            try
            {
                var conn = new SQLiteConnection(DatabasePath);
                conn.CreateTable<Map>();
                Logger.Log($"Map Database has been created", LogLevel.Info);
            }
            catch (Exception e)
            {
                Logger.Log(e, LogLevel.Exception);
                throw;
            }
        }

        /// <summary>
        ///     Checks the maps in the database vs. the amount of .chart files on disk.
        ///     If there's a mismatch, it will add any missing ones
        /// </summary>
        private static void SyncMissingOrUpdatedFiles(IReadOnlyCollection<string> files)
        {
            var maps = FetchAll();

            foreach (var map in maps)
            {
                var filePath = BackslashToForward($"{Application.dataPath + "/../Charts"}/{map.Directory}/{map.Path}");

                // Check if the file actually exists.
                if (files.Any(x => BackslashToForward(x) == filePath))
                {
                    // Check if the file was updated. In this case, we check if the last write times are different
                    // BEFORE checking Md5 checksum of the file since it's faster to check if we even need to
                    // bother updating it.
                    if (map.LastFileWrite != File.GetLastWriteTimeUtc(filePath))
                    {
                        if (map.Md5Checksum == MapsetHelper.GetMd5Checksum(filePath))
                            continue;

                        Map newMap;

                        try
                        {
                            newMap = Map.FromQua(map.LoadQua(false), filePath);
                        }
                        catch (Exception e)
                        {
                            Logger.Error(e, LogType.Runtime);
                            File.Delete(filePath);
                            new SQLiteConnection(DatabasePath).Delete(map);
                            Logger.Important($"Removed {filePath} from the cache, as the file could not be parsed.", LogType.Runtime);
                            continue;
                        }

                        newMap.CalculateDifficulties();

                        newMap.Id = map.Id;
                        new SQLiteConnection(DatabasePath).Update(newMap);

                        Logger.Important($"Updated cached map: {newMap.Id}, as the file was updated.", LogType.Runtime);
                    }

                    continue;
                }

                // The file doesn't exist, so we can safely delete it from the cache.
                new SQLiteConnection(DatabasePath).Delete(map);
                Logger.Important($"Removed {filePath} from the cache, as the file no longer exists", LogType.Runtime);
            }
        }
    }
}