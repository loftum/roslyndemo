using System;
using System.ComponentModel;
using System.Windows;
using RoslynDemo.Core.IO;

namespace Studio.Wpf
{
    public abstract class MainWindowBase : Window
    {
        protected readonly FileManager FileManager = new FileManager();

        protected override void OnClosing(CancelEventArgs e)
        {
            var settings = new WindowSettings
            {
                Top = Top,
                Left = Left,
                Width = Width,
                Height = Height
            };
            FileManager.SaveJson(settings);

            base.OnClosing(e);
        }

        protected override void OnInitialized(EventArgs e)
        {
            var settings = FileManager.LoadJson<WindowSettings>() ?? new WindowSettings();
            Top = settings.Top;
            Left = settings.Left;
            Width = settings.Width;
            Height = settings.Height;

            base.OnInitialized(e);
        }
    }
}