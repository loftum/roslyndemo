using System;
using System.ComponentModel;
using System.Windows;
using Convenient.Stuff.IO;
using Visualizer.ViewModels;

namespace Visualizer
{
    public partial class MainWindow
    {
        private readonly FileManager _fileManager = new FileManager();
        protected MainViewModel Vm => (MainViewModel)DataContext;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Parse_Click(object sender, RoutedEventArgs e)
        {
            Vm.Parse(Input.Text);
        }

        private void Compile_Click(object sender, RoutedEventArgs e)
        {
            Vm.Compile(Input.Text);
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
