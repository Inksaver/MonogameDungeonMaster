using System.Drawing;
using System.Xml.Linq;

namespace XMLDungeonReader
{
    internal static class Shared
    {
        public static Cell[,]? Cells;                                                        // 2D Array of Cell objects
        public static Point CurrentCellCoords = new Point(0, 0);
        public static int MapWidth = 0;
        public static int MapHeight = 0;
        private static string AppPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        private static int CurrentLevel = 0;
        private static int mapGridSizeX = 16;
        private static int mapGridSizeY = 16;
        private static string[] facings = { "N", "E", "S", "W" };
        public static Dictionary<string, string> map = new Dictionary<string, string>();    // dictionary based on TiledToInfo.txt map index->wall type(s)
        public static Dictionary<string, Door> Doors = new Dictionary<string, Door>();
        public static Dictionary<string, Floor> Floors = new Dictionary<string, Floor>();
        public static Dictionary<string, Champion> Champions = new Dictionary<string, Champion>();
        public static Dictionary<string, Item> Items = new Dictionary<string, Item>();
        public static Dictionary<string, WallWriting> WallWritings = new Dictionary<string, WallWriting>();

        private static string[] ConvertNullToString(string data)
        {
            /// Properies from Tiled could have "Null";"Wall";Null";"Wall" change "Null" to ""
            string[] imageTypes = data.Split(";", StringSplitOptions.RemoveEmptyEntries);
            for (int index = 0; index < imageTypes.Length; index++)
            {
                if (imageTypes[index] == "Null")
                    imageTypes[index] = "";             // Replace "Null" with ""
            }
            return imageTypes;  // "";"Wall";"";"Wall"
        }
        public static string[] DecodeData(string data, string separator) //  eg Swing:2:0
        {
            return data.Split(separator, StringSplitOptions.RemoveEmptyEntries); // eg {"Swing", "2", "0" }
        }
        private static string FormatProperty(string original, string newItem)
        {
            /// eg WallDecorationsN contains "Slime", add "Hook"
            string retValue = original;         // either "" or some string
            if (newItem != "")                  // add more text
                if (original == "")              // currently empty
                    retValue = newItem;         // Slime
                else
                    retValue += $";{newItem}";  // Slime;Hook

            return retValue;
        }
        public static void GetLevelData(string level)
        {
            // get a Dictionary of all the tiles used for Doors, Walls, FloorDecorations
            /*
            15=Wall:Alcove;Alcove;Wall;Alcove
            16=Wall:Alcove;Alcove;Alcove;Alcove
            17=Wall:Invisible;Invisible;Invisible;Invisible
            18=Wall:Removeable;Removeable;Removeable;Removeable
            19=Wall:Pillar;Pillar;Pillar;Pillar
            20=Stairs:Null;Null;StairsUp;Null
             */
            string[] lines = File.ReadAllLines(Path.Combine(AppPath, "Data", "TiledToItem.txt"));
            foreach (string line in lines)
            {
                string[] data = line.Split('=');    // eg 1=Wall;Wall;Wall;Wall -> {"1", "Wall;Wall;Wall;Wall"}
                map.Add(data[0], data[1]);          // map["1"] =  "Wall;Wall;Wall;Wall"
            }

            XElement doc = XElement.Load(Path.Combine(AppPath, "Data", level));     // Load .tmx file
            string sWidth = doc.Attribute("width").Value;                           // get width of map -> width="20"
            string sHeight = doc.Attribute("height").Value;                         // get height of map -> height="19"
            string sGridSizeX = doc.Attribute("tilewidth").Value;                   // get the width of each tile on the map
            string sGridSizeY = doc.Attribute("tileheight").Value;                  // get the height of each tile on the map
            int.TryParse(sWidth, out MapWidth);                                     // set variables from string values
            int.TryParse(sHeight, out MapHeight);                                   // set variables from string values
            int.TryParse(sGridSizeX, out mapGridSizeX);                             // set module variables from string values
            int.TryParse(sGridSizeY, out mapGridSizeY);                             // set module variables from string values
            Cells = new Cell[MapWidth, MapHeight];                                  // define array size eg 20 wide, 19 high
            for (int col = 0; col < Cells.GetLength(0); col++)                      // populate array with Cell objects
            {
                for (int row = 0; row < Cells.GetLength(1); row++)
                {
                    Cells[col, row] = new Cell(col, row);
                }
            }

            // create iterable of all "layer" items using xml.linq
            IEnumerable<string> query = from item in doc.Descendants("layer")
                                        select item.Attribute("name") + item.Value;

            /*
             Only 2 descendants "layer"
             <layer id="1" name="Walls" width="20" height="19">
             <layer id="7" name="FloorDecorations" width="20" height="19" visible="0">
            All data between <layer ...> and </layer> is collected
            [0] "name=\"Walls\"\n1,1,1->1,\n1,1,1,1,1,1,1,1,0 -> to end of 19x19 grid
            [1] "name=\"FloorDecorations\"\n0,0,0,0 ->0,\n0,0,0,0,0,0,56,0 -> to end of 19x19 grid
            */
            foreach (string layer in query)                                         // iterate layers to extract image information
            {
                lines = layer.Split("\n", StringSplitOptions.RemoveEmptyEntries);   // re-use lines[]
                /*
                [0] "name=\"Walls\""
                [1] "1,1,1,1,1,1,1,1,1,1,1,1 -> "
                [2] "1,1,1,1,1,1,1,0,1,1,0,1 -> "
                */
                string name = lines[0].Substring(6).Replace("\"", "");              // clean lines[0] -> name="Walls" -> Walls
                ProcessData(name, lines);
            }
            GetTiledObjects("Doors", doc);
            GetTiledObjects("WallDecorations", doc);
            GetTiledObjects("WallActuators", doc);
            GetTiledObjects("Champions", doc);
            GetTiledObjects("WallWriting", doc);
            GetTiledObjects("AlcoveItems", doc);
            GetTiledObjects("FloorItems", doc);
            GetTiledObjects("FloorActuators", doc);
            GetTiledObjects("Stairs", doc);
            GetTiledObjects("Ceilings", doc);
        }
        private static Point GetPoint(string data)
        {
            Point retValue = new Point(0, 0);
            string[] cellcoord = data.Split(',');              // eg "2,3" = "2","3"
            if (int.TryParse(cellcoord[0], out int cellX))      // eg "2" -> cellX = 2
            {
                if (int.TryParse(cellcoord[1], out int cellY))  // eg "3" -> cellY = 3
                {
                    retValue.X = cellX;
                    retValue.Y = cellY;
                }
            }
            return retValue;
        }
        private static Point[] GetPoints(string data)
        {
            /// Create a list of points from a string eg "2,3;5,7"
            List<Point> lstPoints = new List<Point>();
            string[] points;
            if (data.Contains(";")) // multiple points
            {
                points = data.Split(";", StringSplitOptions.RemoveEmptyEntries); // eg "2,3;5,7" -> "2,3", "5,7"
            }
            else
                points = new string[] { data };                     // only one Point eg "2,3"

            foreach (string point in points)                        // convert each string equivalent of a point into a Point
            {
                string[] cellcoord = point.Split(',');              // eg "2,3" = "2","3"
                if (int.TryParse(cellcoord[0], out int cellX))      // eg "2" -> cellX = 2
                {
                    if (int.TryParse(cellcoord[1], out int cellY))  // eg "3" -> cellY = 3
                        lstPoints.Add(new Point(cellX, cellY));
                }
            }
            return lstPoints.ToArray();                             // Convert list to array
        }
        private static void GetTiledObjects(string objectName, XElement doc)
        {
            /// Get objectgroup objectName and their properties

            // general query
            var objectQuery = from item in doc.Descendants("objectgroup")
                              where item.Attribute("name").Value == objectName
                              select item.Descendants("object");

            // check if <properties> </properties> are available
            var subquery = from item in objectQuery
                           where item.Descendants("properties").Descendants("property").Count() > 0
                           select item.Descendants("properties").Descendants("property");

            if (subquery.Count() == 0)
            {
                /*
                Example of objectgroup without properties eg "Champions"
                <objectgroup id="10" name="Champions" visible="0">
                    <object id="25" name="C00" gid="43" x="176" y="64" width="16" height="16"/>
                </objectgroup>
                */
                if (objectName == "Champions")
                {
                    //int idx = 0;
                    foreach (var item in objectQuery)
                    {
                        foreach (var subitem in item)
                        {
                            string gid = "";
                            int x = -1;
                            int y = -1;
                            string id = "";

                            foreach (var attribute in subitem.Attributes())
                            {
                                if (attribute.Name == "gid") gid = attribute.Value;
                                if (attribute.Name == "x")
                                {
                                    if (int.TryParse(attribute.Value, out x)) x = x / mapGridSizeX;
                                }
                                if (attribute.Name == "y")
                                {
                                    y = Convert.ToInt32(attribute.Value);
                                    if (int.TryParse(attribute.Value, out y)) y = (y - mapGridSizeY) / mapGridSizeY;
                                }
                                if (attribute.Name == "name") id = attribute.Value;
                            }
                            // tile type is "Mirror" for all champions. If not chosen image is drawn on top
                            // tile gid gives location of mirror: 41="N",42="E",43="S",44="W"
                            Champion champ = Champions.FirstOrDefault(kvp => kvp.Value.ID == id).Value;
                            string name = champ.Name;
                            if (gid == "41") Cells[x, y].Champions["N"] = name; // eg "Alex"
                            else if (gid == "42") Cells[x, y].Champions["E"] = name;
                            else if (gid == "43") Cells[x, y].Champions["S"] = name;
                            else if (gid == "44") Cells[x, y].Champions["W"] = name;
                        }
                    }
                }
            }
            else
            {
                objectQuery = from item in doc.Descendants("objectgroup")
                              where item.Attribute("name").Value == objectName              // eg DoorFrames
                              select item.Descendants("object").Descendants("properties");
                //SetInfoFromGID(objectName, gid, colX, rowY);    // set DoorName, Frame etc
                foreach (var obj in objectQuery)
                {
                    foreach (var prop in obj)
                    {
                        var newquery = from item in prop.Descendants("property") select item;
                        string name = prop.Parent.Attribute("name").ToString().Substring(6).Replace("\"", "");      // clean property -> name="Door01" -> Door01
                        string gid = prop.Parent.Attribute("gid").ToString().Substring(5).Replace("\"", "");        // clean property -> gid="62" -> "62"
                        string objClass = "";
                        if (prop.Parent.Attribute("class") != null)
                            objClass = prop.Parent.Attribute("class").ToString().Substring(7).Replace("\"", "");    // clean property -> class="Decorations" -> Decorations
                        string x = prop.Parent.Attribute("x").ToString().Substring(3).Replace("\"", "");            // clean property -> x="61" -> 61
                        string y = prop.Parent.Attribute("y").ToString().Substring(3).Replace("\"", "");            // clean property -> y="32" -> 32
                        if (int.TryParse(x, out int colX)) colX = colX / mapGridSizeX;
                        if (int.TryParse(y, out int rowY)) rowY = (rowY - mapGridSizeY) / mapGridSizeY;
                        //SetInfoFromGID(objectName, name, gid, colX, rowY);    // set DoorName, Frame etc
                        string[] dataItems = SetInfoFromGID(objectName, name, gid, colX, rowY);    // set DoorName, Frame etc
                        //Console.WriteLine(name);
                        //Console.WriteLine(name);
                        foreach (var val in newquery) // Iterate all properties
                        {
                            string propName = val.Attribute("name").Value;
                            string propValue = val.Attribute("value").Value;
                            switch (objectName)
                            {
                                case ("Doors"):
                                    // Tiled provides these properties:
                                    // Control: "", Button, Key, Remote
                                    // ControlLocation: "", 7,9
                                    // Frame: Frame, FrameButton, FrameKey
                                    // IsBreakable: true / false
                                    // IsToggled: true / false
                                    // OpenState: 0-1 0 = fully closed, 1 = fully open

                                    {   
                                        Door temp = Doors[$"{CurrentLevel}-{colX}-{rowY}"];
                                        // Cell[].Doors already given "Doortrack" or Frame and Door type
                                        switch (propName)
                                        {
                                            case ("Control"): temp.Control = propValue; break;
                                            case ("ControlLocation"): temp.ControlLocation = GetPoint(propValue); break; //[-1,-1] given for code opening eg start of game
                                            case ("OpenState"):
                                                {
                                                    temp.OpenState = Convert.ToSingle(propValue);
                                                    if (temp.OpenState == 1) // door is closed
                                                        Cells[colX, rowY].IsTraversable = false;
                                                    break;
                                                }
                                            case ("IsBreakable"): temp.IsBreakable = Convert.ToBoolean(propValue); break;
                                            case ("IsToggled"): temp.IsToggled = Convert.ToBoolean(propValue); break;
                                            case ("Frame"): temp.Frame = propValue; break;
                                        }
                                        break;
                                    }
                                case ("WallDecorations"):   // Wallook, Wallring, Wallslime, manacles, Wallgrate -> Non active items
                                    {
                                        // eg propName = "E", propValue = "Manacles"  : WallDecorations["E"] = "Manacles" or "SconceT:15"
                                        if (propValue != "") // eg "SconceT:15"
                                            Cells[colX, rowY].WallDecorations[propName] = FormatProperty(Cells[colX, rowY].WallDecorations[propName], propValue);
                                        break;
                                    }
                                case ("WallActuators"): //Champions/Daroou , Fireball/Fireball1, SconceT:15
                                    {

                                        // eg button, coinslot, sconce etc
                                        if (propValue != "")
                                        {
                                            Cells[colX, rowY].WallActuators[propName] = propValue;  // eg SmallButton
                                        }
                                        break;
                                    }
                                case ("WallWriting"):
                                    {
                                        // WallWriting objects already created in SetInfoFromGID
                                        // dataItems = {"", "Text", "", "" }
                                        // now add the writing text to the objects
                                        for (int i = 0; i < facings.Length; i++)
                                        {
                                            if (propName == facings[i] && dataItems[i] != "")
                                            {
                                                WallWriting temp = WallWritings[$"{CurrentLevel}-{colX}-{rowY}-{facings[i]}"];
                                                temp.ProcessMessages(propValue); // eg "This is line 1;This is line 2;This is line 3;this is line 4"
                                                                                 //Cells[colX, rowY].WallWritingKeys[propName] = propValue;
                                                                                 //Cells[colX, rowY].Walls[propName] = "Writing";
                                            }
                                        }
                                        break;
                                    }
                                case ("AlcoveItems"): // can be applied to Altar or Alcove
                                    {
                                        Cells[colX, rowY].AlcoveItems[propName] = FormatProperty(Cells[colX, rowY].AlcoveItems[propName], propValue);
                                        break;
                                    }
                                case ("FloorItems"):
                                    {
                                        Cells[colX, rowY].FloorItems[propName] = FormatProperty(Cells[colX, rowY].FloorItems[propName], propValue);
                                        break;
                                    }
                                case ("FloorActuators"):
                                    {
                                        /// SetInfoFromGID has already created a Floor object for this floorplate
                                        /// This fills the properties of the object

                                        Floor temp = Floors[$"{CurrentLevel}-{colX}-{rowY}"];
                                        switch (propName)
                                        {
                                            case ("Action"): temp.Action = propValue; break;
                                            case ("IsVisible"):
                                                {
                                                    temp.IsVisible = Convert.ToBoolean(propValue);
                                                    //Cells[colX, rowY].FloorIsVisible = Convert.ToBoolean(propValue);
                                                    break;
                                                }
                                            case ("IsToggled"): temp.IsToggled = Convert.ToBoolean(propValue); break;
                                            case ("ObjectRequired"): temp.ObjectRequired = propValue; break;
                                            case ("PartySize"): temp.PartySize = Convert.ToInt32(propValue); break;
                                            case ("Target"): temp.Target = GetPoints(propValue)[0]; break;
                                        }
                                        break;
                                    }
                                case ("Stairs"):
                                    {
                                        //SetInfoFromGID("Stairs", gid, colX, rowY);    // get image set for stairs
                                        Cells[colX, rowY].IsStairs = true;
                                        switch (propName)
                                        {
                                            case ("TargetLevel"): Cells[colX, rowY].StairsToLevel = propValue; break;
                                            case ("TargetCell"):
                                                {
                                                    Cells[colX, rowY].StairsToCell = GetPoints(propValue)[0];
                                                    break;
                                                }
                                        }
                                        break;
                                    }
                                case ("Ceilings"):
                                    {
                                        // Tiled provides these properties:
                                        // FromCoordinate value = "-1,-1"
                                        // FromLevel value = 0, 1 etc
                                        //if(propName == "FromCoordinate")
                                        if (propValue != "")
                                            Cells[colX, rowY].Ceiling = "CPit";
                                    }
                                    break;
                            }
                        }
                    }
                }
            }
        }
        private static void ProcessData(string name, string[] lines)
        {
            /*
            layer name eg Walls, FloorDecorations
            name="Walls"
            string[] lines =
            1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,
            1,1,1,1,1,1,1,0,1,1,0,1,1,1,1,1,1,1,1,1,
            1,1,0,1,1,0,0,0,1,1,1,1,1,1,1,1,1,1,1,1,
            1,1,0,1,1,0,1,0,0,0,1,1,1,1,0,0,0,1,1,1,
            1,1,0,1,1,0,1,0,1,0,0,0,0,0,0,0,0,1,1,1,
            1,0,0,1,1,0,1,1,1,1,0,0,0,1,0,0,0,1,1,1,
            1,1,0,1,1,0,1,1,1,1,0,1,0,1,0,0,0,0,0,1,
            1,1,0,0,0,0,1,1,1,1,0,0,0,1,1,1,0,1,1,1,
            1,1,1,1,1,1,1,1,1,1,0,1,1,1,1,1,0,0,0,1,
            1,1,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,
            1,1,1,1,1,0,1,1,0,1,1,0,0,0,1,0,1,0,1,1,
            1,1,0,0,1,0,0,1,0,1,1,1,1,1,1,0,1,0,1,1,
            1,1,0,0,1,0,1,0,0,0,0,1,1,1,0,0,1,0,1,1,
            1,1,0,0,1,0,1,0,0,0,0,1,1,0,0,0,1,0,1,1,
            1,1,0,1,1,0,1,0,1,1,0,1,0,0,0,1,0,0,0,1,
            1,1,0,1,1,0,1,0,0,0,0,0,0,0,1,1,0,1,0,1,
            1,0,0,1,1,0,1,0,0,0,0,1,1,1,1,1,0,1,0,1,
            1,1,0,0,0,0,1,1,1,1,1,1,1,1,1,1,0,0,0,1,
            1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1
            */
            for (int rowY = 0; rowY < lines.Length - 1; rowY++)
            {
                string currentRow = lines[rowY + 1];                        // +1 as lines[0] is name="Xxxx", lines[1] = 1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,
                string[] xCols = currentRow.Split(",", StringSplitOptions.RemoveEmptyEntries);   //
                for (int colX = 0; colX < xCols.Length - 1; colX++)         // rowY is outer loop, colX is inner loop -> cells[xCol, yRow] -> [0,0], [1,0], [2,0]
                {
                    string cellType = xCols[colX];
                    if (cellType != "0")
                    {
                        if (map.TryGetValue(cellType, out string info))
                        {
                            string[] infos = info.Split(':');                   // eg infos[0] = "Wall", infos[1] = "Wall;Wall;Wall;Wall"
                            string[] dataTypes = ConvertNullToString(infos[1]); // used if data consists of 4 items separated with ; 
                            if (name == "Walls" && infos[0] == "Wall")          // reading data from Walls layer and only considering conversions starting with "Wall"
                            {

                                for (int i = 0; i < facings.Length; i++)
                                {
                                    Cells[colX, rowY].Walls[facings[i]] = dataTypes[i]; // eg cells[y, x].Walls["N"] = Wall
                                }
                            }
                            //else if (name == "FloorDecorations" && infos[0] == "FloorDecoration")
                            else if (name == "FloorDecorations")
                            {
                                // pit, hidden pit, slime and puddles
                                // eg infos[0] = FloorDecoration, infos[1] = Slime
                                // all current images are Cell.Facing independant
                                Cells[colX, rowY].Floor = $"{dataTypes[0]}";
                            }
                        }
                        else
                        {
                            // key not found in map
                            throw new Exception($"gid key {cellType} not found");
                        }
                    }
                }
            }
        }
        private static string[] SetInfoFromGID(string type, string name, string gid, int colX, int rowY)
        {
            // GetLevelData -> GetTiledObjects -> SetInfoFromGID at an early stage so THIS function creates Door, WallWriting etc objects
            // Return the string found in the map Dictionary eg "Doortrack";"Grate";"Doortrack";"Grate" or "","Text","",""
            // eg type = "Stairs", name = "Stairs01", gid = "27"
            string[] dataItems = { };                                           // Empty sting array
            if (map.TryGetValue(gid, out string info))                          // dictionary based on TiledToItem.txt map index->wall type(s)
            {
                string[] infos = DecodeData(info, ":");                         // eg infos[0] = "Wall", infos[1] = "Wall;Wall;Wall;Wall"
                dataItems = ConvertNullToString(infos[1]);                      // used if data consists of 4 items separated with ; 
                if (type == "Walls")
                {
                    for (int i = 0; i < facings.Length; i++)                    // facings = { "N", "E", "S", "W" }
                    {
                        Cells[colX, rowY].Walls[facings[i]] = dataItems[i];    // eg cells[y, x].Walls["N"] = Wall
                    }
                }
                else if (type == "Doors")                                       // eg infos[0] = "Wall", infos[1] = "Wall;Wall;Wall;Wall"
                {
                    string key = $"{CurrentLevel}-{colX}-{rowY}";
                    Door temp;
                    string doorType = dataItems[0];                             // eg "Doortrack";"Grate";"Doortrack";"Grate"
                    if (doorType == "Doortrack") doorType = dataItems[1];
                    if (!Doors.TryGetValue(key, out temp))
                    {
                        temp = new Door(key, name, doorType, colX, rowY);
                        Doors.Add(key, temp);
                        Cells[colX, rowY].DoorObjectKey = key;
                    }
                    for (int i = 0; i < facings.Length; i++)
                    {
                        Cells[colX, rowY].Doors[facings[i]] = dataItems[i];    // eg cells[y, x].Doors["E"] = "Wood"
                    }
                }
                else if (type == "WallActuators") { }
                else if (type == "WallWriting")
                {
                    /* .tmx Tiled file
                    45=WallWriting:Text;Null;Null;Null (N side)
                    46=WallWriting:Null;Text;Null;Null (E side)
                    47=WallWriting:Null;Null;Text;Null (S side)
                    48=WallWriting:Null;Null;Null;Text (W side)
                    WallWriting objects are added to the Dictionary WallWritings
                    The information from Tiled .tmx file indicates the prescence of Text
                    GetTiledObject() in addition to creating these objects
                    */
                    for (int i = 0; i < dataItems.Length; i++)
                    {
                        if (dataItems[i] != "") // "Text" found here
                        {
                            string key = $"{CurrentLevel}-{colX}-{rowY}-{facings[i]}";
                            WallWritings.Add(key, new WallWriting(key));
                            Cells[colX, rowY].WallWritingKeys[facings[i]] = key;
                        }
                    }
                }
                else if (type == "Stairs")                              // 27 = Stairs:StairsDownRail; StairsDown; StairsDownRail; Null
                {
                    //imageTypes =  {"StairsUp", "", "", ""} Stairs
                    /*
                     <objectgroup id="21" name="Stairs" visible="0">
                      <object id="77" name="Stairs01" gid="27" x="64" y="256" width="16" height="16">
                       <properties>
                        <property name="TargetCell" value="4,15"/>
                        <property name="TargetLevel" value="02"/>
                       </properties>
                      </object>
                     </objectgroup>
                     */

                    for (int i = 0; i < facings.Length; i++)
                    {
                        Cells[colX, rowY].Walls[facings[i]] = dataItems[i];    // eg cells[y, x].Walls["N"] = Wall
                    }
                }
            }
            else
            {
                // not in map dictionary eg FloorActuator
                if (type == "FloorActuators" && gid == "32") //Floor plate
                {
                    string key = $"{CurrentLevel}-{colX}-{rowY}";
                    Floor temp;
                    if (!Floors.TryGetValue(key, out temp))
                    {
                        temp = new Floor(key, colX, rowY);
                        Floors.Add(key, temp);
                        Cells[colX, rowY].FloorObjectKey = key;
                    }
                }
            }
            return dataItems;
        }
        public static void LoadChampions()
        {
            /// Create 24 Champion objects from the text file Champions.txt
            string key = "";
            List<string> lines = ProcessTextFile("Data/Champions.txt");                 // remove comments and blank lines
            foreach (string line in lines)                                              // Fighter=Apprentice;(Swing: 2, Thrust: 3, Club: 0, Parry: 2)
            {
                string[] data = DecodeData(line, "=");                                  // "Fighter", "Apprentice;(Swing: 2, Thrust: 3, Club: 0, Parry: 2)"
                if (data.Length > 1)                                                    // non-empty eg Name=Hawk
                {
                    if (data[0] == "Name")
                    {
                        key = data[1];                                                  // eg Hawk
                        Champions.Add(key, new Champion(key));
                    }
                    else
                    {
                        if (data.Length > 1) // data is present
                        {
                            if (data[0].ToLower() == "id") Champions[key].ID = data[1]; // Champion ID kept here. Should be unique
                            else if (data[0].ToLower() == "title") Champions[key].Title = data[1];
                            else if (data[0] == "Fighter" || data[0] == "Ninja" || data[0] == "Priest" || data[0] == "Wizard")
                            {
                                string[] levelData = DecodeData(data[1], ";");    //  "Apprentice", "Swing:2", "Thrust:3", "Club:0"; "Parry:2"
                                foreach (string level in levelData) // {Apprentice, Swing:2, Thrust:3
                                {
                                    if (level.Contains(":"))
                                    {
                                        string[] levelDecoded = DecodeData(level, ":");
                                        Champions[key].Stats[levelDecoded[0]] = Convert.ToInt32(levelDecoded[1]);
                                    }
                                    else
                                    {
                                        Champions[key].Skills[data[0]] = levelData[0]; // Champions[0].Skills["Fighter"] = "Apprentice"
                                    }
                                }
                            }
                            else if (data[0] == "Items")
                            {
                                /// Items=Torso:Leather Jerkin;Legs:Leather Pants;Feet:Leather Boots;Hand:Torch:14

                                string[] items = DecodeData(data[1], ";");     // Torso:Leather Jerkin;Legs:Leather Pants;Hand:Torch:14
                                foreach (string item in items)                 // Torso:Leather Jerkin or Hand:Torch:14 Could be 1 entry only
                                {
                                    string[] subItems = DecodeData(item, ":"); // eg {Torso, Leather Jerkin} or {Hand, Torch, 14}
                                    // subItems[0] is location of item eg Torso or Hand
                                    // subItems[1] is item eg Torch
                                    if (Items.ContainsKey(subItems[1]))
                                    {
                                        if (subItems.Length > 2)                            // eg {Hand, Torch, 14}
                                            subItems[1] = $"{subItems[1]}:{subItems[2]}";   // eg Torch:14                                        

                                        Champions[key].Items[subItems[0]][0] = subItems[1]; // eg Champions["Hawk"]Items["Hand"][0] = "Torch:14"
                                    }
                                    else
                                        throw new Exception($"The item {subItems[1]} is not present in the Dictionary.\nCheck Items.txt and Champions.txt for correct spelling");
                                }
                            }
                            else if ((data[0] == "Pouch" || data[0] == "Quiver" || data[0] == "Inventory") && data.Length > 1) // Quiver=0:Arrow,2:Arrow
                            {
                                // data[0] = location of item (Pouch, Quiver or Inventory
                                // data[1] already confirmed as present eg Quiver=0:Arrow,2:Arrow

                                string[] items = DecodeData(data[1], ";");      //0:Arrow;2:Arrow or 0:Torch:7;2:Torch:1
                                foreach (string item in items)                  // 0:Arrow or 0:Torch:7
                                {
                                    string[] subItems = DecodeData(item, ":");  // 0:Torch:7 = {0, Torch, 7}
                                    // subItems[0] = 0 or other index
                                    int.TryParse(subItems[0], out int position);
                                    if (subItems.Length > 2)                                // eg {0, Torch, 7}
                                        subItems[1] = $"{subItems[1]}:{subItems[2]}";       // eg Torch:14                                        

                                    Champions[key].Items[data[0]][position] = subItems[1];  // eg Champions["Hawk"]Items["Inventory"][0] = "Torch:14"
                                }
                            }
                            else if (data[0] == "Load")
                            {
                                if (int.TryParse(data[1], out int load))
                                    Champions[key].MaxLoad = load;
                            }
                            else // All other attributes
                            {
                                if (int.TryParse(data[1], out int value))
                                    Champions[key].Stats[data[0]] = value; // Champions[0].Skills["Health"] = 60
                            }
                        }
                    }
                }
            }
        }
        public static void LoadItems()
        {
            List<string> lines = ProcessTextFile("Data/Items.txt");
            foreach (string line in lines) // Moonstone=Weight:0.2;Mana:3;Influence:1;Active:Neck
            {
                string[] kvp = DecodeData(line, "=");      // Weight:0.2;Mana:3;Influence:1;Active:Neck
                string key = kvp[0];                       // Moonstone
                string[] data = DecodeData(kvp[1], ";");   // Weight:0.2 Mana:3 Influence:1 Active:Neck
                                                           // allow for multiple items of same type eg "Torch"

                if (Items.TryGetValue(key, out Item item))
                {
                    // "Torch" already exists so check for Torch01, Torch02 etc
                    int index = 1;
                    key = $"{kvp[0]}{index:D2}";        // Torch01 max Torch99
                    while (Items.TryGetValue(key, out item))
                    {
                        index++;
                        key = $"{kvp[0]}{index:D2}";
                    }
                }
                Items.Add(key, new Item(kvp[0]));
                // Item object created, now update it
                for (int i = 0; i < data.Length; i++)
                {
                    string[] properties = DecodeData(data[i], ":");  // eg {"Swing", "2", "0" }
                    if (Items[key].Properties.ContainsKey(properties[0]))
                    {
                        if (properties.Length == 2)
                            Items[key].Properties[properties[0]] = properties[1];
                        else
                            Items[key].Properties[properties[0]] = $"{properties[1]}:{properties[2]}";  // eg Items["Swing"] = "2:0" }
                    }
                    else
                    {
                        if (properties[0] == "States") // Storm Ring: properties[0] = States, properties[1] = Empty,Full
                        {
                            string[] subProperties = DecodeData(properties[1], ","); // {Empty, Full}
                            for (int j = 0; j < subProperties.Length; j++)
                            {
                                Items[key].ImageNames.Add($"{key}:{subProperties[j]}");    // eg Storm Ring:Empty from Storm Ring + States:Empty,Full
                            }
                            Items[key].ImageName = Items[key].ImageNames[0];
                        }
                    }
                }
            }
        }
        private static List<string> ProcessTextFile(string filename)
        {
            List<string> retValue = new List<string>();

            string[] lines = File.ReadAllLines(filename);
            foreach (string line in lines)
            {
                if (!line.StartsWith('#') && line.Trim().Length > 0)
                    retValue.Add(line);
            }
            return retValue;
        }
    }
}
