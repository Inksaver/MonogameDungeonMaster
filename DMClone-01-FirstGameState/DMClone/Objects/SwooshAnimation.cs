using DMClone.Engine.Objects;
using DMClone.Engine.Objects.Animations;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace DMClone.Objects
{
    internal class SwooshAnimation : BaseGameObject
    {
        private const float fps = 0.1f;                             // Animation speed
        private const int frameLifespan = 1;                        // image display lifespan ms
        private Animation swoosh = new Animation(false, fps);       // create new Animation Object, no looping, 0.1 fps
        
        public int Frame { get; private set; } = 0;
        public SwooshAnimation(ContentManager contentManager, string imagePath)
        {
            for(int i = 1; i <= 78; i++)                            // 78 images called Frame01 to Frame78
            {
                string frame = $"{imagePath}/Frame" + i.ToString("D2");
                swoosh.AddFrame(contentManager.Load<Texture2D>(frame), frameLifespan);
            }
            swoosh.Reset();
            texture = swoosh.CurrentFrame.FrameTexture;
        }
        public override void Update(GameTime gametime)
        {
            /// updated from MainGame.Update(gameTime) => SwooshState.UpdateGameState///
            texture = null;
            if (swoosh.Update(gametime))
            {
                texture = swoosh.CurrentFrame.FrameTexture;
                Frame = swoosh.Frame;
            }
            else
                Destroy(); // set boolean flag for this animation to be destroyed
        }
        public override void Render(SpriteBatch spriteBatch)
        {
            if (texture != null)
                spriteBatch.Draw(texture, Vector2.Zero, Color.White);
        }
    }
}
