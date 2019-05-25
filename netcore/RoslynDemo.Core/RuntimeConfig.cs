using System.Collections.Generic;

namespace RoslynDemo.Core
{
    public static class RuntimeConfig
    {
        public static Dictionary<string, object> Generate()
        {
            var version = "2.2.0";// Environment.Version;
            //var version = Environment.Version;

            return new Dictionary<string, object>
            {
                ["runtimeOptions"] = new Dictionary<string, object>
                {
                    ["tfm"] = $"netcoreapp{version}",
                    ["framework"] = new Dictionary<string, object>
                    {
                        ["name"] = "Microsoft.NETCore.App",
                        ["version"] = $"{version}"
                    }
                }
            };
        }
    }
}