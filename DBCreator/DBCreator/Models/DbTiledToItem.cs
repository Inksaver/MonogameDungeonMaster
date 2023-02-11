using SQLite;

namespace DBCreator.Models
{
    internal class DbTiledToItem
    {
        [PrimaryKey]
        public int ID { get; set; }
        public string Type { get; set; }
        public string N { get; set; }
        public string E { get; set; }
        public string S { get; set; }
        public string W { get; set; }
        public DbTiledToItem() { }
        public DbTiledToItem(int id, string type, string n, string e, string s, string w)
        {
            ID = id;
            Type = type;
            N = n;
            E = e;
            S = s;
            W = w;
        }

    }
}
