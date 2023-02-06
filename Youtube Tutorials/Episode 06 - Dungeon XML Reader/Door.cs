using System.Drawing;

namespace XMLDungeonReader
{
    /// Object reference in a dictionary indexed by Level 1, CellX 2,CellY 2 -> eg 1-2-2
    /// Type:Wood, Metal etc
    /// Direction: NS / EW
    /// Open state: 0 - 1
    /// Frame type: Frame, FrameButton, FrameKey
    /// Breakable: true/false
    /// Control: Type / Location <summary>
    /// </summary>
    internal class Door 
    {
        public string Name { get; set; }            // Tiled name eg "Door01"
        public string Key { get; set; }             // Same as Dictionary key  Level-CellX-CellY eg "1-2-2"
        public string DoorType { get; set; }        // WoodW
        public string Frame { get; set; }           // Frame, FrameButton, FrameKey
        public float OpenState { get; set; }        // 0 is fully open, 1 is fully closed
        public bool IsBreakable { get; set; }       // if breakable does not have a control
        public bool IsToggled { get; set; }         // true = can be opened and closed. false = stays open
        public string Control { get; set; }         // "" / Remote / Button / Key
        public Point ControlLocation { get; set; }  // -1,-1 is default = code control or frame button/key
        public Point CellLocation { get; set; }     // Cell this is door is located in.
        public Door(string key, string name, string type, int x, int y)
        {
            // key = "0-6-9", name = "Door02", type = "Grate", 6, 9;
            Key = key;
            Name = name;
            DoorType = type;
            CellLocation = new Point(x, y);
        }
    }
}
