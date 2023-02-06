using System.Collections.Generic;

namespace XMLDungeonReader
{
    /// <summary>
    /// http://dmweb.free.fr/?q=node/691
    /// All 24 Champions are represented by objects from this class
    /// They are created from a text file and stored in a Shared.Champions dictionary using the name as a key
    /// Health, Stamina and Mana vary due to injury,use of weapons or casting spells. Restored with sleep and potions
    /// These levels also shown on 3 bar graphs per Champion at top of the screen
    /// Basic Skills: 00:Fighter;01:Ninja;02:Priest;03:Wizard
    /* Skill Levels
    1. - (0)
    2. Neophyte(500)
    3. Novice(1000)
    4. Apprentice(2000)
    5. Journeyman(4000)
    6. Craftsman(8000)
    7. Artisan(16000)
    8. Adept(32000)
    9. Expert(64000)
    10. Lo Master(128000)
    11. Um Master(256000)
    12. On Master(512000)
    13. Ee Master(1024000)
    14. Pal Master(2048000)
    15. Mon Master(4096000)
    16. Archmaster(8192000)
    
    Health: Current health increased with sleeping and healing potions. Speed increase depends on Vitality
    Strength: Weapon striking power and load carrying
    Stamina: decreases walk, fight, hungry or thirsty.
    Dexterity: Throwing ability and weapon use eg slash/stab
    Wisdom: Spell learning ability and Mana recovery
    Vitality: Recovery speed from wounds/ resist injury
    AntiMagic: Resistance to magic attacks
    AntiFire: Resistance to fire attacks
    Luck: Hidden attribute
    */
    /// </summary>
    internal class Champion
    {
        public string ID { get; set; }                  // Allocated when Champions.txt is parsed. Used later to find Champion
        public string Name { get; set; }                // Champion name first name max 7 letters
        public string Title { get; set; }               // optional
        public float MaxLoad { get; set; } = 100;       // Max carrying capacity BaseMaxLoad = (8 x CurrentStrength + 100) / 10 Kg. eg If Stamina >= MaxStamina/2 -> MaxLoad = BaseMaxLoad
        public Dictionary<string, string> Skills { get; set; } = new Dictionary<string, string>
        {
            {"Fighter", ""},
            {"Ninja", ""},
            {"Priest", ""},
            {"Wizard", ""}
        };
        /// <summary>
        /// Stats is initialised in the constructor with default integer values
        /// </summary>
        public Dictionary<string, int> Stats { get; set; } // hidden skill experience + basic skill experience / 2.
        public Dictionary<string, string[]> Items { get; set; } = new Dictionary<string, string[]>
        {
            {"Head",        new string[]{""}},
            {"Neck",        new string[]{""}},
            {"Torso",       new string[]{""}},
            {"Hand",        new string[]{""}},
            {"Weapon",      new string[]{""}},
            {"Legs",        new string[]{""}},
            {"Feet",        new string[]{""}},
            {"Pouch",       new string[]{"",""}} ,
            {"Quiver",      new string[]{"","","","","",""}},
            {"Inventory",   new string[]{"","","","","","","","","","","","","","","",""}}
        };
        public Champion(string name)
        {
            //ID = id;
            Name = name;
            Stats = new Dictionary<string, int>
            {
                {"Fighter",0},{"Ninja",0},{"Priest",0},{"Wizard",0},
                {"Swing",0},{"Thrust",0},{"Club",0},{"Parry",0},
                {"Steal",0},{"Fight",0},{"Throw",0},{"Shoot",0},
                {"Identify",0},{"Heal",0},{"Influence",0},{"Defend",0},
                {"Fire",0},{"Air",0},{"Earth",0},{"Water",0},
                {"Health",0},{"Stamina",0},{"Mana",0},{"Luck",0},
                {"Strength",0},{"Dexterity",0},{"Wisdom",0},{"Vitality",0},
                {"AntiMagic",0},{"AntiFire",0}
            };
        }
    }
}
