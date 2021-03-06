﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace RoslynDemo.Core
{
    public static class Assemblies
    {
        public static IEnumerable<Assembly> FromAppDomain() => AppDomain.CurrentDomain.GetAssemblies().Where(a => !a.IsDynamic);

        public static IEnumerable<Assembly> FromDisk() => Directory
            .EnumerateFiles(AppDomain.CurrentDomain.BaseDirectory, "*.dll")
            .Select(Assembly.LoadFile);

        public static IEnumerable<Assembly> FromCurrentContext() => FromAppDomain().Concat(FromDisk());

        public static readonly string[] RoslynNamespaces = {
            "Microsoft.CodeAnalysis",
            "Microsoft.CodeAnalysis.CSharp",
            "Microsoft.CodeAnalysis.Scripting",
            "Microsoft.CodeAnalysis.CSharp.Scripting"
        };
    }
}