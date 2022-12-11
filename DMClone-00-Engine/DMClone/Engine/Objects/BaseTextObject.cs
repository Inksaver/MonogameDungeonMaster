using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DMClone.Engine.Objects
{
    internal class BaseTextObject : BaseGameObject
    {
        //protected SpriteFont font;
        public override void Render(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(font, RenderText, position, Color.White);
        }
    }
}
