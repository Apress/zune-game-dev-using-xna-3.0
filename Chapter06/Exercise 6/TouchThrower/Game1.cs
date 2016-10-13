using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

namespace TouchThrower
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        // Texture
        Texture2D ballTex;

        // Tracking variables
        bool isTracking = false;
        Vector2 touchStartPoint;
        Vector2 touchEndPoint;        
        TimeSpan touchStartTime;
        TimeSpan totalTouchTime;
        
        // Speed and deceleration
        float velocity;
        float deceleration = 0.1f;        

        // Ball position and direction
        Vector2 ballPosition;
        Vector2 ballDirection;

        // Screen width and height
        int screenWidth, screenHeight;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            // Frame rate is 30 fps by default for Zune.
            TargetElapsedTime = TimeSpan.FromSeconds(1 / 30.0);
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            ballTex = Content.Load<Texture2D>("ball");            
            screenWidth = GraphicsDevice.Viewport.Width;
            screenHeight = GraphicsDevice.Viewport.Height;

            ballPosition = new Vector2(
                screenWidth / 2 - ballTex.Width / 2, 
                screenHeight / 2 - ballTex.Height / 2);

            ballDirection = Vector2.Zero;
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            HandleCollisions();

            HandleTouchPad(gameTime);

            // stop the ball if the velocity becomes negative
            if (velocity > 0)
                velocity = velocity - deceleration;
            else
                velocity = 0f;

            // update the ball position
            ballPosition += ballDirection * velocity;

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();
            spriteBatch.Draw(ballTex, ballPosition, Color.White);            
            spriteBatch.End();

            base.Draw(gameTime);
        }

        private void HandleCollisions()
        {
            if (ballPosition.X <= 0 ||
                ballPosition.X + ballTex.Width >= screenWidth)
            {
                ballDirection = Vector2.Reflect(ballDirection, Vector2.UnitX);
            }

            if (ballPosition.Y <= 0 ||
                ballPosition.Y + ballTex.Height >= screenHeight)
            {
                ballDirection = Vector2.Reflect(ballDirection, Vector2.UnitY);
            }
        }

        private void HandleTouchPad(GameTime gameTime)
        {
            GamePadState state = GamePad.GetState(PlayerIndex.One);
            Vector2 zunePadVector = state.ThumbSticks.Left;

            if (isTracking)
            {
                // if the zune pad is no longer being touched
                if (zunePadVector == Vector2.Zero)
                {
                    // end tracking
                    isTracking = false;
                    totalTouchTime = gameTime.TotalRealTime.Subtract(touchStartTime);                    
                    
                    // fling the ball
                    FlingBall(totalTouchTime, touchStartPoint, touchEndPoint);
                }
                else
                {
                    // update the current end point
                    touchEndPoint = zunePadVector;
                }
            }
            else
            {
                // if the zune pad has been touched                
                if (zunePadVector != Vector2.Zero)
                {
                    // start tracking
                    velocity = 0.0f;
                    ballDirection = Vector2.Zero;
                    touchStartTime = gameTime.TotalRealTime;
                    touchStartPoint = zunePadVector;
                    isTracking = true;
                }
            }
        }

        private void FlingBall(TimeSpan touchTime, Vector2 startPoint, Vector2 endPoint)
        {
            ballDirection = Vector2.Subtract(endPoint, startPoint);
            // invert Y axis
            ballDirection.Y = -ballDirection.Y;
            velocity = ballDirection.Length() / (float)touchTime.TotalSeconds;            
        }
    }
}
