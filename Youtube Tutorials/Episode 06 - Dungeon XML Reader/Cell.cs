using System.Collections.Generic;
using System.Drawing;

namespace XMLDungeonReader
{
    internal class Cell
    {
        #region Public properties
        public Dictionary<string, string> AlcoveItems { get; set; } = new Dictionary<string, string>
            {
            {"N", "" },
            {"E", "" },
            {"S", "" },
            {"W", "" }
        };
        public Dictionary<string, string> Champions { get; set; } = new Dictionary<string, string>
        {
            {"N", "" },
            {"E", "" },
            {"S", "" },
            {"W", "" }
        };
        public Dictionary<string, string> FloorItems { get; set; } = new Dictionary<string, string>
        {
            {"NE", "" },
            {"NW", "" },
            {"SE", "" },
            {"SW", "" }
        };
        public Dictionary<string, string> Walls { get; set; } = new Dictionary<string, string>
        {
            {"N", "" },
            {"E", "" },
            {"S", "" },
            {"W", "" }
        };
        public Dictionary<string, string> WallActuators { get; set; } = new Dictionary<string, string>
            {
            {"N", "" },
            {"E", "" },
            {"S", "" },
            {"W", "" }
        };
        public Dictionary<string, string> WallDecorations { get; set; } = new Dictionary<string, string>
        {
            {"N", "" },
            {"E", "" },
            {"S", "" },
            {"W", "" }
        };
        public Dictionary<string, string> WallWritingKeys { get; set; } = new Dictionary<string, string>
        {
            {"N", "" },
            {"E", "" },
            {"S", "" },
            {"W", "" }
        };
        public Dictionary<string, string> Doors { get; set; } = new Dictionary<string, string>
        {
            //  eg "Doortrack" "Grate" "Doortrack" "Grate"
            {"N", "" },
            {"E", "" },
            {"S", "" },
            {"W", "" }
        };
        public int X { get; set; }                          // X coord of this cell
        public int Y { get; set; }                          // Y coord of this cell
        public string Ceiling { get; set; } = "";           // empty = normal, or "Pit" or "HiddenPit"
        public string DoorObjectKey { get; set; } = "";     // 1-2-2 (Level-X-Y)
        public string Floor { get; set; } = "";             // empty = normal, or "Pit" or "HiddenPit" or "Slime" or "Puddle"
        public string FloorObjectKey { get; set; } = "";    // 1-2-2 (Level-X-Y)
        public Point[] FloorRemoteCells { get; set; }       // array of remote cell coords this cell can operate on
        public bool IsStairs { get; set; } = false;         // Is this cell a staircase
        public Point StairsToCell { get; set; }             // Point coords of staicase target
        public string StairsToLevel { get; set; } = "";     // Level stairs lead to
        public bool IsTraversable { get; set; } = true;     // Is this a wall or door/ open ground
        #endregion
        #region static class variables and properties
        /// <summary>
        /// Use static properties here so direction is set at class level for all objects
        /// The direction is opposite to the player, so images can be correctly accessed and drawn
        /// If a wall is facing S and player facing N, the cells on the left should display the E image
        /// and the cells on the right should display the W image
        /// </summary>
        private static string facing        = "S";
        private static string playerFacing  = "N";
        private static string left          = "E";
        private static string right         = "W";
        public  static string PlayerFacing
        {
            get { return playerFacing; }
            set
            {
                playerFacing = value;
                if (value == "N")
                {
                    facing = "S";
                    left = "E";
                    right = "W";
                }
                else if (value == "E")
                {
                    facing = "W";
                    left = "S";
                    right = "N";
                }
                else if (value == "S")
                {
                    facing = "N";
                    left = "W";
                    right = "E";
                }
                else if (value == "W")
                {
                    facing = "E";
                    left = "N";
                    right = "S";
                }
            }
        }
        public static string Facing
        {
            get { return facing; }
            private set { facing = value; }
        }
        #endregion
        private string[] directions = { "N", "E", "S", "W" };   // keys used for a number of properties
        #region Constructor
        public Cell(int x, int y)
        {
            X = x;
            Y = y;
        }
        #endregion
        public List<string> GetData()
        {
            /// Return information from Cell properties as a List
            /// Used in Console to display data of cell contents / floor / ceiling / door
            List<string> data = new List<string>();
            data.Add($"Current Cell: [{X}, {Y}], Facing {PlayerFacing}, Looking at the {Facing} sides of walls");
            /// Walls
            string wallType = "";
            foreach (string direction in directions)
            {
                if (Walls[direction] != "")
                    wallType += $"{direction}:{Walls[direction]}, ";
            }
            if (wallType != "")
                data.Add($"Walls: {wallType}");
            /// FloorActuators / Floors
            if (FloorObjectKey != "")
            {
                Floor floor = Shared.Floors[FloorObjectKey];
                data.Add($"FloorActuator -> Target {floor.Target}, Toggled: {floor.IsToggled}, Visible: {floor.IsVisible}, PartySize: {floor.PartySize}");
            }
            else if (Floor != "")
                data.Add($"Floor: {Floor}");
            /// Ceilings
            if (Ceiling != "")
                data.Add($"Ceiling Type: {Ceiling}");
            if (DoorObjectKey != "")
            {
                Door door = Shared.Doors[DoorObjectKey];
                data.Add($"Door: {door.DoorType}, OpenState: {door.OpenState}, Breakable: {door.IsBreakable}, Toggled {door.IsToggled}");
                data.Add($"Door Control: {door.Control}, Control Location: [{door.ControlLocation.X}, {door.ControlLocation.Y}]");
            }
            /// FloorItems
            string floorItems = "";
            foreach (KeyValuePair<string, string> item in FloorItems)
            {
                if (item.Value != "")
                    floorItems += $"{item.Key}:{item.Value}, ";
            }
            if (floorItems != "")
                data.Add($"FloorItems: {floorItems}");

            ///Stairs
            if (IsStairs)
                data.Add($"Stairs To Level: {StairsToLevel}, Target Cell coordinates: {StairsToCell.X}, {StairsToCell.Y}");
            
            /// Wall Decorations
            string wallDecorations = "";
            foreach (KeyValuePair<string, string> item in WallDecorations)
            {
                if (item.Value != "") wallDecorations += $"{item.Key}:{item.Value}, ";
            }
            if (wallDecorations != "") data.Add($"WallDecorations: {wallDecorations}");
            /// Wall Actuators
            string wallActuators = "";
            foreach (KeyValuePair<string, string> item in WallActuators)
            {
                if (item.Value != "") wallActuators += $"{item.Key}:{item.Value}, ";
            }
            if (wallActuators != "") data.Add($"WallActuators: {wallActuators}");

            /// Champions
            foreach (KeyValuePair<string, string> item in Champions)
            {
                if (item.Value != "")
                    data.Add($"Champion: {item.Value}");
            }
            /// WallWriting
            foreach (KeyValuePair<string, string> item in WallWritingKeys)
            {
                if (item.Value != "")
                {
                    WallWriting writing = Shared.WallWritings[item.Value];
                    if(writing.Messages.Count > 0)
                    {
                        foreach(string message in writing.Messages)
                        {
                            data.Add(message);
                        }
                    }
                }
            }
            if (data.Count < 10)
            {
                for (int i = data.Count; i < 10; i++)
                {
                    data.Add("");
                }
            }
            return data;
        }
        public string GetRightSide()
        {
            return right;
        }
        public string GetLeftSide()
        {
            return left;
        }
        public List<string> GetWallData(string side) // cellLeft.GetLeftFacing(), CurrentLevel, pointLeft.X, pointLeft.Y
        {
            /// return a list of { X, Y, Wall/Empty, WallDecorations }
            /// Used in Console to display walls/ writing / champions from cell in front, to the left / right
            List<string> data = new List<string>{ $"[{X}, {Y}]"};

            if (Walls[side] == "")
                data.Add("Empty Cell");
            else
            {
                data.Add(Walls[side]);
                if (WallDecorations[side] != "")
                    data.Add(WallDecorations[side]);
                if (AlcoveItems[side] != "")
                {
                    string text = AlcoveItems[side];
                    while (text.Length > 26)
                    {
                        data.Add(text.Substring(0, 26));
                        text = text.Substring(26);
                    }
                    if(text.Length> 0)
                        data.Add(text);
                }
            }
            if (WallWritingKeys[side] != "")
            {
                WallWriting writing = Shared.WallWritings[WallWritingKeys[side]];
                data[data.Count - 1] += " Writing:";
                foreach (string message in writing.Messages)
                    data.Add(message);
            }
            if (Champions[side] != "")
                data.Add($"Champion: {Champions[side]}");

            if(data.Count < 6)
            {
                for(int i = data.Count; i < 6; i++)
                    data.Add("");
            }

            return data;
        }
    }
}
