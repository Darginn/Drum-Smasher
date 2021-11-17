using Assets.Scripts.Configs.Bindable;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Assets.Scripts.Enums;

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


    }
}