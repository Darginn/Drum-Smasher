using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Assets.Scripts.Configs
{
    public static class ConfigManager
    {
        static readonly Dictionary<string, IConfig> _configs;
        static readonly object _configLock;

        static ConfigManager()
        {
            _configs = new Dictionary<string, IConfig>();
            _configLock = new object();
        }

        /// <summary>
        /// Tries to get the config of a specified <see cref="BaseConfig{T}"/> type
        /// <para>If the config is not registered, tries to load and register the config</para>
        /// <para>If the config failed to load a default instance of that config will be created and registered</para>
        /// </summary>
        public static IConfig GetOrLoadOrAdd<T>() where T : BaseConfig<T>
        {
            lock(_configs)
            {
                if (_configs.TryGetValue(BaseConfig<T>.ConfigName, out IConfig cfg))
                    return cfg;

                cfg = LoadAndAdd<T>();

                if (cfg == null)
                    cfg = AddNew<T>();

                if (cfg == null)
                    throw new OperationCanceledException("Unable to get, load and add new config file");

                return cfg;
            }
        }

        /// <summary>
        /// Saves all currently loaded configs
        /// </summary>
        public static void SaveConfigs()
        {
            lock (_configLock)
            {
                foreach (IConfig cfg in _configs.Values)
                    cfg.Save();
            }
        }

        static IConfig AddNew<T>() where T : BaseConfig<T>
        {
            return _configs[BaseConfig<T>.ConfigName] = (Activator.CreateInstance(typeof(T)) as IConfig);
        }

        static IConfig LoadAndAdd<T>() where T : BaseConfig<T>
        {
            return _configs[BaseConfig<T>.ConfigName] = BaseConfig<T>.Load();
        }
    }
}
