using System;
using System.IO;
using Convenient.Stuff.Serializers;

namespace Convenient.Stuff.IO
{
    public class FileManager
    {
        private static readonly string BaseFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Files");

        static FileManager()
        {
            if (!Directory.Exists(BaseFolder))
            {
                Directory.CreateDirectory(BaseFolder);
            }
        }

        public void SaveJson<T>(T item)
        {
            var path = PathFor<T>();
            File.WriteAllText(path, item.ToJson());
        }

        public T LoadJson<T>()
        {
            var path = PathFor<T>();
            if (!File.Exists(path))
            {
                return default(T);
            }
            return Json.Deserialize<T>(File.ReadAllText(path));
        }

        private static string PathFor<T>()
        {
            return Path.Combine(BaseFolder, $"{typeof(T).Name}.json");
        }
    }
}