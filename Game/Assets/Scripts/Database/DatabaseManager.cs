using SQLite;
using UnityEngine;

namespace Assets.Scripts.Database
{
    public class DatabaseManager : MonoBehaviour
    {
        /// <summary>
        /// The path of the local database
        /// </summary>
        public static readonly string DatabasePath = Application.dataPath + "/DSGame.db";

        /// <summary>
        /// </summary>
        public static SQLiteConnection Connection { get; private set; }

        /// <summary>
        /// </summary>
        public void Start() => Connection = new SQLiteConnection(DatabasePath);
    }
}
