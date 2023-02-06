using System.Collections.Generic;
using System;

namespace XMLDungeonReader
{
    internal class WallWriting
    {
        public string Name { get; set; }            // Tiled name eg "WritingA"
        public List<string> Messages { get; set; }  // eg {"This is Line1","This is Line2"}
        public WallWriting(string name)
        {
            // message eg "Hall of Champions;Line 2"
            Name = name;
            Messages = new List<string>();
        }
        public void ProcessMessages (string message)
        {
            string[] messages = message.Split(";", StringSplitOptions.RemoveEmptyEntries);
            foreach (string s in messages)
                Messages.Add(s);
        }
    }
}
