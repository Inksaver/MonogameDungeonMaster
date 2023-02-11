using SQLite;

namespace DBCreator.Models
{
    public class DbLevel
    {
        [PrimaryKey]
        public string Name { get; set; }
        public int Level { get; set; }
        public DbLevel() { }
        public DbLevel(string name, int level)
        {
            Name = name;
            Level = level;
        }
    }
}
