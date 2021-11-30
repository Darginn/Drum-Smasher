using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Assets.Scripts.Configs;
using Assets.Scripts.Configs.GameConfigs;
using Assets.Scripts.IO.Charts;
using SQLite;
using UnityEngine;

namespace Assets.Scripts.Database.Maps
{
    public static class MapDatabaseCache
    {
        /// <summary>
        /// The path of the local database
        /// </summary>
        public static readonly string DatabasePath = GlobalConfig.Load().GameDirectory + "/DSGame.db";

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
            var chartFiles = Directory.GetFiles(GlobalConfig.Load().SongDirectory, "*.chart", SearchOption.AllDirectories).ToList();
            Logger.Log($"Found {chartFiles.Count} .chart files inside the song directory", LogLevel.Info);

            SyncMissingOrUpdatedFiles(chartFiles);
            AddNonCachedFiles(chartFiles);

            OrderAndSetMapsets();
        }

        /// <summary>
        /// Creates the `maps` database table.
        /// </summary>
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
                Logger.Log(e);
                throw;
            }
        }

        /// <summary>
        /// Checks the maps in the database vs. the amount of .chart files on disk.
        /// If there's a mismatch, it will add any missing ones
        /// </summary>
        private static void SyncMissingOrUpdatedFiles(IReadOnlyCollection<string> files)
        {
            var maps = FetchAll();

            foreach (var map in maps)
            {
                var filePath = BackslashToForward($"{GlobalConfig.Load().SongDirectory}/{map.Directory}/{map.Path}");

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
                            newMap = Map.FromChart(map.LoadChart(false), filePath);
                        }
                        catch (Exception e)
                        {
                            Logger.Log(e);
                            File.Delete(filePath);
                            new SQLiteConnection(DatabasePath).Delete(map);
                            Logger.Log($"Removed {filePath} from the cache, as the file could not be parsed.", LogLevel.Warning);
                            continue;
                        }

                        newMap.CalculateDifficulties();

                        newMap.Id = map.Id;
                        new SQLiteConnection(DatabasePath).Update(newMap);

                        Logger.Log($"Updated cached map: {newMap.Id}, as the file was updated.", LogLevel.Info);
                    }

                    continue;
                }

                // The file doesn't exist, so we can safely delete it from the cache.
                new SQLiteConnection(DatabasePath).Delete(map);
                Logger.Log($"Removed {filePath} from the cache, as the file no longer exists", LogLevel.Info);
            }
        }

        /// <summary>
        /// Adds any new files that are currently not cached.
        /// Used if the user adds a file to the folder.
        /// </summary>
        /// <param name="files"></param>
        private static void AddNonCachedFiles(List<string> files)
        {
            var maps = FetchAll();

            foreach (var file in files)
            {
                if (maps.Any(x => BackslashToForward(file) == BackslashToForward($"{GlobalConfig.Load().SongDirectory}/{x.Directory}/{x.Path}")))
                    continue;

                // Found map that isn't cached in the database yet.
                try
                {
                    var map = Map.FromChart(ChartFile.Parse(file, false), file);
                    map.CalculateDifficulties();

                    InsertMap(map, file);
                }
                catch (Exception e)
                {
                    Logger.Log(e);
                }
            }
        }

        /// <summary>
        /// Responsible for fetching all the maps from the database and returning them.
        /// </summary>
        /// <returns></returns>
        public static List<Map> FetchAll() => new SQLiteConnection(DatabasePath).Table<Map>().ToList();

        /// <summary>
        /// Converts all backslash characters to forward slashes.
        /// Used for paths
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        private static string BackslashToForward(string p) => p.Replace("\\", "/");

        /// <summary>
        /// Inserts an individual map to the database.
        /// </summary>
        /// <param name="map"></param>
        /// <param name="file"></param>
        public static int InsertMap(Map map, string file)
        {
            try
            {
                new SQLiteConnection(DatabasePath).Insert(map);

                return new SQLiteConnection(DatabasePath).Get<Map>(x => x.Md5Checksum == map.Md5Checksum).Id;
            }
            catch (Exception e)
            {
                Logger.Log(e);
                File.Delete(file);
                return -1;
            }
        }

        /// <summary>
        /// Updates an individual map in the database.
        /// </summary>
        /// <param name="map"></param>
        public static void UpdateMap(Map map)
        {
            try
            {
                new SQLiteConnection(DatabasePath).Update(map);
                Logger.Log($"Updated map: {map.Md5Checksum} (#{map.Id}) in the cache", LogLevel.Info);
            }
            catch (Exception e)
            {
                Logger.Log(e);
            }
        }

        /// <summary>
        /// Removes an individual map in the database.
        /// </summary>
        /// <param name="map"></param>
        public static void RemoveMap(Map map)
        {
            try
            {
                new SQLiteConnection(DatabasePath).Delete(map);
                Logger.Log($"Deleted map: {map.Md5Checksum} (#{map.Id}) in the cache", LogLevel.Info);
            }
            catch (Exception e)
            {
                Logger.Log(e);
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Map FindSet(int id)
        {
            try
            {

                return new SQLiteConnection(DatabasePath).Find<Map>(x => x.MapSetId == id);
            }
            catch (Exception e)
            {
                Logger.Log(e);
                return null;
            }
        }

        /// <summary>
        ///     Fetches all maps, groups them into mapsets, sets them to allow them to be played.
        /// </summary>
        public static void OrderAndSetMapsets()
        {
            var maps = FetchAll();

            var mapsets = MapsetHelper.ConvertMapsToMapsets(maps);
            MapManager.Mapsets = MapsetHelper.OrderMapsByDifficulty(MapsetHelper.OrderMapsetsByArtist(mapsets));
        }

        /// <summary>
        /// </summary>
        public static void ForceUpdateMaps()
        {
            for (var i = 0; i < MapsToUpdate.Count; i++)
            {
                try
                {
                    var path = $"{GlobalConfig.Load().SongDirectory}/{MapsToUpdate[i].Directory}/{MapsToUpdate[i].Path}";

                    if (!File.Exists(path))
                        continue;

                    var map = Map.FromChart(ChartFile.Parse(path, false), path);
                    map.CalculateDifficulties();
                    map.Id = MapsToUpdate[i].Id;

                    if (map.Id == 0)
                        map.Id = InsertMap(map, path);
                    else
                        UpdateMap(map);

                    MapsToUpdate[i] = map;
                    MapManager.Selected.Value = map;
                }
                catch (Exception e)
                {
                    Logger.Log(e);
                }
            }

            MapsToUpdate.Clear();
            OrderAndSetMapsets();

            var selectedMapset = MapManager.Mapsets.Find(x => x.Maps.Any(y => y.Id == MapManager.Selected.Value.Id));
            MapManager.Selected.Value = selectedMapset.Maps.Find(x => x.Id == MapManager.Selected.Value.Id);
        }
    }
}