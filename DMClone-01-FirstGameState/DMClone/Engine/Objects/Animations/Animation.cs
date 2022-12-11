using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace DMClone.Engine.Objects.Animations
{
    internal class Animation
    {
        private List<AnimationFrame> frames = new List<AnimationFrame>();
        private int animationAge = 0;
        private int lifespan = -1;
        private float timePassed = 0;
        private bool isLoop = false;
        public bool Active { get; set; } = true;
        public int Frame { get; set; } = 0;
        public float FrameRate { get; set; } = 1.0f;
        public int Lifespan
        {
            // returns the running time of the animation
            get
            {
                if (lifespan < 0)
                {
                    lifespan = 0;
                    foreach (var frame in frames)
                    {
                        lifespan += frame.Lifespan;
                    }
                }
                return lifespan;
            }
        }
        public AnimationFrame CurrentFrame { get; private set; }
        public Animation ReverseAnimation
        {
            get
            {
                var newAnimation = new Animation(isLoop);
                for (int i = frames.Count - 1; i >= 0; i--)
                {
                    newAnimation.AddFrame(frames[i].SourceRectangle, frames[i].Lifespan);
                }

                return newAnimation;
            }
        }

        public Animation(bool looping, float fps = 1.0f)
        {
            isLoop = looping;
            FrameRate = fps;
        }

        public void AddFrame(Rectangle sourceRectangle, int lifespan)
        {
            frames.Add(new AnimationFrame(sourceRectangle, lifespan));
        }
        public void AddFrame(Texture2D frameTexture, int lifespan)
        {
            frames.Add(new AnimationFrame(frameTexture, lifespan));
        }

        public bool Update(GameTime gameTime)
        {
            /// change frames when animationAge >= Lifespan
            
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            timePassed += dt;
            animationAge++;

            if (isLoop && animationAge > Lifespan)
            {
                animationAge = 0;
            }
            if (timePassed > FrameRate)
            {
                timePassed = 0;
                Frame += 1;
                if (Frame >= frames.Count)
                {
                    Active = false;
                    CurrentFrame = null;
                }
                else
                {
                    CurrentFrame = frames[Frame];
                }
            }
            return Active;
        }

        public void Reset()
        {
            animationAge = 0;
            Frame = 0;
            CurrentFrame = frames[0];
        }
    }
}
