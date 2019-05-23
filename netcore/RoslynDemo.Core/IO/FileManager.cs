using System;
using System.IO;
using System.Linq;
using RoslynDemo.Core.Serializers;

namespace RoslynDemo.Core.IO
{
    public class FileManager
    {
        public static readonly string BaseFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Files");

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

        public string ToFullPath(string part, params string[] parts)
        {
            var allParts = new[] {BaseFolder, part}.Concat(parts).ToArray();
            return Path.Combine(allParts);
        }
    }
}