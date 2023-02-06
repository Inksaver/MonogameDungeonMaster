using System.Collections.Generic;

namespace XMLDungeonReader
{
    internal class Item
    {
        public string Name { get; set; }
        public string ImageName { get; set; }
        public List<string> ImageNames { get; set; } = new List<string>();
        public Dictionary<string, string> Properties { get; set; } = new Dictionary<string, string>
        {
            {"Weight", "0"  }, {"Action",""      }, {"Damage", "0"   }, {"Active",""        },
            {"Health", "0"  }, {"Stamina", "0"   }, {"Mana", "0"     }, {"Luck", "0"        },
            {"Strength","0" }, {"Dexterity","0"  }, {"Wisdom", "0"   }, {"Vitality", "0"    },
            {"AntiMagic","0"}, {"AntiFire","0"   }, {"Load", "0"     }, {"Distance", "0"    },
            {"Swing", "0"   }, {"Thrust", "0"    }, {"Club", "0"     }, {"Parry", "0"       },
            {"Brandish","0" }, {"Slash", "0"     }, {"Jab", "0"      }, {"Chop", "0"        },
            {"Melee", "0"   }, {"Stab", ""       }, {"Cleave","0"    }, {"Disrupt","0"      },
            {"Bash", "0"    }, {"Stun", "0"      }, {"Berzerk", "0"  }, {"Shoot Damage", "0"},
            {"Fireball", "0"}, {"Lightning", "0" }, {"Confuse", "0"  }, {"Value","0"        },
            {"Steal", "0"   }, {"Fight", "0"     }, {"Throw", "0"    }, {"Shoot", "0"       },
            {"Identify", "0"}, {"Heal", "0"      }, {"Influence", "0"}, {"Defend", "0"      },
            {"Light", "0"   }, {"Dispell", "0"   }, {"Armour", "0"   }, {"Sharp Resist", "0"},
            {"Block","0"    }, {"Hit", "0"       }, {"Shield", "0"   }, {"Food", "0"        },
            {"Water", "0"   }, {"Calm", "0"      }, {"Fireshield","0"}, {"Spellshield", "0" },
            {"Charges", "0" }, {"Skill13","0"    }, {"Skill14","0"   }, {"Skill15","0"      },
            {"Window",""    }, {"Freeze Life","0"}, {"Climb Down","0"}, {"Punch", "0"       },
            {"Blow Horn","0"}
        };
        public Item(string name)
        {
            Name = name;
            ImageName = name;
        }
    }
}
