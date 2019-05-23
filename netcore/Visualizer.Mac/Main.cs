using AppKit;
using RoslynDemo.Core.Models.Syntax;

namespace Visualizer.Mac
{
    static class MainClass
    {
        static void Main(string[] args)
        {
            NSApplication.Init();
            NSApplication.Main(args);
        }
    }
}
