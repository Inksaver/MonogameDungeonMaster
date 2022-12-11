using DMClone.Engine.States;
using Microsoft.Xna.Framework;
using DMClone.Objects;

namespace DMClone.States.Swoosh
{
    internal class SwooshState : BaseGameState
    {
        /// <summary>
        /// Add references to game objects here
        /// This gameState plays a sound and a short animation
        /// Then switches state to new StartState()
        /// </summary>
        /// 

        private SwooshAnimation swoosh;
        private bool swooshPlayed = false;
        
        public override void LoadContent()
        {
            //Load animation frames for playing
            string imagePath = "Video";
            swoosh = new SwooshAnimation(contentManager, imagePath);
            AddGameObject("Swoosh", swoosh);

            // clear and add single soundtrack to sound manager
            soundManager.AddSoundtrackToDictionary("SwooshSound", LoadSound("Video/Intro").CreateInstance());
        }
        public override void HandleInput(GameTime gameTime) { }
        protected override void SetInputManager()   { }
        public override void UpdateGameState(GameTime gameTime)
        {
            /// updated from MainGame.Update(gameTime) ///
            swoosh.Update(gameTime);
            if (swoosh.Frame == 3 && !swooshPlayed) // ensure sound only played when frame 3 reached 
            {
                if (!swooshPlayed)
                {
                    NotifyEvent(new SwooshEvents.SwooshMoviePlays(), "SwooshSound", false); // play just "SwooshSound" soundtrack without looping
                    swooshPlayed = true;
                }
            }
        }
        // Draw is done in BaseGameState.Render()
    }
}
