using DMClone.Engine.States;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DMClone.Engine
{
    internal class MainGame : Game
    {
        //public static MouseCursor FromTexture2D(Texture2D texture, int originx, int originy)
        public Vector2 WindowSize { get; set; }
        public static int WindowWidth { get; set; }
        public static int WindowHeight { get; set; }
        private BaseGameState currentGameState;
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        /// <summary>
        /// variables to allow scaling to a specific resolution, irrespective of screen res.
        /// They are used in Initialize()
        /// </summary>
        private RenderTarget2D renderTarget;
        private Rectangle renderScaleRectangle;
        private int designedResolutionWidth;
        private int designedResolutionHeight;
        private float designedResolutionAspectRatio;
        private BaseGameState firstGameState;
        public MainGame(int width, int height, BaseGameState firstGameState)
        {
            /// Called from Program.cs with new SwooshState() passed as firstGameState ///
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            
            //IsMouseVisible = true;
            this.firstGameState = firstGameState;
            designedResolutionWidth = width;
            designedResolutionHeight = height;
            designedResolutionAspectRatio = width / (float)height;
        }
        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            graphics.PreferredBackBufferWidth = designedResolutionWidth;
            graphics.PreferredBackBufferHeight = designedResolutionHeight;
            graphics.IsFullScreen = false;
            graphics.ApplyChanges();
            //MouseCursor.FromTexture2D(Content.Load<Texture2D>("Sprites/hand_cursor"), 1, 1);
            /// render target is a graphical buffer used to draw on until ready to send to screen
            /// sets viewport resolution, mipmap flag to false, background to black (0)
            /// no depth stencil buffer, zero preferredMultiSampleCount
            renderTarget = new RenderTarget2D(graphics.GraphicsDevice,
                                              designedResolutionWidth,
                                              designedResolutionHeight,
                                              false,
                                              SurfaceFormat.Color,
                                              DepthFormat.None,
                                              0,
                                              RenderTargetUsage.DiscardContents);

            renderScaleRectangle = GetScaleRectangle();
            WindowSize = new Vector2(renderScaleRectangle.Width, renderScaleRectangle.Height);
            WindowWidth = renderScaleRectangle.Width;
            WindowHeight = renderScaleRectangle.Height;
            Shared.Initialise();    // Errors here if Shared.Initialise did not complete correctly
            Shared.WindowWidth = WindowWidth; 
            Shared.WindowHeight = WindowHeight;
            Shared.ContentManager = Content;

            base.Initialize();
        }
        /// <summary>
        /// Uses the current window size compared to the design resolution
        /// Provides black bars similar to tv screen showing 4:3 on 16:9
        /// </summary>
        /// <returns>Scaled Rectangle</returns>
        private Rectangle GetScaleRectangle()
        {
            var variance = 0.5;
            // calculate aspect ratio of game
            var actualAspectRatio = Window.ClientBounds.Width / (float)Window.ClientBounds.Height;
            Rectangle scaleRectangle;
            if (actualAspectRatio <= designedResolutionAspectRatio) // lower than or equal to designed ratio add bars top & bottom
            {
                var presentHeight = (int)(Window.ClientBounds.Width / designedResolutionAspectRatio + variance);
                var barHeight = (Window.ClientBounds.Height - presentHeight) / 2;
                scaleRectangle = new Rectangle(0, barHeight, Window.ClientBounds.Width, presentHeight);
            }
            else
            {
                var presentWidth = (int)(Window.ClientBounds.Height * designedResolutionAspectRatio + variance);
                var barWidth = (Window.ClientBounds.Width - presentWidth) / 2;
                scaleRectangle = new Rectangle(barWidth, 0, presentWidth, Window.ClientBounds.Height);
            }

            return scaleRectangle;
        }
        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            SwitchGameState(firstGameState, true);  // true means unload content when switching
        }
        private void CurrentGameState_OnStateSwitched(object sender, BaseGameState e, bool unload)
        {
            SwitchGameState(e, unload);
        }
        private void SwitchGameState(BaseGameState gameState, bool unloadContent)
        {
            if (currentGameState != null)
            {
                currentGameState.OnStateSwitched -= (sender, e) => CurrentGameState_OnStateSwitched(sender, e, unloadContent);
                currentGameState.OnEventNotification -= CurrentGameState_OnEventNotification;
                if (unloadContent)
                    currentGameState.UnloadContent();
            }
            currentGameState = gameState;
            currentGameState.Initialize(Content, graphics);
            currentGameState.LoadContent();     // internal flag will determine whether to re-load content per state
            currentGameState.OnStateSwitched += (sender, e) => CurrentGameState_OnStateSwitched(sender, e, unloadContent);
            currentGameState.OnEventNotification += CurrentGameState_OnEventNotification;
            
        }
        private void CurrentGameState_OnEventNotification(object sender, BaseGameStateEvent e)
        {
            switch (e)
            {
                case BaseGameStateEvent.GameQuit _:
                    Exit();
                    break;
            }
        }
        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            currentGameState?.UnloadContent(); //Unload if currentGameState != null
        }
        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        protected override void Update(GameTime gameTime)
        {
            if (currentGameState != null)
            {
                currentGameState.HandleInput(gameTime);
                currentGameState.Update(gameTime);
            }
            base.Update(gameTime);
        }
        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            // Render to the Render Target
            GraphicsDevice.SetRenderTarget(renderTarget);
            GraphicsDevice.Clear(Color.CornflowerBlue);
            if (currentGameState != null)
            {
                spriteBatch.Begin();
                currentGameState.Render(spriteBatch);
                spriteBatch.End();
            }

            // Now render the scaled content
            graphics.GraphicsDevice.SetRenderTarget(null);
            graphics.GraphicsDevice.Clear(ClearOptions.Target, Color.Black, 1.0f, 0);
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque);
            spriteBatch.Draw(renderTarget, renderScaleRectangle, Color.White);
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
