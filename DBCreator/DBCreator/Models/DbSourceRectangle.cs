using SQLite;

namespace DBCreator.Models
{
    public class DbSourceRectangle
    {
        [PrimaryKey]
        public string Name { get; set; }
        public string Spritesheet { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public DbSourceRectangle() { }
        public DbSourceRectangle( string name, string spritesheet, int x, int y, int w, int h )
        {
            Name = name;
            Spritesheet = spritesheet;
            X = x;
            Y = y;
            Width = w;
            Height = h;
        }
    }
}
