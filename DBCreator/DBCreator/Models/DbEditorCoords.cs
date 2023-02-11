using SQLite;

namespace DBCreator.Models
{
    public class DbEditorCoords
    {
        public string PointsName { get; set; }
        public int Index { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public DbEditorCoords() { }

        public DbEditorCoords(string pointsName, int index, int x, int y)
        {
            PointsName = pointsName;
            Index = index;
            X = x;
            Y = y;
        }
    }
}
