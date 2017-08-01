using System.Windows;
using Editor.ViewModels;

namespace Editor
{
    public partial class MainWindow
    {
        protected MainViewModel Vm => (MainViewModel) DataContext;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Parse_Click(object sender, RoutedEventArgs e)
        {
            Vm.Parse(Input.Text);
        }
    }
}
