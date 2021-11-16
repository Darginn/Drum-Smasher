using Assets.Settings.Bindable;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace Assets.Scripts.Database.Maps
{
    public class MapManager
    {
        /// <summary>
        /// The currently selected map.
        /// </summary>
        public static Bindable<Map> Selected { get; set; } = new Bindable<Map>(null);

        /// <summary>
        /// The list of mapsets that are currently loaded.
        /// </summary>
        public static List<Mapset> Mapsets { get; set; } = new List<Mapset>();

        /// <summary>
        /// List of recently selected/played maps
        /// </summary>
        public static List<Map> RecentlyPlayed { get; set; } = new List<Map>();

        /// <summary>
        /// The osu! Songs folder path
        /// </summary>
        public static string OsuSongsFolder { get; set; }

        /// <summary>
        /// The current path of the selected map's audio file
        /// </summary>
        public static string CurrentAudioPath => GetAudioPath(Selected?.Value);

        /// <summary>
        /// The current background of the map.
        /// </summary>
        public static Texture2D CurrentBackground { get; set; }

        /// <summary>
        /// The current path of the selected map's background path.
        /// </summary>
        public static string CurrentBackgroundPath => GetBackgroundPath(Selected.Value);

        /// <summary>
        /// Event invoked when a mapset has been deleted
        /// </summary>
        public static event EventHandler<MapsetDeletedEventArgs> MapsetDeleted;

        /// <summary>
        /// Event invoked when a map has been deleted
        /// </summary>
        public static event EventHandler<MapDeletedEventArgs> MapDeleted;

        /// <summary>
        /// Event invoked when a map has been updated
        /// </summary>
        public static event EventHandler<MapUpdatedEventArgs> MapUpdated;

        /// <summary>
        /// Gets the background path for a given map.
        /// </summary>
        /// <param name="map"></param>
        /// <returns></returns>
        public static string GetBackgroundPath(Map map)
        {
            if (map == null)
                return "";

            switch (map.Game)
            {
                case MapGame.Osu:
                    throw new NotImplementedException();              // osu map bg directory

                case MapGame.DrumSmasher:
                    throw new NotImplementedException();              // ds map bg directory

                default:
                    return "";
            }
        }

        /// <summary>
        /// Gets a map's audio path taking into account the game.
        /// </summary>
        /// <param name="map"></param>
        /// <returns></returns>
        public static string GetAudioPath(Map map)
        {
            if (map == null)
                return "";

            switch (map.Game)
            {
                case MapGame.Osu:
                    throw new NotImplementedException();              // osu map audio directory

                case MapGame.DrumSmasher:
                    throw new NotImplementedException();              // ds map audio directory

                default:
                    return "";
            }
        }

        /// <summary>
        /// Finds a map based on the md5 hash
        /// </summary>
        /// <param name="md5"></param>
        /// <returns></returns>
        public static Map FindMapFromMd5(string md5)
        {
            foreach (var set in Mapsets)
            {
                var found = set.Maps.Find(x => x.Md5Checksum == md5);

                if (found != null)
                    return found;
            }

            return null;
        }

        /// <summary>
        /// Finds a map based on its online id
        /// </summary>
        /// <returns></returns>
        public static Map FindMapFromOnlineId(int id)
        {
            foreach (var set in Mapsets)
            {
                var found = set.Maps.Find(x => x.MapId == id);

                if (found != null)
                    return found;
            }

            return null;
        }

        ///<summary>
        /// Gets a map's custom audio sample path taking into account the game.
        /// Hitsounds etc.
        /// </summary>
        /// <param name="map"></param>
        /// <param name="samplePath"></param>
        /// <returns></returns>
        public static string GetCustomAudioSamplePath(Map map, string samplePath)
        {
            switch (map.Game)
            {
                case MapGame.Osu:
                    throw new NotImplementedException();              // osu map custom audio path

                case MapGame.DrumSmasher:
                    throw new NotImplementedException();              // ds map custom audio path

                default:
                    return "";
            }
        }

        /// <summary>
        /// Deletes a map from the game
        /// </summary>
        /// <param name="map"></param>
        /// <param name="index"></param>
        public static void Delete(Map map, int index)
        {
            if (map.Game != MapGame.DrumSmasher)
            {
                throw new NotImplementedException("You cannot delete a map loaded from another game yet");
            }

            try
            {
                throw new NotImplementedException();
            }
            catch (Exception e)
            {
                Logger.Log(e, LogType.Exception);
            }

            map.Mapset.Maps.Remove(map);

            // Delete the mapset entirely if there are no more maps left.
            if (map.Mapset.Maps.Count == 0)
                Mapsets.Remove(map.Mapset);

            //PlaylistManager.RemoveMapFromAllPlaylists(map);

            // Raise an event with the deleted map
            MapDeleted?.Invoke(typeof(MapManager), new MapDeletedEventArgs(map, index));
        }

        /// <summary>
        /// Deletes the mapset from the game
        /// </summary>
        public static void Delete(Mapset mapset, int index)
        {
            if (mapset.Maps.Count == 0)
                return;

            if (mapset.Maps.First().Game != MapGame.DrumSmasher)
            {
                throw new NotImplementedException("You cannot delete a map loaded from another game yet");
            }

            // Dispose of the playing track, so it can be deleted
            // assuming the song is playing when the delete method is called
            if (mapset.Maps.Contains(Selected.Value))
            {
            }

            try
            {
                Directory.Delete(mapset.Directory, true);
            }
            catch (Exception e)
            {
                Logger.Log(e, LogType.Exception);
            }

            try
            {
                // mapset.Maps.ForEach(MapDatabaseCache.RemoveMap);             --- MapDatabaseCache class fucking hates me ~Dargin
            }
            catch (Exception e)
            {
                Logger.Log(e, LogType.Exception);
            }

            Mapsets.Remove(mapset);

            // Raise an event letting subscribers know a mapset has been deleted
            MapsetDeleted?.Invoke(typeof(MapManager), new MapsetDeletedEventArgs(mapset, index));

            // Dispose and delete the mapset's background if it exists

            // background stuff
        }

    }
}