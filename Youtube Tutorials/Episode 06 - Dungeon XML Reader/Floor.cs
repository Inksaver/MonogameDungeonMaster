using System.Drawing;

namespace XMLDungeonReader
{
    internal class Floor
    {
        public string Key { get; set; }                         // Same as Dictionary key  Level-CellX-CellY eg "1-2-2"
        public bool IsToggled { get; set; }                     // true = can be active or inactive. false = stays active once activated
        public bool IsVisible { get; set; }                     // Can player see it?
        public string Action { get; set; }                      // "Activate" eg door, Teleport, pit
        public int PartySize { get; set; }                      // 0 = empty party or placed item
        public string ObjectRequired { get; set; }              // Item placed on plate to Activate it
        public Point Target { get; set; } = new Point(-1, -1);  // cell this actuator works on
        public Point Location { get; set; }                     // Cell this is located in
        public Floor(string key, int x, int y)
        {
            Key = key;
            Location = new Point(x, y);
        }
    }
}
