using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.IO;

namespace Assets.Scripts.Configs
{
    public abstract class BaseConfig<T> : JsonFile<T>, IConfig where T : class
    {
        public static string ConfigName => nameof(T);
        public static string ConfigFileName => $"{ConfigName}.cfg";
        public static string ConfigFilePath => Path.Combine(Environment.CurrentDirectory, "Configs", ConfigFileName);

        public static T Load()
        {
            return Load(ConfigFilePath);
        }

        public void Save()
        {
            Save(ConfigFilePath);
        }
    }
}
