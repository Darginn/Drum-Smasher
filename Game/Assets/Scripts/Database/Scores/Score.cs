using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;
using Assets.Scripts.Enums;

namespace Assets.Scripts.Database.Scores
{
    /// <summary>
    /// The following is all the schema of data that will be stored in the local scores database
    /// When retrieving data from the scores db, this is all the data that will be able to be
    /// accessed
    /// </summary>
    public class Score
    {
        /// <summary>
        /// The unique id of the score
        /// </summary>
        [PrimaryKey]
        [AutoIncrement]
        public int Id { get; set; }

        /// <summary>
        /// The id of the user local profile that the score has (also to be stored in database)
        /// </summary>
        public int LocalProfileId { get; set; }

        /// <summary>
        /// The MD5 Hash of the map
        /// </summary>
        public string MapMd5 { get; set; }

        /// <summary>
        /// The name of the player who set the score
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The date and time the score was achieved
        /// </summary>
        public string DateTime { get; set; }

        /// <summary>
        /// The score the player achieved
        /// </summary>
        public int TotalScore { get; set; }

        /// <summary>
        /// The grade achieved for this score
        /// </summary>
        public Grade Grade { get; set; }

        /// <summary>
        /// The accuracy the player achieved
        /// </summary>
        public double Accuracy { get; set; }

        /// <summary>
        /// The max combo the player achieved
        /// </summary>
        public int MaxCombo { get; set; }

        /// <summary>
        /// The amount of greats the user got.
        /// </summary>
        public int CountGreat { get; set; }

        /// <summary>
        /// The amount of goods the user got.
        /// </summary>
        public int CountGood { get; set; }

        /// <summary>
        ///     The amount of misses the user got.
        /// </summary>
        public int CountMiss { get; set; }
    }
}
