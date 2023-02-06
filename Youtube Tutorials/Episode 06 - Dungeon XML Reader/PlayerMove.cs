using System;
using System.Collections.Generic;
using System.Drawing;

namespace XMLDungeonReader
{
    /// <summary>
    /// Detects keyboard input and sets Cell.PlayerFacing direction and current cell coordinates
    /// </summary>
    internal static class PlayerMove
    {
        private static Dictionary<ConsoleKey, Dictionary<string, Point>> Moves = new Dictionary<ConsoleKey, Dictionary<string, Point>>();
        private static Dictionary<ConsoleKey, Dictionary<string, string>> Turns = new Dictionary<ConsoleKey, Dictionary<string, string>>();
        public static bool Quit = false;
        public static string Message = "";
        static PlayerMove()
        {
            /// Static class constuctor runs once only when class first referenced
            /// Populate key bindings dictionaries to WASD NumPad and Arrow keys+DEL/PageDown 
            ///
            /// Move Forward  -> UpArrow, NumPad8, W
            /// Move Backward -> DownArrow, NumPad5, S
            /// Move Left -> LeftArrow, NumPad4, A
            /// Move Right -> RightArrow, NumPad6, D
            Dictionary<string, Point> up = new Dictionary<string, Point>
            {
                {"N", new Point(0, -1) },
                {"E", new Point(1, 0) },
                {"S", new Point(0, 1) },
                {"W", new Point(-1, 0) }
            };
            Moves.Add(ConsoleKey.UpArrow, up);
            Moves.Add(ConsoleKey.NumPad8, up);
            Moves.Add(ConsoleKey.W, up);
            Dictionary<string, Point> down = new Dictionary<string, Point>
            {
                {"N", new Point(0, 1) },
                {"E", new Point(-1, 0) },
                {"S", new Point(0, -1) },
                {"W", new Point(1, 0) }
            };
            Moves.Add(ConsoleKey.DownArrow, down);
            Moves.Add(ConsoleKey.NumPad5, down);
            Moves.Add(ConsoleKey.S, down);

            Dictionary<string, Point> left = new Dictionary<string, Point>
            {
                {"N", new Point(-1, 0) },
                {"E", new Point(0, -1) },
                {"S", new Point(1, 0) },
                {"W", new Point(0, 1) }
            };
            Moves.Add(ConsoleKey.LeftArrow, left);
            Moves.Add(ConsoleKey.NumPad4, left);
            Moves.Add(ConsoleKey.A, left);

            Dictionary<string, Point> right = new Dictionary<string, Point>
            {
                {"N", new Point(1, 0) },
                {"E", new Point(0, 1) },
                {"S", new Point(-1, 0) },
                {"W", new Point(0, -1) }
            };
            Moves.Add(ConsoleKey.RightArrow, right);
            Moves.Add(ConsoleKey.NumPad6, right);
            Moves.Add(ConsoleKey.D, right);

            Dictionary<string, string> turnLeft = new Dictionary<string, string>
            {
                { "N","W" },
                { "E","N" },
                { "S","E" },
                { "W","S" }
            };
            Turns.Add(ConsoleKey.Delete, turnLeft);
            Turns.Add(ConsoleKey.NumPad7, turnLeft);
            Turns.Add(ConsoleKey.Q, turnLeft);

            Dictionary<string, string> turnRight = new Dictionary<string, string>
            {
                { "N","E" },
                { "E","S" },
                { "S","W" },
                { "W","N" }
            };
            Turns.Add(ConsoleKey.PageDown, turnRight);
            Turns.Add(ConsoleKey.NumPad9, turnRight);
            Turns.Add(ConsoleKey.E, turnRight);
        }
        public static void ReadKeyboard()
        {
            Point direction  = new Point(0, 0);
            ConsoleKeyInfo keyPressed = Console.ReadKey();
            if (keyPressed.Key == ConsoleKey.Escape)
                Quit = true;
            else if (keyPressed.Key == ConsoleKey.UpArrow   || keyPressed.Key == ConsoleKey.DownArrow ||
                     keyPressed.Key == ConsoleKey.LeftArrow || keyPressed.Key == ConsoleKey.RightArrow)
            {
                direction = Moves[keyPressed.Key][Cell.PlayerFacing];
                Message = Move(direction);
            }
            else if (keyPressed.Key == ConsoleKey.Delete || keyPressed.Key == ConsoleKey.PageDown)
            {
                Cell.PlayerFacing = Turns[keyPressed.Key][Cell.PlayerFacing];
            }
        }
        private static string Move(Point direction)
        {
            if (direction.X == 0 && direction.Y == 0)
                return "";
            Point compare = new Point(Shared.CurrentCellCoords.X + direction.X, Shared.CurrentCellCoords.Y + direction.Y);
            if ( compare.X < 0 || compare.X > Shared.MapWidth || compare.Y < 0 || compare.Y > Shared.MapHeight)
                return "You are at the edge of the map";

            Shared.CurrentCellCoords.X = compare.X;
            Shared.CurrentCellCoords.Y = compare.Y;

            return "";
        }  
    }
}
