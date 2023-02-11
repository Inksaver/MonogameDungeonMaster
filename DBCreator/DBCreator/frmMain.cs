using DBCreator.Models;
using SQLite;
using System.Data;
using System.Linq;

namespace DBCreator
{
    // https://www.youtube.com/playlist?list=PLJJcOjd3n1ZeEvwvLeWI96_bkQOJg3-cx
    // https://github.com/oysteinkrog/SQLite.Net-PCL

    public partial class frmMain : Form
    {
        SQLiteConnection db;
        private string dbPath = "";
        private string dbName = "DMData.db";
        private string dbPathAndFile = "";
        private string dataSource = "";
        private int DungeonY = 84;
        private List<string> ImagePaths = new List<string>()
        {
            "Champions",
            "Doors",
            "Floors",
            "GUI",
            "Items",
            "Monsters1",
            "Start",
            "Stairs",
            "WallDecorations",
            "WallsLayer1",
            "WallsLayer2",
            "WallsLayer3+4"
        };
        public frmMain()
        {
            InitializeComponent();
            this.CenterToScreen();
        }
        private void btnAddChampions_Click(object sender, EventArgs e) { AddChampions(true); }
        private void btnAddCoords_Click(object sender, EventArgs e) { AddCoordinates(true); }
        private void btnAddItems_Click(object sender, EventArgs e) { AddItems(true); }
        private void btnAddMouseTargets_Click(object sender, EventArgs e) { AddMouseTargets(true); }
        private void btnAddSourceRects_Click(object sender, EventArgs e) { AddSourceRectangles(true); }
        private void btnCreateWallDecorations_Click(object sender, EventArgs e) { CreateWallDecorationCoords(); }
        private void frmMain_Shown(object sender, EventArgs e) { Initialise(); }
        private void btnRebuild_Click(object sender, EventArgs e) { Rebuild(); }
        private void btnAddLevels_Click(object sender, EventArgs e) { AddLevels(true); }
        private void btnAddTiled_Click(object sender, EventArgs e) { AddTiledToItem(true); }
        private void btnEditorCoords_Click(object sender, EventArgs e) { AddEditorCoordinates(true); }
        private void lstTables_Click(object sender, EventArgs e) { DisplayTable(); }
        private void ConfirmRequest(bool replace, string message)
        {
            if (replace)
            {
                MessageBox.Show(message);
                FillListBox();
            }
            this.Cursor = Cursors.Default;
        }
        private void AddItems(bool replace)
        {
            this.Cursor = Cursors.WaitCursor;
            if (replace)
            {
                if (TableExists("DbItem"))
                    db.DropTable<DbItem>();
                db.CreateTable<DbItem>();
            }

            List<string> lines = ProcessTextFile(Path.Combine(dataSource, "Items.txt"));
            foreach (string line in lines) // Moonstone=Weight:0.2;Mana:3;Influence:1;Active:Neck
            {
                string[] lineLevel1 = DecodeData(line, "=");       // Weight:0.2;Mana:3;Influence:1;Active:Neck
                string key = lineLevel1[0];                        // Moonstone
                string[] data = DecodeData(lineLevel1[1], ";");    // Weight:0.2 Mana:3 Influence:1 Active:Neck

                DbItem item = new DbItem(lineLevel1[0]);
                // Item object created, now update it
                for (int i = 0; i < data.Length; i++)
                {
                    string[] properties = DecodeData(data[i], ":");  // eg {"Swing", "2", "0" }
                    if (properties[0] == "Type")
                        item.Type = properties[1];
                    else if (properties[0] == "Active")
                        item.ActiveArea = properties[1];
                    else if (properties[0] == "Locations") // convert to string[] when Item object is created
                        item.Locations = properties[1];
                    else if (properties[0] == "Weight")
                        item.Weight = Convert.ToDecimal(properties[1]);
                    else if (properties[0] == "Damage")
                        item.Damage = Convert.ToInt32(properties[1]);
                    else if (properties[0] == "Distance")
                        item.Distance = Convert.ToInt32(properties[1]);
                    else if (properties[0] == "Armour")
                        item.Armour = Convert.ToInt32(properties[1]);
                    else if (properties[0] == "Sharp Resist")
                        item.SharpResist = Convert.ToInt32(properties[1]);
                    else if (properties[0] == "Value")
                        item.Value = Convert.ToInt32(properties[1]);
                    else if (properties[0] == "Charges")
                        item.Charges = Convert.ToInt32(properties[1]);
                    else if (properties.Length == 3) // property, damage, level:
                    {
                        if (string.IsNullOrEmpty(item.Ability1))
                            item.Ability1 = $"{properties[0]}:{properties[1]}:{properties[2]}";
                        else if (string.IsNullOrEmpty(item.Ability2))
                            item.Ability2 = $"{properties[0]}:{properties[1]}:{properties[2]}";
                        else if (string.IsNullOrEmpty(item.Ability3))
                            item.Ability3 = $"{properties[0]}:{properties[1]}:{properties[2]}";
                    }
                    else // properties length will be 2 eg AntiMagic:15 or Skill14:1
                    {
                        if (string.IsNullOrEmpty(item.SkillBonus1))
                            item.SkillBonus1 = $"{properties[0]}:{properties[1]}";
                        else if (string.IsNullOrEmpty(item.SkillBonus2))
                            item.SkillBonus2 = $"{properties[0]}:{properties[1]}";
                    }

                }
                // now insert into db
                Insert(item);
            }
            ConfirmRequest(replace, "Items added to database");
        }
        private void AddChampions(bool replace)
        {
            this.Cursor = Cursors.WaitCursor;
            if (replace)
            {
                if (TableExists("DbChampion"))
                    db.DropTable<DbChampion>();
                db.CreateTable<DbChampion>();
            }
            DbChampion? champ = null;
            List<string> lines = ProcessTextFile(Path.Combine(dataSource, "Champions.txt"));
            foreach (string line in lines) // Name=Iaido
            {
                string[] data = DecodeData(line, "=");         // "Fighter", "Apprentice;(Swing: 2, Thrust: 3, Club: 0, Parry: 2)"

                if (data.Length > 1)                                                            // non-empty eg Name=Hawk
                {
                    if (data[0] == "Name")
                    {                                                         // eg Hawk
                        if (champ != null)
                            Insert(champ); // now insert into db
                        champ = new DbChampion(data[1]);
                    }
                    else
                    {
                        if (data[0].ToLower() == "id") champ.Id = Convert.ToInt32(data[1].Substring(1));         // Champion ID kept here. Should be unique
                        else if (data[0].ToLower() == "title") champ.Title = data[1];
                        else if (data[0] == "Fighter" || data[0] == "Ninja" || data[0] == "Priest" || data[0] == "Wizard")
                        {
                            // Fighter=Apprentice;Swing:2;Thrust:3;Club:0;Parry:2
                            string[] levelData = DecodeData(data[1], ";");   //  "Apprentice", "Swing:2", "Thrust:3", "Club:0"; "Parry:2"
                            foreach (string level in levelData) // {Apprentice, Swing:2, Thrust:3
                            {
                                if (level.Contains(":"))
                                {
                                    string[] levelDecoded = DecodeData(level, ":"); //"Swing:2"
                                    if (levelDecoded[0] == "Swing")
                                        champ.Swing = Convert.ToInt32(levelDecoded[1]);
                                    else if (levelDecoded[0] == "Thrust")
                                        champ.Thrust = Convert.ToInt32(levelDecoded[1]);
                                    else if (levelDecoded[0] == "Club")
                                        champ.Club = Convert.ToInt32(levelDecoded[1]);
                                    else if (levelDecoded[0] == "Parry")
                                        champ.Parry = Convert.ToInt32(levelDecoded[1]);
                                    else if (levelDecoded[0] == "Steal")
                                        champ.Steal = Convert.ToInt32(levelDecoded[1]);
                                    else if (levelDecoded[0] == "Fight")
                                        champ.Fight = Convert.ToInt32(levelDecoded[1]);
                                    else if (levelDecoded[0] == "Throw")
                                        champ.Throw = Convert.ToInt32(levelDecoded[1]);
                                    else if (levelDecoded[0] == "Shoot")
                                        champ.Shoot = Convert.ToInt32(levelDecoded[1]);
                                    else if (levelDecoded[0] == "Identify")
                                        champ.Identify = Convert.ToInt32(levelDecoded[1]);
                                    else if (levelDecoded[0] == "Heal")
                                        champ.Heal = Convert.ToInt32(levelDecoded[1]);
                                    else if (levelDecoded[0] == "Influence")
                                        champ.Influence = Convert.ToInt32(levelDecoded[1]);
                                    else if (levelDecoded[0] == "Defend")
                                        champ.Defend = Convert.ToInt32(levelDecoded[1]);
                                    else if (levelDecoded[0] == "Fire")
                                        champ.Fire = Convert.ToInt32(levelDecoded[1]);
                                    else if (levelDecoded[0] == "Air")
                                        champ.Air = Convert.ToInt32(levelDecoded[1]);
                                    else if (levelDecoded[0] == "Earth")
                                        champ.Earth = Convert.ToInt32(levelDecoded[1]);
                                    else if (levelDecoded[0] == "Water")
                                        champ.Water = Convert.ToInt32(levelDecoded[1]);
                                }
                                else
                                {
                                    if (data[0] == "Fighter")
                                        champ.Fighter = levelData[0]; //eg "Apprentice"
                                    else if (data[0] == "Ninja")
                                        champ.Ninja = levelData[0];
                                    else if (data[0] == "Priest")
                                        champ.Priest = levelData[0];
                                    else if (data[0] == "Wizard")
                                        champ.Wizard = levelData[0];
                                }
                            }
                        }
                        else if (data[0] == "Items")
                        {
                            /// Items=Torso:Leather Jerkin;Legs:Leather Pants;Feet:Leather Boots;Hand:Torch:14

                            string[] items = DecodeData(data[1], ";");    // Torso:Leather Jerkin;Legs:Leather Pants;Hand:Torch:14
                            foreach (string item in items)                                                  // Torso:Leather Jerkin or Hand:Torch:14 Could be 1 entry only
                            {
                                string[] subItems = DecodeData(item, ":"); // eg {Torso, Leather Jerkin} or {Hand, Torch, 14}
                                // subItems[0] is location of item eg Torso or Hand
                                // subItems[1] is item eg Torch
                                if (subItems[0] == "Head")
                                    champ.Head = CombineItems(subItems);                     // eg Champions["Hawk"]Items["Hand"][0] = "Torch:14"
                                else if (subItems[0] == "Neck")
                                    champ.Neck = CombineItems(subItems);
                                else if (subItems[0] == "Torso")
                                    champ.Torso = CombineItems(subItems);
                                else if (subItems[0] == "Legs")
                                    champ.Legs = CombineItems(subItems);
                                else if (subItems[0] == "Feet")
                                    champ.Feet = CombineItems(subItems);
                                else if (subItems[0] == "Hand")
                                    champ.Hand = CombineItems(subItems);
                                else if (subItems[0] == "Weapon")
                                    champ.Weapon = CombineItems(subItems);
                            }
                        }
                        else if ((data[0] == "Pouch" || data[0] == "Quiver" || data[0] == "Inventory") && data.Length > 1) // Quiver=0:Arrow,2:Arrow
                        {
                            if (data[0] == "Pouch")
                                champ.Pouch = data[1];                     // eg Champions["Hawk"]Items["Inventory"][0] = "Torch:14"
                            else if (data[0] == "Quiver")
                                champ.Quiver = data[1];
                            else if (data[0] == "Inventory")
                                champ.Inventory = data[1];
                        }
                        else if (data[0] == "Load")
                        {
                            int[] values = GetReincarnateSkills(data);
                            champ.MaxLoad = values[0];
                            champ.ReMaxLoad = values[1];
                        }
                        else if (data[0] == "Health")
                        {
                            if (int.TryParse(data[1], out int value))
                                champ.Health = value;
                        }
                        else if (data[0] == "Stamina")
                        {
                            if (int.TryParse(data[1], out int value))
                                champ.Stamina = value;
                        }
                        else if (data[0] == "Mana")
                        {
                            if (int.TryParse(data[1], out int value))
                                champ.Mana = value;
                        }
                        else if (data[0] == "Luck")                     // Luck=40;42 -> "Luck", "40;42"
                        {
                            int[] values = GetReincarnateSkills(data);  // "40;42" ->  "40","42"
                            champ.Luck = values[0];                     // 40
                            champ.ReLuck = values[1];                   // 42
                        }
                        else if (data[0] == "Strength")
                        {
                            int[] values = GetReincarnateSkills(data);
                            champ.Strength = values[0];
                            champ.ReStrength = values[1];
                        }
                        else if (data[0] == "Dexterity")
                        {
                            int[] values = GetReincarnateSkills(data);
                            champ.Dexterity = values[0];
                            champ.ReDexterity = values[1];
                        }
                        else if (data[0] == "Wisdom")
                        {
                            int[] values = GetReincarnateSkills(data);
                            champ.Wisdom = values[0];
                            champ.ReWisdom = values[1];
                        }
                        else if (data[0] == "Vitality")
                        {
                            int[] values = GetReincarnateSkills(data);
                            champ.Vitality = values[0];
                            champ.ReVitality = values[1];
                        }
                        else if (data[0] == "AntiMagic")
                        {
                            int[] values = GetReincarnateSkills(data);
                            champ.AntiMagic = values[0];
                            champ.ReAntiMagic = values[1];
                        }
                        else if (data[0] == "AntiFire")
                        {
                            int[] values = GetReincarnateSkills(data);
                            champ.AntiFire = values[0];
                            champ.ReAntiFire = values[1];
                        }
                    }
                }
            }
            Insert(champ); // final champion selected
            ConfirmRequest(replace, "Champions added to database");
        }
        private void AddCoordinates(bool replace)
        {
            this.Cursor = Cursors.WaitCursor;
            if (replace)
            {
                if (TableExists("DbCoordinate"))
                    db.DropTable<DbCoordinate>();
                db.CreateTable<DbCoordinate>();
            }
            foreach (string fileName in ImagePaths)
            {
                if (File.Exists(Path.Combine(dataSource, $"{fileName}.coords")))
                {
                    List<string> lines = ProcessTextFile(Path.Combine(dataSource, $"{fileName}.coords"));
                    bool isRelative = false;
                    foreach (string line in lines)
                    {
                        if (line.StartsWith("Relative="))
                        {
                            if (line.Substring(9, 4).ToLower() == "true")
                                isRelative = true;
                            else
                                isRelative = false;
                        }
                        else
                        {
                            string[] data = DecodeData(line, ",");   // eg Wall.L4,0,0 -> {"Wall.L4", "0", "0"}
                            if (data.Length >= 3)
                            {
                                if (int.TryParse(data[1], out int x) && int.TryParse(data[2], out int y))
                                {
                                    if (isRelative)
                                        y += DungeonY;
                                    Insert(new DbCoordinate(data[0], x, y));
                                }
                            }
                        }
                    }
                }
            }
            ConfirmRequest(replace, "Coordinates added to database");
        }
        private void AddEditorCoordinates(bool replace)
        {
            this.Cursor = Cursors.WaitCursor;
            if (replace)
            {
                if (TableExists("DbEditorCoords"))
                    db.DropTable<DbEditorCoords>();
                db.CreateTable<DbEditorCoords>();
            }
            if (File.Exists(Path.Combine(dataSource, "Editor.coords")))
            {
                List<string> lines = ProcessTextFile(Path.Combine(dataSource, "Editor.coords"));
                foreach (string line in lines)
                {
                    string[] data = DecodeData(line, ",");   // eg namePoints.0,353,192
                    if (int.TryParse(data[1], out int x) && int.TryParse(data[2], out int y))
                    {
                        string[] subData = DecodeData(data[0], ".");
                        int index = Convert.ToInt32(subData[1]);
                        Insert(new DbEditorCoords(subData[0], index, x, y + DungeonY)); // namePoints , 0, 353, 192 (108 + 84)
                    }
                }
            }
            ConfirmRequest(replace, "Editor Coordinates added to database");
        }
        private void AddLevels(bool replace)
        {
            this.Cursor = Cursors.WaitCursor;
            if (replace)
            {
                if (TableExists("DbLevel"))
                    db.DropTable<DbLevel>();
                db.CreateTable<DbLevel>();
            }
            if (File.Exists(Path.Combine(dataSource, "ChampionLevels.txt")))
            {
                List<string> lines = ProcessTextFile(Path.Combine(dataSource, "ChampionLevels.txt"));
                foreach (string line in lines)
                {
                    string[] data = DecodeData(line, ",");   // eg "Neophyte",500
                    string key = data[0].Replace('"', ' ').Trim();
                    //if (key == "") key = " ";
                    Insert(new DbLevel(key, Convert.ToInt32(data[1])));
                }
            }
            ConfirmRequest(replace, "Champion Levels added to database");
        }
        private void AddMouseTargets(bool replace)
        {
            this.Cursor = Cursors.WaitCursor;
            if (replace)
            {
                if (TableExists("DbMouseTarget"))
                    db.DropTable<DbMouseTarget>();
                db.CreateTable<DbMouseTarget>();
            }
            if (File.Exists(Path.Combine(dataSource, "MouseTargets.txt")))
            {
                List<string> lines = ProcessTextFile(Path.Combine(dataSource, "MouseTargets.txt"));
                bool isRelative = false;
                string group = "";
                foreach (string line in lines)  // eg Wood.C3,0,0,202,213
                {
                    if (line.StartsWith("Targets="))
                    {
                        string[] data = DecodeData(line.Substring(8), ":"); //Targets=Master:False
                        group = data[0];
                        if (data[1].ToLower() == "true")
                            isRelative = true;
                        else
                            isRelative = false;
                    }
                    else
                    {
                        string[] data = line.Split(",", StringSplitOptions.RemoveEmptyEntries);
                        if (int.TryParse(data[1], out int X) &&
                            int.TryParse(data[2], out int Y) &&
                            int.TryParse(data[3], out int Width) &&
                            int.TryParse(data[4], out int Height))
                        {
                            if (isRelative)
                                Y += DungeonY;
                            Insert(new DbMouseTarget(data[0], group, X, Y, Width, Height)); // key = "Wood.C3" value = string[]{"0","0","202","213"}
                        }
                    }
                }
            }
            ConfirmRequest(replace, "Mouse Targets added to database");
            if (File.Exists(Path.Combine(dataSource, "Floor+AlcoveRectangles.txt")))
            {
                this.Cursor = Cursors.WaitCursor;
                List<string> lines = ProcessTextFile(Path.Combine(dataSource, "Floor+AlcoveRectangles.txt"));
                foreach (string line in lines)  // eg Wood.C3,0,0,202,213
                {
                    string[] data = line.Split(",", StringSplitOptions.RemoveEmptyEntries);
                    if (int.TryParse(data[1], out int X) &&
                        int.TryParse(data[2], out int Y) &&
                        int.TryParse(data[3], out int Width) &&
                        int.TryParse(data[4], out int Height))
                    {
                        Y += DungeonY;
                        Insert(new DbMouseTarget(data[0], "FloorAlcove", X, Y, Width, Height)); // key = "Wood.C3" value = string[]{"0","0","202","213"}
                    }
                }
            }
            ConfirmRequest(replace, "Floor Alcive Rectangles added to database");
        }
        private void AddSourceRectangles(bool replace)
        {
            this.Cursor = Cursors.WaitCursor;
            if (replace)
            {
                if (TableExists("DbSourceRectangle"))
                    db.DropTable<DbSourceRectangle>();
                db.CreateTable<DbSourceRectangle>();
            }
            foreach (string fileName in ImagePaths)
            {
                if (File.Exists(Path.Combine(dataSource, $"{fileName}.data")))
                {
                    List<string> lines = ProcessTextFile(Path.Combine(dataSource, $"{fileName}.data"));
                    foreach (string line in lines)  // eg Wood.C3,0,0,202,213
                    {
                        string[] data = line.Split(",", StringSplitOptions.RemoveEmptyEntries);
                        if (int.TryParse(data[1], out int X) &&
                            int.TryParse(data[2], out int Y) &&
                            int.TryParse(data[3], out int Width) &&
                            int.TryParse(data[4], out int Height))
                        {
                            Insert(new DbSourceRectangle(data[0], fileName, X, Y, Width, Height)); // key = "Wood.C3" value = string[]{"0","0","202","213"}
                        }
                    }
                }
            }
            if (File.Exists(Path.Combine(dataSource, "Fonts.font")))
            {
                string fontName = "";
                List<string> lines = ProcessTextFile(Path.Combine(dataSource, "Fonts.font"));
                foreach (string line in lines)  // eg Font:Gilded.24 or A,0,0,16,24
                {
                    if (line.StartsWith("Font")) // eg Font:Gilded.24
                    {
                        string[] data = DecodeData(line, ":");
                        fontName = data[1]; // eg Gilded.24
                    }
                    else
                    {
                        string[] data = DecodeData(line, ","); // eg  A,0,0,16,24
                        if (int.TryParse(data[1], out int X) &&
                            int.TryParse(data[2], out int Y) &&
                            int.TryParse(data[3], out int Width) &&
                            int.TryParse(data[4], out int Height))
                        {
                            Insert(new DbSourceRectangle($"{fontName}.{data[0]}", "Fonts", X, Y, Width, Height)); // key = "Gilded.24.A"
                        }
                    }
                }
            }
            ConfirmRequest(replace, "Source Rectangles added to database");
        }
        private void AddTiledToItem(bool replace)
        {
            this.Cursor = Cursors.WaitCursor;
            if (replace)
            {
                if (TableExists("DbTiledToItem"))
                    db.DropTable<DbTiledToItem>();
                db.CreateTable<DbTiledToItem>();
            }
            if (File.Exists(Path.Combine(dataSource, "TiledToItem.txt")))
            {
                List<string> lines = ProcessTextFile(Path.Combine(dataSource, "TiledToItem.txt"));
                foreach (string line in lines)  // eg 1=Wall:Wall;Wall;Wall;Wall
                {
                    string[] data = DecodeData(line, "="); // eg "1", "Wall:Wall;Wall;Wall;Wall"
                    int key = Convert.ToInt32(data[0]);
                    string[] subData = DecodeData(data[1], ":"); // eg "Wall" "Wall;Wall;Wall;Wall"
                    string type = subData[0];
                    string[] subSubData = DecodeData(subData[1], ";"); // eg "Wall","Wall","Wall","Wall"
                    for (int i = 0; i < subSubData.Length; i++)
                    {
                        if (subSubData[i] == "Null")
                            subSubData[i] = "";
                    }
                    Insert(new DbTiledToItem(key, type, subSubData[0], subSubData[1], subSubData[2], subSubData[3]));
                }
            }
            ConfirmRequest(replace, "TiledToItems added to database");
        }
        private void CreateConfig()
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            do
            {
                folderBrowserDialog.Description = "Select Directory containing data files eg .txt, .data, .coords";
                DialogResult result = folderBrowserDialog.ShowDialog();
                if (result == DialogResult.OK)
                    dataSource = folderBrowserDialog.SelectedPath;
                else
                    dataSource = "";
            } while (dataSource == "");

            do
            {
                folderBrowserDialog.Description = "Select directory where database is to be created";
                DialogResult result = folderBrowserDialog.ShowDialog();
                if (result == DialogResult.OK)
                    dbPath = folderBrowserDialog.SelectedPath;
                else
                    dbPath = "";
            } while (dbPath == "");
            do
            {
                InputBox("Database name requested (.db default extension)", "Type a name for the database", ref dbName);
            } while (dbName == "");

            if (!dbName.Contains("."))
                dbName += ".db";

            using (StreamWriter f = new StreamWriter("IOPaths.txt"))
            {
                f.WriteLine($"dataSource={dataSource}");
                f.WriteLine($"dbPath={dbPath}");
                f.WriteLine($"dbName={dbName}");
            }
        }
        private void DisplayTable()
        {
            string tableName = lstTables.Text;
            if (tableName == "DbChampion")
                dgvDMData.DataSource = db.Table<DbChampion>().ToList<DbChampion>();
            else if (tableName == "DbCoordinate")
                dgvDMData.DataSource = db.Table<DbCoordinate>().ToList<DbCoordinate>();
            else if (tableName == "DbEditorCoords")
                dgvDMData.DataSource = db.Table<DbEditorCoords>().ToList<DbEditorCoords>();
            else if (tableName == "DbItem")
                dgvDMData.DataSource = db.Table<DbItem>().ToList<DbItem>();
            else if (tableName == "DbLevel")
                dgvDMData.DataSource = db.Table<DbLevel>().ToList<DbLevel>();
            else if (tableName == "DbMouseTarget")
                dgvDMData.DataSource = db.Table<DbMouseTarget>().ToList<DbMouseTarget>();
            else if (tableName == "DbSourceRectangle")
                dgvDMData.DataSource = db.Table<DbSourceRectangle>().ToList<DbSourceRectangle>();
            else if (tableName == "DbTiledToItem")
                dgvDMData.DataSource = db.Table<DbTiledToItem>().ToList<DbTiledToItem>();
        }
        private string CombineItems(string[] items)
        {
            string retValue = "";
            if (items.Length > 2)                                                    // eg {Hand, Torch, 14}
                retValue = $"{items[1]}:{items[2]}";
            else
                retValue = items[1];
            return retValue;
        }
        private void CreateWallDecorationCoords()
        {
            /// This is a one-off helper function to complete the coordinates table (361 items)
            /// Takes the rectangles created by Gimp xcf files and exported to
            /// WallDecorations.data by the console based XCFtoCSV.exe
            /// and calculates the coordinates for the items to be drawn
            /// in the dingeon.
            /// Items not included in the list below are excluded so have to be manually added
            string source = Path.Combine(dataSource, "WallDecorations.Data");
            string saveFile = "WallDecorationsXtra.coords";
            List<string> lines = ProcessTextFile(source);
            int centreX = 224;
            int centreY = 120;
            //delete if exists
            if (File.Exists(saveFile)) File.Delete(saveFile);
            //open filename
            using (StreamWriter f = new StreamWriter(saveFile))
            {
                foreach (string line in lines)
                {
                    if (line.Contains("hook") ||
                        line.Contains("ring") ||
                        line.Contains("gem") ||
                        line.Contains("hole") ||
                        line.Contains("slot") ||
                        line.Contains("key") ||
                        line.Contains("Lever") ||
                        line.Contains("Launcher"))
                    {
                        // write a coordinate to file
                        string[] data = DecodeData(line, ",");                              // Wallhook.L3,0,69,10,27
                        string position = data[0].Substring(data[0].IndexOf(".") + 1, 1);   // C or L or R
                        string layer = data[0].Substring(data[0].IndexOf(".") + 2, 1);      // 1, 2 or 3
                        int width = Convert.ToInt32(data[3]);
                        int height = Convert.ToInt32(data[4]);
                        if (position == "C" && layer == "3")
                        {
                            centreX = 224;
                            centreY = 120;
                        }
                        else if (position == "C" && layer == "2")
                        {
                            centreX = 224;
                            centreY = 111;
                        }
                        else if (position == "C" && layer == "1")
                        {
                            centreX = 224;
                            centreY = 106;
                        }
                        else if (position == "L" && layer == "3")
                        {
                            centreX = 101;
                            centreY = 113;
                        }
                        else if (position == "L" && layer == "2")
                        {
                            centreX = 139;
                            centreY = 107;
                        }
                        else if (position == "L" && layer == "1")
                        {
                            centreX = 162;
                            centreY = 104;
                        }
                        else if (position == "R" && layer == "3")
                        {
                            centreX = 346;
                            centreY = 113;
                        }
                        else if (position == "R" && layer == "2")
                        {
                            centreX = 308;
                            centreY = 107;
                        }
                        else if (position == "R" && layer == "1")
                        {
                            centreX = 284;
                            centreY = 104;
                        }
                        string output = $"{data[0]},{centreX - (width / 2)},{centreY - (height / 2)}";
                        f.WriteLine(output);
                    }
                }
            }
        }
        private void FillListBox()
        {
            lstTables.Items.Clear();
            if (File.Exists(dbPathAndFile))
            {
                if (TableExists("DbItem"))
                    lstTables.Items.Add("DbItem");
                if (TableExists("DbChampion"))
                    lstTables.Items.Add("DbChampion");
                if (TableExists("DbCoordinate"))
                    lstTables.Items.Add("DbCoordinate");
                if (TableExists("DbSourceRectangle"))
                    lstTables.Items.Add("DbSourceRectangle");
                if (TableExists("DbMouseTarget"))
                    lstTables.Items.Add("DbMouseTarget");
                if (TableExists("DbLevel"))
                    lstTables.Items.Add("DbLevel");
                if (TableExists("DbTiledToItem"))
                    lstTables.Items.Add("DbTiledToItem");
                if (TableExists("DbEditorCoords"))
                    lstTables.Items.Add("DbEditorCoords");
            }
        }
        private int[] GetReincarnateSkills(string[] data)
        {
            int[] retValue = { -1, -1 };
            string[] subData = DecodeData(data[1], ";"); // "40,42" -> 40, 42
            int.TryParse(subData[0], out retValue[0]);
            int.TryParse(subData[1], out retValue[1]);

            return retValue;
        }
        private void Insert(Object obj)
        {
            db.Insert(obj);
        }
        private void Initialise()
        {
            bool createConfig = false;
            if (!File.Exists("IOPaths.txt"))
                createConfig = true;
            else
            {
                List<string> lines = ProcessTextFile("IOPaths.txt");
                if (lines.Count != 3)
                {
                    MessageBox.Show("Config file IOPaths.txt does not contain just 3 lines: re-configuring...");
                    createConfig = true;
                }
                else
                {
                    foreach (string line in lines) // {dataSource=C:\Users, dbPath=C:\Users\Projects, dbName=DMData.db}
                    {
                        string[] data = DecodeData(line, "=");  // eg dataSource=C:\Users
                        if (data[0] == "dataSource")
                        {
                            if (!Directory.Exists(data[1]))
                                createConfig = true;
                            else
                                dataSource = data[1];
                        }
                        if (data[0] == "dbPath")
                        {
                            if (!Directory.Exists(data[1]))
                                createConfig = true;
                            else
                                dbPath = data[1];
                        }
                        if (data[0] == "dbName")
                        {
                            dbName = data[1];
                        }
                    }
                }
            }
            if (createConfig) // missing or incorrect config file
                CreateConfig();

            dbPathAndFile = Path.Combine(dbPath, dbName);
            CheckDatabase();
        }
        private void CheckDatabase()
        {
            if (!File.Exists(dbPathAndFile))
            {
                db = new SQLiteConnection(dbPathAndFile);
                db.CreateTable<DbItem>();
                db.CreateTable<DbChampion>();
                db.CreateTable<DbCoordinate>();
                db.CreateTable<DbSourceRectangle>();
                db.CreateTable<DbMouseTarget>();
                db.CreateTable<DbLevel>();
                db.CreateTable<DbEditorCoords>();
                db.CreateTable<DbTiledToItem>();
            }
            else
                db = new SQLiteConnection(dbPathAndFile);

            FillListBox();
        }
        public bool TableExists(string tableName)
        {
            /// Checks the database to see if the table exists
            TableMapping map = new TableMapping(typeof(SqlDbType)); // Instead of mapping to a specific table just map the whole database type
            object[] ps = new object[0]; // An empty parameters object since I never worked out how to use it properly! (At least I'm honest)

            int tableCount = db.Query(map, "SELECT * FROM sqlite_master WHERE type = 'table' AND name = '" + tableName + "'", ps).Count; // Executes the query from which we can count the results
            if (tableCount == 0)
                return false;
            else if (tableCount == 1)
                return true;
            else
                throw new Exception("More than one table by the name of " + tableName + " exists in the database.", null);
        }
        private string[] DecodeData(string data, string separator) //  eg Swing:2:0
        {
            /// return a string array via splitting text using 'separator' eg "="
            return data.Split(separator, StringSplitOptions.RemoveEmptyEntries); // eg {"Swing", "2", "0" }
        }
        private List<string> ProcessTextFile(string filename)
        {
            List<string> retValue = new List<string>();

            string[] lines = File.ReadAllLines(filename);
            foreach (string line in lines)
            {
                if (!line.StartsWith('#') && line.Trim().Length > 0)
                {
                    retValue.Add(line);
                }
            }
            return retValue;
        }
        private void Rebuild()
        {
            this.Cursor = Cursors.WaitCursor;
            if (File.Exists(dbPathAndFile))
            {
                db.Close();
                File.Delete(dbPathAndFile);
            }

            CheckDatabase();
            AddItems(false);
            AddChampions(false);
            AddCoordinates(false);
            AddMouseTargets(false);
            AddSourceRectangles(false);
            AddLevels(false);
            AddTiledToItem(false);
            AddEditorCoordinates(false);

            FillListBox();
            this.Cursor = Cursors.Default;
            MessageBox.Show("Database rebuilt");
        }
        public static DialogResult InputBox(string title, string promptText, ref string value)
        {
            Form form = new Form();
            Label label = new Label();
            TextBox textBox = new TextBox();
            Button buttonOk = new Button();
            Button buttonCancel = new Button();

            form.Text = title;
            label.Text = promptText;
            textBox.Text = value;

            buttonOk.Text = "OK";
            buttonCancel.Text = "Cancel";
            buttonOk.DialogResult = DialogResult.OK;
            buttonCancel.DialogResult = DialogResult.Cancel;

            label.SetBounds(9, 20, 372, 13);
            textBox.SetBounds(12, 36, 372, 20);
            buttonOk.SetBounds(228, 72, 75, 23);
            buttonCancel.SetBounds(309, 72, 75, 23);

            label.AutoSize = true;
            textBox.Anchor = textBox.Anchor | AnchorStyles.Right;
            buttonOk.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            buttonCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

            form.ClientSize = new Size(396, 107);
            form.Controls.AddRange(new Control[] { label, textBox, buttonOk, buttonCancel });
            form.ClientSize = new Size(Math.Max(300, label.Right + 10), form.ClientSize.Height);
            form.FormBorderStyle = FormBorderStyle.FixedDialog;
            form.StartPosition = FormStartPosition.CenterScreen;
            form.MinimizeBox = false;
            form.MaximizeBox = false;
            form.AcceptButton = buttonOk;
            form.CancelButton = buttonCancel;

            DialogResult dialogResult = form.ShowDialog();
            value = textBox.Text;
            return dialogResult;
        }
    }
}