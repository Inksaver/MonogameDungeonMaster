using DMClone.Engine.States;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DMClone.Engine.Objects
{
    internal class BaseGameObject
    {
        protected Rectangle dRectangle;             // destination rectangle eg dungeon opening: 0,94,488,326
        protected Rectangle sRectangle;             // use this rectangle to get portion of the source eg Start.png           
        protected Texture2D texture;
        protected Texture2D boundingBoxTexture;
        protected Vector2 position = Vector2.One;
        protected float angle;
        protected Vector2 direction;
        protected List<BoundingBox> boundingBoxes = new List<BoundingBox>();
        protected SpriteFont font;
        //protected string renderText = "";
        //protected string text;
        public virtual string RenderText { get; set; }
        public virtual int TextPosX { get; set; } = -1;
        public virtual int TextPosY { get; set; } = -1;
        public virtual float TextWidth { get; set; }
        public virtual float TextHeight { get; set; }
        public virtual string TextAlignH { get; set; } = "";
        public virtual string TextAlignV { get; set; } = "";
        public virtual float TextScale { get; set; } = 1.0f;
        public virtual Color TextColor { get; set; } = Color.White;
        public virtual bool IsVisible { get; set; } = true;
        public virtual float Rotation { get; set; } = 0.0f;
        public virtual int DrawOrder { get; set; } = 0;                 // drawingOrder of this game object
        public virtual Vector2 Offset { get; set; } = Vector2.Zero;
        public int zIndex;
        public event EventHandler<BaseGameStateEvent> OnObjectChanged;
        public bool Destroyed { get; private set; }
        public virtual int Width { get { return texture.Width; } }
        public virtual int Height { get { return texture.Height; } }
        public virtual Vector2 Position
        {
            get { return position; }
            set
            {
                var deltaX = value.X - position.X;
                var deltaY = value.Y - position.Y;
                position = value;

                foreach (var bb in boundingBoxes)
                {
                    bb.Position = new Vector2(bb.Position.X + deltaX, bb.Position.Y + deltaY);
                }
            }
        }
        public virtual void RevertImage() { }
        public virtual void ChangeImage(string itemName) { }
        public virtual void ChangeImage(Texture2D texture, string itemName) { }
        public virtual void ChangeSourceRectangleWidth(int byAmount)
        {
            sRectangle = new Rectangle(sRectangle.X, sRectangle.Y, sRectangle.Width + byAmount, sRectangle.Height);
        }
        public List<BoundingBox> BoundingBoxes
        {
            get
            {
                return boundingBoxes;
            }
        }
        public virtual void SetSize(string size)
        {

        }
        public virtual void ChangeText(string text)
        {
            RenderText = text.ToUpper();
            //TextWidth = font.MeasureString(text).X * TextScale;
            //TextHeight = font.MeasureString(text).Y * TextScale;
        }
        public virtual void ChangeText(string text, string row)
        {
            RenderText = text;
            TextAlignV = row;
        }
        public virtual void ChangeTextColour(Color colour)
        {
            TextColor = colour;
        }
        public virtual void Initialise() { }
        public virtual void Initialise(Texture2D texture) { }
        public virtual void Initialise(string textureName) { }
        public virtual void OnNotify(BaseGameStateEvent gameEvent) { }
        public virtual void Render(SpriteBatch spriteBatch)
        {

            if (!Destroyed && IsVisible)
            {
                if (dRectangle == Rectangle.Empty)
                    spriteBatch.Draw(texture, position + Offset, Color.White);
                else
                {
                    Rectangle destRectangle = new Rectangle((int)position.X, (int)position.Y, sRectangle.Width, sRectangle.Height);
                    if (Rotation == 0.0f)
                        spriteBatch.Draw(texture: texture, destinationRectangle: destRectangle, sourceRectangle: sRectangle, Color.White);
                    else
                    {
                        Vector2 originOffset = new Vector2(destRectangle.Width / 2, destRectangle.Height / 2);
                        spriteBatch.Draw(texture: texture,
                                            destinationRectangle: destRectangle,
                                            sourceRectangle: sRectangle,
                                            color: Color.White,
                                            rotation: Rotation,
                                            origin: originOffset + Offset,
                                            effects: SpriteEffects.None,
                                            layerDepth: 0);
                    }
                }
            }
        }
        public virtual void DrawString(SpriteBatch spriteBatch)
        {
            if (TextScale == 1.0f)
            {
                spriteBatch.DrawString(spriteFont: font,
                                        text: RenderText,
                                        position: new Vector2(TextPosX, TextPosY),
                                        color: TextColor);
            }
            else
            {
                spriteBatch.DrawString(spriteFont: font,
                                        text: RenderText,
                                        position: new Vector2(TextPosX, TextPosY),
                                        color: TextColor,
                                        rotation: Rotation,
                                        origin: Vector2.Zero,
                                        scale: TextScale,
                                        effects: SpriteEffects.None,
                                        layerDepth: 0);
            }
        }
        public void RenderBoundingBoxes(SpriteBatch spriteBatch)
        {
            if (Destroyed)
            {
                return;
            }
            if (boundingBoxTexture == null)
            {
                CreateBoundingBoxTexture(spriteBatch.GraphicsDevice);
            }
            foreach (var bb in boundingBoxes)
            {
                spriteBatch.Draw(boundingBoxTexture, bb.Rectangle, Color.Red);
            }
        }
        public virtual void Update(GameTime gameTime) { }
        public void Destroy()
        {
            Destroyed = true;
        }
        public void SendEvent(BaseGameStateEvent e)
        {
            OnObjectChanged?.Invoke(this, e);
        }
        public void AddBoundingBox(BoundingBox bb)
        {
            boundingBoxes.Add(bb);
        }
        protected Vector2 CalculateDirection(float angleOffset = 0.0f)
        {
            direction = new Vector2((float)Math.Cos(angle - angleOffset), (float)Math.Sin(angle - angleOffset));
            direction.Normalize();

            return direction;
        }
        private void CreateBoundingBoxTexture(GraphicsDevice graphicsDevice)
        {
            boundingBoxTexture = new Texture2D(graphicsDevice, 1, 1);
            boundingBoxTexture.SetData<Color>(new Color[] { Color.White });
        }
    }
}
