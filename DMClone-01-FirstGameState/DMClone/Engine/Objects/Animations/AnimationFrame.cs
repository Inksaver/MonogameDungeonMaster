using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DMClone.Engine.Objects.Animations
{
    internal class AnimationFrame
    {
        public Rectangle SourceRectangle { get; private set; }
        public Texture2D FrameTexture { get; private set; }
        public int Lifespan { get; private set; }

        public AnimationFrame(Rectangle sourceRectangle, int lifespan)
        {
            SourceRectangle = sourceRectangle;
            Lifespan = lifespan;
        }
        public AnimationFrame(Texture2D frameTexture, int lifespan)
        {
            this.FrameTexture = frameTexture;
            Lifespan = lifespan;
        }
    }
}
