using SQLite;

namespace DBCreator.Models
{
    public class DbCoordinate
    {
        [PrimaryKey]
        public string Name { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public DbCoordinate() { }

        public DbCoordinate(string name, int x, int y)
        {
            Name = name;
            X = x;
            Y = y;
        }
    }
}
