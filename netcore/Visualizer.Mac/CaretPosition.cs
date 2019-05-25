namespace Visualizer.Mac
{
    public struct CaretPosition
    {
        public int Location { get; }
        public int Row { get; }
        public int Column { get; }

        public CaretPosition(int location, int row, int column)
        {
            Location = location;
            Row = row;
            Column = column;
        }
    }
}
