using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace DSServer.Plugins
{
    public class PluginHandler
    {
        private ConcurrentDictionary<string, IPlugin> _plugins;

        public PluginHandler()
        {
            _plugins = new ConcurrentDictionary<string, IPlugin>();
        }

        public IPlugin this[string pluginName]
        {
            get
            {
                if (_plugins.TryGetValue(pluginName, out IPlugin plugin))
                    return plugin;

                return null;
            }
        }

        public int LoadPlugins(DirectoryInfo dir)
        {
            int loaded = 0;

            List<DirectoryInfo> directories = new List<DirectoryInfo>()
            {
                dir
            };

            directories.AddRange(dir.EnumerateDirectories());

            foreach(DirectoryInfo dirInfo in directories)
            {
                foreach(FileInfo file in dirInfo.EnumerateFiles())
                {
                    if (file.Extension.EndsWith("plugin", StringComparison.CurrentCultureIgnoreCase))
                        if (LoadPlugin(file))
                            loaded++;

                }
            }

            return loaded;
        }

        public bool LoadPlugin(FileInfo file)
        {
            if (!file.Exists)
                return false;

            byte[] assemblyBytes = new byte[file.Length];
            using (FileStream fstream = file.OpenRead())
                fstream.Read(assemblyBytes, 0, assemblyBytes.Length);

            Assembly ass = Assembly.Load(assemblyBytes);

            try
            {
                Type[] types = ass.GetTypes();

                for (int i = 0; i < types.Length; i++)
                {
                    if (!types[i].IsAssignableFrom(typeof(IPlugin)))
                        continue;

                    IPlugin plugin = Activator.CreateInstance(types[i]) as IPlugin;

                    if (plugin == null || _plugins.ContainsKey(plugin.Name) || !plugin.Load())
                        return false;

                    if (!_plugins.TryAdd(plugin.Name, plugin))
                    {
                        plugin.Unload();
                        return false;
                    }

                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }

            return false;
        }

        public void UnloadAllPlugins()
        {
            List<IPlugin> plugins = _plugins.Values.ToList();
            _plugins.Clear();

            foreach (IPlugin plugin in plugins)
                plugin.Unload();
        }
    }
}