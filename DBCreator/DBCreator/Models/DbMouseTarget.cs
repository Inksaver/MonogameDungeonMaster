using SQLite;

namespace DBCreator.Models
{
    public class DbMouseTarget
    {
        [PrimaryKey]
        public string Name { get; set; }
        public string Group { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public DbMouseTarget() { }
        public DbMouseTarget(string name, string group, int x, int y, int w, int h)
        {
            Name = name;
            Group = group;
            X = x;
            Y = y;
            Width = w;
            Height = h;
        }
    }
}
