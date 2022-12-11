using System;
using DMClone.Engine;

namespace DMClone
{
    public static class Program
    {
        /// <summary>
        /// Change these values for a screen res appropriate to your game
        /// </summary>
        private const int WIDTH = 640;
        private const int HEIGHT = 480;
        [STAThread]
        static void Main()
        {
            
            using (var game = new MainGame(WIDTH, HEIGHT, null))       // supply the first state your game should show, eg new DevState()
            {
                game.IsFixedTimeStep = true;
                game.TargetElapsedTime = TimeSpan.FromMilliseconds(1000.0f / 60);   // 60fps
                game.Run();
            }
        }
    }
}