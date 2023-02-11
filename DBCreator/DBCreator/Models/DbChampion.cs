using SQLite;

namespace DBCreator.Models
{
    public class DbChampion
    {
        [PrimaryKey]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public string Fighter { get; set; }
        public string Ninja { get; set; }
        public string Priest { get; set; }
        public string Wizard { get; set; }
        public string Head { get; set; }
        public string Neck { get; set; }
        public string Torso { get; set; }
        public string Legs { get; set; }
        public string Feet { get; set; }
        public string Hand { get; set; }
        public string Weapon { get; set; }
        public string Pouch { get; set; }
        public string Quiver { get; set; }
        public string Inventory { get; set; }
        public int Swing { get; set; }
        public int Thrust { get; set; }
        public int Club { get; set; }
        public int Parry { get; set; }
        public int Steal { get; set; }
        public int Fight { get; set; }
        public int Throw { get; set; }
        public int Shoot { get; set; }
        public int Identify { get; set; }
        public int Heal { get; set; }
        public int Influence { get; set; }
        public int Defend { get; set; }
        public int Fire { get; set; }
        public int Air { get; set; }
        public int Earth { get; set; }
        public int Water { get; set; }
        public int Health { get; set; }
        public int Stamina { get; set; }
        public int Mana { get; set; }
        public int Luck { get; set; }
        public int Strength { get; set; }
        public int Dexterity { get; set; }
        public int Wisdom { get; set; }
        public int Vitality { get; set; }
        public int AntiMagic { get; set; }
        public int AntiFire { get; set; }
        public int MaxLoad { get; set; }
        // reincarnate values
        public int ReLuck { get; set; } 
        public int ReStrength { get; set; }
        public int ReDexterity { get; set; }
        public int ReWisdom { get; set; }
        public int ReVitality { get; set; }
        public int ReAntiMagic { get; set; }
        public int ReAntiFire { get; set; }
        public int ReMaxLoad { get; set; }
        public DbChampion() { }
        public DbChampion(string name)
        { 
            Name = name;
        }
    }
}
