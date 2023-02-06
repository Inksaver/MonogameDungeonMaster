using System.Drawing;

namespace XMLDungeonReader
{
    internal class Program
    {
        /// <summary>
        /// Help with using Linq and XML
        /// https://makolyte.com/csharp-search-xml-for-elements-and-attributes/
        /// </summary>
        static ConsoleColor Fore = ConsoleColor.White;
        static ConsoleColor Back = ConsoleColor.Black;
        static void Main(string[] args)
        {
            Console.BackgroundColor = Back;
            Console.ForegroundColor = Fore;
            Console.WindowWidth = 90;                       // Set Console width
            Console.WindowHeight = 25;                      // Set Console height
            Shared.LoadItems();
            Shared.LoadChampions();
            Shared.GetLevelData("Level00.tmx");             // read all the layers and objects from Level00.tmx
            Cell.PlayerFacing = "S";                        // Level00.tmx starts facing S on coords [2,2] All cells will be facing N (opposite of player)
            Shared.CurrentCellCoords = new Point(2, 2);
            Point[,] viewport = GetNearbyCells();           // Point provided by System.Drawing -> will be provided by Monogame when implemented there
            while (!PlayerMove.Quit)
            {
                int lines = DrawScreen(viewport);

                if (PlayerMove.Message != "")
                {
                    Console.SetCursorPosition(0, lines - 1);
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(PlayerMove.Message.PadRight(88));
                    Console.Write(new string(' ', 88));
                    Console.ForegroundColor = Fore;
                    Thread.Sleep(2000);
                    PlayerMove.Message = "";
                    Console.SetCursorPosition(0, lines - 1);
                    Instructions();
                }
                PlayerMove.ReadKeyboard();
                if (PlayerMove.Message == "")
                    viewport = GetNearbyCells();
            }
        }
        private static int DrawScreen(Point[,] viewport)
        {
            Console.Clear();
            int lines = 0;

            Point pointLeft = viewport[0, 2];           // get cell coordinates on left side
            Point pointCentre = viewport[1, 1];         // get cell coordinates in front
            Point pointRight = viewport[2, 2];          // get cell coordinates on right side

            Cell cellLeft;                              // declare Cell object for Cell to the left of player
            Cell cellAhead;                             // declare Cell object for Cell ahead of player
            Cell cellRight;                             // declare Cell object for Cell to the right of player

            // default values for text for neighbouring cells
            List<string> defaultList = new List<string> { "Outside Map Border", "", "", "", "", "" };
            List<string> leftText = defaultList;
            List<string> aheadText = defaultList;
            List<string> rightText = defaultList;
            // Point values will have -1 in one or both values if there is no cell in range
            if (pointLeft.X >= 0 && pointLeft.Y >= 0)
            {
                cellLeft = Shared.Cells[pointLeft.X, pointLeft.Y];
                leftText = cellLeft.GetWallData(cellLeft.GetLeftSide());
            }
            if (pointCentre.X >= 0 && pointCentre.Y >= 0)
            {
                cellAhead = Shared.Cells[pointCentre.X, pointCentre.Y];
                if (pointCentre.X == -1 || pointCentre.Y == -1)
                    aheadText = defaultList;
                else
                    aheadText = cellAhead.GetWallData(Cell.Facing);
            }
            if (pointRight.X >= 0 && pointRight.Y >= 0)
            {
                cellRight = Shared.Cells[pointRight.X, pointRight.Y];
                rightText = cellRight.GetWallData(cellRight.GetRightSide());
            }
            List<string> currentText = Shared.Cells[Shared.CurrentCellCoords.X, Shared.CurrentCellCoords.Y].GetData();

            //draw 3 rectangles with text from all 3 cells inside each 30 chars each inc borders
            string rectTop = "╔".PadRight(29, '═') + "╦".PadRight(30, '═') + "╦".PadRight(29, '═') + "╗";
            string rectDivide = "╠".PadRight(29, '═') + "╬".PadRight(30, '═') + "╬".PadRight(29, '═') + "╣";
            string rectBottomDivide = "╠".PadRight(29, '═') + "╩".PadRight(30, '═') + "╩".PadRight(29, '═') + "╣";
            string rectBottom = "╚".PadRight(29, '═') + "╩".PadRight(30, '═') + "╩".PadRight(29, '═') + "╝";
            string rectSolidBottom = "╚".PadRight(88, '═') + "╝";

            List<string> list = new List<string>(); //list of 6 lines (some may be empty)
            list.Add($"║ Cell to the Left:".PadRight(29) + $"║ Cell ahead:".PadRight(30) + $"║ Cell to the Right:".PadRight(29) + "║");
            list.Add($"║ {leftText[0]} {leftText[1]}".PadRight(29) + $"║ {aheadText[0]} {aheadText[1]}".PadRight(30) + $"║ {rightText[0]} {rightText[1]}".PadRight(29) + "║");
            for (int i = 2; i < 6; i++)
            {
                list.Add($"║ {leftText[i]}".PadRight(29) + $"║ {aheadText[i]}".PadRight(30) + $"║ {rightText[i]}".PadRight(29) + "║");
            }
            Console.WriteLine(rectTop);
            foreach (string line in list)
            {
                Console.WriteLine(line);
                lines++;
            }
            Console.WriteLine(rectBottomDivide);
            lines++; // 8 lines written so far
            foreach (string text in currentText)
            {
                Console.WriteLine($"║ {text}".PadRight(88) + "║");
                lines++;
            }
            Console.WriteLine(rectSolidBottom);
            lines++;
            lines += Instructions();

            return lines;   // number of lines of text written to the console
        }
        private static int Instructions()
        {
            Console.ForegroundColor = Fore;
            Console.Write("{↑, W, Num8} =");
            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            Console.Write(" Forward");
            Console.ForegroundColor = Fore;
            Console.Write(", {↓, S, Num5} =");
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.Write(" Back");
            Console.ForegroundColor = Fore;
            Console.Write(", {←, A, Num4} = ");
            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            Console.Write(" Left");
            Console.ForegroundColor = Fore;
            Console.Write(", {→ , D, Num6} =");
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine(" Right");
            Console.ForegroundColor = Fore;
            Console.Write("{DEL, Q, 7} =");
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.Write(" Turn Left");
            Console.ForegroundColor = Fore;
            Console.Write(", {PageDown, Q, 7} =");
            Console.ForegroundColor = ConsoleColor.DarkBlue;
            Console.Write(" Turn Right");
            Console.ForegroundColor = Fore;
            Console.Write(", ESC =");
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.Write(" Quit");
            Console.ForegroundColor = Fore;
            Console.Write(". Waiting for  input >_ ");
            return 2;
        }
        private static Point[,] GetNearbyCells()
        {
            /*
            return a 3 row x 3 col array of Points of the cells around the player
            start with facing south on cell[2,2] then move S to cell[2,3]-> 
            ┌───┬───┬───┐    ┌───┬───┬───┐    ┌───┬───┬───┐    ┌─────┬─────┬─────┐
            │0,0│1,0│2,0│    │3,4│2,4│1,4│    │3,5│2,5│1,5│    │-1,-1│-1,-1│-1,-1│
            ├───┼───┼───┤    ├───┼───┼───┤    ├───┼───┼───┤    ├─────┼─────┼─────┤
            │0,1│1,1│2,1│    │3,3│2,3│1,3│    │3,4│2,4│1,4│    │-1,-1│-1,-1│-1,-1│
            ├───┼───┼───┤    ├───┼───┼───┤    ├───┼───┼───┤    ├─────┼─────┼─────┤
            │0,2│1,2│2,2│    │3,2│2,2│1,2│    │3,3│2,3│1,3│    │-1,-1│ 0, 0│ 1, 0│ = player in top left corner [0,0]
            └───┴───┴───┘    └───┴───┴───┘    └───┴───┴───┘    └─────┴─────┴─────┘
            */

            Point[,] retValue = new Point[3, 3];    // create 3,3, array of Points

            retValue[0, 0] = GetCorrectedCoordinate(-1, 2);
            retValue[1, 0] = GetCorrectedCoordinate(0, 2);
            retValue[2, 0] = GetCorrectedCoordinate(1, 2);
            retValue[0, 1] = GetCorrectedCoordinate(-1, 1);
            retValue[1, 1] = GetCorrectedCoordinate(0, 1);
            retValue[2, 1] = GetCorrectedCoordinate(1, 1);
            retValue[0, 2] = GetCorrectedCoordinate(-1, 0);
            retValue[1, 2] = GetCorrectedCoordinate(0, 0);
            retValue[2, 2] = GetCorrectedCoordinate(1, 0);

            return retValue;
        }
        private static Point GetCorrectedCoordinate(int toSide, int ahead)
        {
            /// return the coordinates of a cell toSide and ahead distances from the original toside can be -ve for left, 0 for centre +ve for right
            /// maxX and maxY are the sizes of the map - 1 to confine coordinates to 1 from border
            /// example current cell [2,2]: fromX = 2, fromY = 2, toSide = -1, ahead = 2, facing = "S"
            int maxX = Shared.Cells.GetLength(0) - 1;               // eg level 1 Shared.map 20 X 19 Cells[,] = [20, 19] -> maxX = 19
            int maxY = Shared.Cells.GetLength(1) - 1;               // eg level 1 Shared.map 20 X 19 Cells[,] = [20, 19] -> maxY = 18
            int fromX = Shared.CurrentCellCoords.X;
            int fromY = Shared.CurrentCellCoords.Y;
            int x = -1;                                             // Point index X = 0
            int y = -1;                                             // Point index Y = 0
            if (Cell.PlayerFacing == "N")           // looking up the map: x range 1 - maxX; y range maxY - 1 (ahead decreases row)
            {
                if (fromY - ahead >= 0)                             // eg 2 - 2 = 0 (facing N so ahead reduces row value)
                    y = fromY - ahead;                              // eg 2 - 2 = 0 -> y = 0
                if (fromX + toSide <= maxX && fromX + toSide >= 0)  // eg 2 + (-1) = 1
                    x = fromX + toSide;                             // eg 2 + (-1) = 1
                                                                    // [1,0]
            }
            else if (Cell.PlayerFacing == "S")      // looking down the map: x range maxX - 1; y range 1 - maxY (ahead increases row)
            {
                if (fromY + ahead <= maxY)                          // eg 2 + 2 = 4 (facing S so ahead increases row value)
                    y = fromY + ahead;                              // eg 2 + 2 = 4 -> y = 4
                if (fromX - toSide >= 0 && fromX - toSide <= maxX)  // eg 2 - (-1) = 3
                    x = fromX - toSide;                             // eg x = 2 - (-1) = 3
                                                                    // [3,4]
            }
            else if (Cell.PlayerFacing == "E")      // looking across the map to E: x range 1 - maxX; y range 1 - maxY (ahead increases col)
            {
                if (fromX + ahead <= maxX)                          // eg 2 + 2 = 4
                    x = fromX + ahead;                              // eg 2 + 2 = 4 -> x = 4
                if (fromY + toSide <= maxY && fromY + toSide >= 0)  // eg 2 + (-1) = 1
                    y = fromY + toSide;                             // eg y = 2 + (-1) = 1
                                                                    // [4,1]
            }
            else if (Cell.PlayerFacing == "W")      // looking across the map to W: x range maxX - 1; y range 1 - maxY (ahead decrease col)
            {
                if (fromX - ahead >= 0)                             // eg 2 - 2 = 0
                    x = fromX - ahead;                              // eg x unchanged -> 0
                if (fromY - toSide <= maxY && fromY - toSide >= 0)  // eg 2 - (-1) = 3
                    y = fromY - toSide;                             // eg y = 2 - (-1) = 3
                                                                    // [0,3]
            }
            return new Point(x, y);
        }
    }
}