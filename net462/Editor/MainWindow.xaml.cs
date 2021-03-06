﻿using System;
using System.ComponentModel;
using System.Windows.Input;
using Convenient.Stuff.IO;
using Editor.ViewModels;

namespace Editor
{
    public partial class MainWindow
    {
        private readonly FileManager _fileManager = new FileManager();
        protected MainViewModel Vm => (MainViewModel) DataContext;

        public MainWindow()
        {
            InitializeComponent();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            _fileManager.SaveJson(new Data
            {
                Input = Input.Text
            });

            base.OnClosing(e);
        }

        protected override void OnInitialized(EventArgs e)
        {
            var settings = _fileManager.LoadJson<WindowSettings>() ?? new WindowSettings();
            Top = settings.Top;
            Left = settings.Left;
            Width = settings.Width;
            Height = settings.Height;

            var data = _fileManager.LoadJson<Data>() ?? new Data();
            Input.Text = data.Input;

            base.OnInitialized(e);
        }
    }

    public class Data
    {
        public string Input { get; set; }
    }

    public class WindowSettings
    {
        public double Top { get; set; }
        public double Left { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }

        public WindowSettings()
        {
            Top = 0;
            Left = 0;
            Width = 800;
            Height = 600;
        }
    }
}