using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.IO
{
    public abstract class JsonFile<T> where T : class
    {
        public void Save(string file)
        {
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(this, Newtonsoft.Json.Formatting.Indented);

            if (File.Exists(file))
                File.Delete(file);

            File.WriteAllText(file, json);
        }

        public static T Load(string file)
        {
            if (!File.Exists(file))
                return null;

            string json = File.ReadAllText(file);

            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(json);
        }
    }
}
