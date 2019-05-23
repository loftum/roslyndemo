namespace Convenient.Stuff.Wpf
{
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