using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using DMClone.Engine.Objects;
using DMClone.Engine.Input;
using DMClone.Engine.Sound;

namespace DMClone.Engine.States
{
    internal abstract class BaseGameState
    {
        //protected Texture2D mouseCursor;
        protected bool debug = false;
        protected bool indestructible = false;
        protected ContentManager contentManager;
        protected int viewportHeight;
        protected int viewportWidth;
        protected SoundManager soundManager = new SoundManager();
        private readonly List<BaseGameObject> gameObjects = new List<BaseGameObject>();
        private readonly Dictionary<string, BaseGameObject> gameObjectsDict = new Dictionary<string, BaseGameObject>(); // alternative to List
        private GraphicsDevice graphicsDevice;
        protected InputManager InputManager { get; set; }
        public void Initialize(ContentManager contentManager, GraphicsDeviceManager graphics)
        {
            this.contentManager = contentManager;
            this.graphicsDevice = graphics.GraphicsDevice;
            this.viewportHeight = graphics.GraphicsDevice.Viewport.Width;
            this.viewportWidth = graphics.GraphicsDevice.Viewport.Width;
            SetInputManager();
        }
        public abstract void LoadContent();                                 // interface for LoadContent in child classes
        public abstract void HandleInput(GameTime gameTime);                // interface for Input in child classes
        public abstract void UpdateGameState(GameTime gameTime);
        public event EventHandler<BaseGameState> OnStateSwitched;           // main class listens for this event
        public event EventHandler<BaseGameStateEvent> OnEventNotification;  // originally from Chapter_05.Enum.Events.cs
        protected abstract void SetInputManager();
        public void UnloadContent()
        {
            contentManager.Unload();
        }
        public void Update(GameTime gameTime)
        {
            UpdateGameState(gameTime);
            //soundManager.PlaySoundtrack(); // used for continuous soud eg music
        }
        public void Update(GameTime gameTime, int soundTrackIndex, bool looping = false)
        {
            UpdateGameState(gameTime);
            //soundManager.PlaySoundtrack(soundTrackIndex, looping);
        }
        protected Texture2D LoadTexture(string textureName)
        {
            return contentManager.Load<Texture2D>(textureName);
        }
        protected SpriteFont LoadFont(string fontName)
        {
            return contentManager.Load<SpriteFont>(fontName);
        }
        protected SoundEffect LoadSound(string soundName)
        {
            return contentManager.Load<SoundEffect>(soundName);
        }
        protected void NotifyEvent(BaseGameStateEvent gameEvent, int index = -1, bool looping = false)// MainClass also listening for event notifications
        {
            OnEventNotification?.Invoke(this, gameEvent);

            foreach (var gameObject in gameObjects)
            {
                gameObject.OnNotify(gameEvent);
            }
            if (index == -1)
                soundManager.OnNotify(gameEvent);
            else
            {
                if(gameEvent.GetType() == typeof(BaseGameStateEvent.SoundStop))
                    soundManager.StopSoundTrack(index);
                else
                    soundManager.PlaySoundtrack(index, looping);
            }
        }
        protected void NotifyEvent(BaseGameStateEvent gameEvent, string key = "", bool looping = false)// MainClass also listening for event notifications
        {
            OnEventNotification?.Invoke(this, gameEvent);

            foreach (var gameObject in gameObjects)
            {
                gameObject.OnNotify(gameEvent);
            }
            if (key == "")
                soundManager.OnNotify(gameEvent);
            else
            {
                if (gameEvent.GetType() == typeof(BaseGameStateEvent.SoundStop))
                    soundManager.StopSoundTrack(key);
                else
                    soundManager.PlaySoundtrack(key, looping);
            }
        }
        protected void SwitchState(BaseGameState gameState, bool unloadContent)                 // child classes can call this to change states
        {
            OnStateSwitched?.Invoke(this, gameState);
            //OnStateSwitched?.DynamicInvoke(this, gameState, unloadContent);
        }
        protected void AddGameObject(BaseGameObject gameObject)             // Add new game objects to List. Keeps track of them for drawing
        {
            gameObjects.Add(gameObject);                                    // eg. sprites, static images, etc
        }
        protected void AddGameObject(string key, BaseGameObject gameObject)             // Add new game objects to List. Keeps track of them for drawing
        {
            gameObjectsDict.Add(key, gameObject);                                    // eg. sprites, static images, etc
        }
        protected void ClearGameObjects()
        {
            gameObjects.Clear();
            gameObjectsDict.Clear();
        }
        protected void RemoveGameObject(BaseGameObject gameObject)
        {
            gameObjects.Remove(gameObject);
        }
        protected void RemoveGameObject(string key)
        {
            gameObjectsDict.Remove(key);
        }
        public virtual void Render(SpriteBatch spriteBatch)                 // Draw all objects in turn. Called from MainGame.Draw
        {
            // All calls use BaseGameObject class
            // clear in black
            graphicsDevice.Clear(Color.Black);
            foreach (var gameObject in gameObjects.OrderBy(a => a.zIndex))  // zIndex to control drawing order
            {
                if (debug)
                    gameObject.RenderBoundingBoxes(spriteBatch);

                if (string.IsNullOrEmpty(gameObject.RenderText))    // NOT a text object
                    gameObject.Render(spriteBatch);
                else
                    gameObject.DrawString(spriteBatch);
            }
            // Alternative Dictionary of gameObjects used so individual objects can be made invisible before calling
            foreach(KeyValuePair<string, BaseGameObject> kvp in gameObjectsDict.OrderBy(a => a.Value.zIndex))
            {
                if (debug)
                    kvp.Value.RenderBoundingBoxes(spriteBatch);

                if (kvp.Value.IsVisible)
                {
                    if (string.IsNullOrEmpty(kvp.Value.RenderText))    // NOT a text object
                        kvp.Value.Render(spriteBatch);
                    else
                        kvp.Value.DrawString(spriteBatch);
                }
            }
        }
    }
}
