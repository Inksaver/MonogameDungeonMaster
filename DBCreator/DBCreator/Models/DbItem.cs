using SQLite;

namespace DBCreator.Models
{
    public class DbItem
    {
        [PrimaryKey]
        public string Name { get; set; }
        public string Type { get; set; }
        public decimal Weight { get; set; }
        public int Charges { get; set; }
        public int Damage { get; set; }   
        public int ShootDamage { get; set; }
        public int Distance { get; set; }
        public int MinState { get; set; }
        public int MaxState { get; set; }
        public string ActiveArea { get; set; }
        public string Ability1 { get; set; }
        public string Ability2 { get; set; }
        public string Ability3 { get; set; }
        public string SkillBonus1 { get; set; }
        public string SkillBonus2 { get; set; }
        public int Armour { get; set; }
        public int SharpResist { get; set; }
        public int Value { get; set; }
        public string Locations { get; set; }
        public DbItem() { }
        public DbItem(string name)
        {
            Name = name;
        }
    }
}
