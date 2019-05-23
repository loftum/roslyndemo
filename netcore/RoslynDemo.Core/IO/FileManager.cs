using System;
using System.IO;
using System.Linq;
using RoslynDemo.Core.Serializers;

namespace RoslynDemo.Core.IO
{
    public class FileManager
    {
        public string BaseFolder { get; }

        public FileManager() : this("Files")
        {
        }

        public FileManager(string folder)
        {
            BaseFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, folder);
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

        public void SaveJson<T>(T item, string filename)
        {
            var path = ToFullPath(filename);
            File.WriteAllText(path, item.ToJson());
        }

        public T LoadJson<T>()
        {
            var path = PathFor<T>();
            return !File.Exists(path) ? default : Json.Deserialize<T>(File.ReadAllText(path));
        }

        private string PathFor<T>()
        {
            return Path.Combine(BaseFolder, $"{typeof(T).Name}.json");
        }

        public string ToFullPath(string part, params string[] parts)
        {
            var allParts = new[] {BaseFolder, part}.Concat(parts).ToArray();
            return Path.Combine(allParts);
        }

        public void CleanFolder()
        {
            foreach (var file in Directory.EnumerateFiles(BaseFolder))
            {
                File.Delete(file);
            }

            foreach (var sub in Directory.EnumerateDirectories(BaseFolder))
            {
                Directory.Delete(sub, true);
            }
        }
    }
}