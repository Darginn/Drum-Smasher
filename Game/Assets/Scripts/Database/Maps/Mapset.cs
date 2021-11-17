using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Database.Maps
{
    public class Mapset : MonoBehaviour
    {
        /// <summary>
        /// The directory of the mapset.
        /// </summary>
        public string Directory { get; set; }

        /// <summary>
        /// The list of maps in this mapset.
        /// </summary>
        public List<Map> Maps { get; set; }

        /// <summary>
        /// The last selected/preferred map in this set
        /// </summary>
        public Map PreferredMap { get; set; }

        public string Artist => Maps.First().Artist;
        public string Title => Maps.First().Title;
        public string Creator => Maps.First().Creator;
        public string Background => MapManager.GetBackgroundPath(Maps.First());

        /// <summary>
        /// Exports the entire mapset to a zip (maybe .taiko or .ds??) file.
        /// </summary>
        [Obsolete("To be implemented")]
        public string ExportToZip(bool openInExplorer = true)
        {
            return null;
        }

    }
}
