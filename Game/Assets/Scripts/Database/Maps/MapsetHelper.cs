﻿using Assets.Scripts.Configs;
using Assets.Scripts.Configs.GameConfigs;
using Assets.Scripts.Enums;
using Assets.Scripts.TaikoGame.Modifiers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace Assets.Scripts.Database.Maps
{
    public static class MapsetHelper
    {
        /// <summary>
        /// Gets the Md5 Checksum of a file, more specifically a .chart file.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        internal static string GetMd5Checksum(string path)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(path))
                {
                    return BitConverter.ToString(md5.ComputeHash(stream)).Replace("-", string.Empty).ToLower();
                }
            }
        }

        /// <summary>
        /// Responsible for taking a list of maps, and grouping each directory.
        /// </summary>
        /// <param name="maps"></param>
        /// <returns></returns>
        internal static List<Mapset> ConvertMapsToMapsets(IEnumerable<Map> maps)
        {
            // Group maps by directory.
            var groupedMaps = maps
                .GroupBy(u => u.Directory)
                .Select(grp => grp.ToList())
                .ToList();

            // Populate the mapsets with the grouped maps.
            var mapsets = new List<Mapset>();

            foreach (var mapset in groupedMaps)
            {
                var set = new Mapset()
                {
                    Directory = mapset.First().Directory,
                    Maps = mapset
                };

                set.Maps.ForEach(x => x.Mapset = set);
                mapsets.Add(set);
            }

            return mapsets;
        }

        /// <summary>
        /// Orders the mapsets by artist, and then by title.
        /// </summary>
        /// <param name="mapsets"></param>
        /// <returns></returns>
        internal static List<Mapset> OrderMapsetsByArtist(IEnumerable<Mapset> mapsets)
        {
            return mapsets.OrderBy(x => x.Maps.First().Artist).ThenBy(x => x.Maps.First().Title).ToList();
        }

        /// <summary>
        /// Orders mapsets by title.
        /// </summary>
        /// <param name="mapsets"></param>
        /// <returns></returns>
        internal static List<Mapset> OrderMapsetsByTitle(IEnumerable<Mapset> mapsets) => mapsets.OrderBy(x => x.Maps.First().Title).ToList();

        /// <summary>
        /// Orders mapsets by creator.
        /// </summary>
        /// <param name="mapsets"></param>
        /// <returns></returns>
        internal static List<Mapset> OrderMapsetsByCreator(IEnumerable<Mapset> mapsets)
        {
            return mapsets.OrderBy(x => x.Maps.First().Creator).ThenBy(x => x.Maps.First().Artist).ThenBy(x => x.Maps.First().Title).ToList();
        }

        /// <summary>
        /// Orders the mapsets based on the set config value.
        /// </summary>
        /// <param name="mapsets"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>


        internal static List<Mapset> OrderMapsetsByConfigValue(IEnumerable<Mapset> mapsets)
        {
            GlobalConfig gc = (GlobalConfig)ConfigManager.GetOrLoadOrAdd<GlobalConfig>();
            switch (gc.SelectOrderMapsetsBy)
            {
                case OrderMapsetsBy.Artist:
                    return OrderMapsetsByArtist(mapsets);
                case OrderMapsetsBy.Title:
                    return OrderMapsetsByTitle(mapsets);
                case OrderMapsetsBy.Creator:
                    return OrderMapsetsByCreator(mapsets);
                case OrderMapsetsBy.DateAdded:
                    return OrderMapsetsByDateAdded(mapsets);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        
        }
        
        /// <summary>
        /// Orders the map's mapsets by date added
        /// </summary>
        /// <param name="mapsets"></param>
        /// <returns></returns>
        internal static List<Mapset> OrderMapsetsByDateAdded(IEnumerable<Mapset> mapsets)
            => mapsets.OrderByDescending(x => x.Maps.First().DateAdded).ThenBy(x => x.Maps.First().Artist).ThenBy(x => x.Maps.First().Title).ToList();

        /// <summary>
        /// Orders the map's mapsets by difficulty.
        /// </summary>
        /// <param name="mapsets"></param>
        /// <returns></returns>
        [Obsolete("Need Difficulty Calculation implementation first")]
        internal static List<Mapset> OrderMapsByDifficulty(List<Mapset> mapsets)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Searches and returns mapsets given a query
        /// </summary>
        /// <param name="mapsets"></param>
        /// <param name="query></param>
        /// <returns></returns>
         
        [Obsolete("Needs fixing before use")]
        internal static List<Mapset> SearchMapsets(IEnumerable<Mapset> mapsets, string query)
        {
            var sets = new List<Mapset>();
            query = query.ToLower();
            // All the possible relational operators for the search query
            var operators = new List<string>
            {
                ">=",
                "<=",
                "==",
                "!=",
                "<",
                ">",
                "=",
            };

            // The shortest and longest matching sequences for every option. For example, "d" and "difficulty" means you
            // can search for "d>5", "di>5", "dif>5" and so on up to "difficulty>5".
            //
            // When adding new options with overlapping first characters, specify the shortest unambiguous sequence as
            // the new key. So, for example, "duration" should be added as "du", "duration". This way "d" still searches
            // for "difficulty" (backwards-compatibility) and "du" searches for duration.
            var options = new Dictionary<SearchFilterOption, (string Shortest, string Longest)>()
            {
                { SearchFilterOption.BPM,        ("b", "bpm") },
                { SearchFilterOption.Difficulty, ("d", "difficulty") },
                { SearchFilterOption.Length,     ("l", "length") },
                { SearchFilterOption.Status,     ("s", "status") }
            };

            // Stores a dictionary of the found pairs in the search query
            // <option, value, operator>
            var foundSearchFilters = new List<SearchFilter>();

            var terms = query.Split(null).ToList();

            // Get a list of all the matching search filters.
            // All matched filters are removed from the list of terms.


            for (var i = terms.Count - 1; i >= 0; i--)
            {
                var term = terms[i];

                foreach (var op in operators)
                {
                    var match = Regex.Match(term, $@"(.+)\b{op}\b(.+)");
                    if (!match.Success)
                        continue;

                    var searchOption = match.Groups[1].Value;
                    var val = match.Groups[2].Value;

                    //TODO: Fix SearchMapsets method

                    //foreach (var (option, (shortest, longest)) in options)
                    //{
                    //    if (longest.StartsWith(searchOption) && searchOption.StartsWith(shortest))
                    //    {
                    //        foundSearchFilters.Add(new SearchFilter
                    //        {
                    //            Option = option,
                    //            Value = val,
                    //            Operator = op,
                    //        });

                    //        // Remove it from the search terms.
                    //        terms.RemoveAt(i);
                    //        break;
                    //    }
                    //}
                }
            }

            // Create a list of mapsets with the matched mapsets
            foreach (var mapset in mapsets)
            {
                foreach (var map in mapset.Maps)
                {
                    var exitLoop = false;

                    foreach (var searchQuery in foundSearchFilters)
                    {
                        switch (searchQuery.Option)
                        {
                            case SearchFilterOption.BPM:
                                if (!float.TryParse(searchQuery.Value, out var valBpm))
                                    exitLoop = true;

                                if (!CompareValues(map.Bpm, valBpm, searchQuery.Operator))
                                    exitLoop = true;
                                break;
                            case SearchFilterOption.Difficulty:
                                if (!float.TryParse(searchQuery.Value, out var valDiff))
                                    exitLoop = true;


                              if (!CompareValues(map.DifficultyFromMods(ModManager.Mods), valDiff, searchQuery.Operator))
                                  exitLoop = true;

                                break;
                            case SearchFilterOption.Length:
                                if (!float.TryParse(searchQuery.Value, out var valLength))
                                    exitLoop = true;

                                if (!CompareValues(map.SongLength, valLength, searchQuery.Operator))
                                    exitLoop = true;

                                break;
                            default:
                                break;
                        }

                        if (exitLoop)
                            break;
                    }

                    if (exitLoop)
                        continue;

                    // Check if the terms exist in any of the following properties.
                    foreach (var term in terms)
                    {
                        try
                        {
                            if (!map.Artist.ToLower().Contains(term) && !map.Title.ToLower().Contains(term) &&
                                !map.Creator.ToLower().Contains(term) && !map.Source.ToLower().Contains(term) &&
                                !map.Description.ToLower().Contains(term) && !map.Tags.ToLower().Contains(term) &&
                                !map.DifficultyName.ToLower().Contains(term))
                            {
                                exitLoop = true;
                                break;
                            }
                        }
                        catch (Exception)
                        {
                            // Some values can be null and they break the game.
                            exitLoop = true;
                            break;
                        }
                    }

                    if (exitLoop)
                        continue;

                    // Add the set if all the comparisons and queries are correct
                    if (sets.All(x => x.Directory != map.Directory))
                        sets.Add(new Mapset()
                        {
                            Directory = map.Directory,
                            Maps = new List<Map> { map }
                        });
                    else
                        sets.Find(x => x.Directory == map.Directory).Maps.Add(map);
                }
            }

            return sets;
        }

        /// <summary>
        ///     Compares two values and determines
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="val1"></param>
        /// <param name="val2"></param>
        /// <param name="operation"></param>
        /// <returns></returns>
        private static bool CompareValues<T>(T val1, T val2, string operation) where T : IComparable<T>
        {
            if (val1 == null || val2 == null)
                return false;

            var compared = val1.CompareTo(val2);

            switch (operation)
            {
                case "<":
                    return compared < 0;
                case ">":
                    return compared > 0;
                case "=":
                case "==":
                    return compared == 0;
                case "<=":
                    return compared <= 0;
                case ">=":
                    return compared >= 0;
                case "!=":
                    return compared != 0;
                default:
                    return false;
            }
        }
    }

    /// <summary>
    ///     A struct for the map searching filter.
    /// </summary>
    public struct SearchFilter
    {
        /// <summary>
        ///     The search option - bpm, length, etc,
        /// </summary>
        public SearchFilterOption Option;

        /// <summary>
        ///     The value the user is searching
        /// </summary>
        public string Value;

        /// <summary>
        ///     The operator the user gave
        /// </summary>
        public string Operator;
    }

    /// <summary>
    ///     Options you can filter by.
    /// </summary>
    public enum SearchFilterOption
    {
        /// <summary>
        /// BPM.
        /// </summary>
        BPM,

        /// <summary>
        /// Difficulty rating.
        /// </summary>
        Difficulty,

        /// <summary>
        /// Length in seconds.
        /// </summary>
        Length,

        /// <summary>
        ///     Status (ranked, not submitted, etc.)
        /// </summary>
        Status,
    }
}