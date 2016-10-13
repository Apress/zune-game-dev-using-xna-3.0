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

namespace AccelerationSample
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Texture2D greenBallTex;
        bool ballIsFalling;        
        
        Vector2 acceleration;
        Vector2 velocity;
        Vector2 initialPosition;
        Vector2 ballPosition;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            acceleration = new Vector2(0f, 15f);
            velocity = Vector2.Zero;
            initialPosition = Vector2.Zero;
            ballPosition = initialPosition;
            ballIsFalling = false;

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

            greenBallTex = Content.Load<Texture2D>("greenball");
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
            float time;                 
            KeyboardState kbState = Keyboard.GetState();

            // Check for 'Enter'. This makes the ball start falling.
            if (kbState.IsKeyDown(Keys.Enter) && ballIsFalling == false)
            {
                ballIsFalling = true;
            }

            // Check for 'R' (for Reset). Resets the ball position
            if (kbState.IsKeyDown(Keys.R) && ballIsFalling == true)
            {
                ballIsFalling = false;
                ballPosition = initialPosition;
                velocity = Vector2.Zero;
            }

            // Calculate the falling ball's position.
            if (ballIsFalling)
            {
                time = (float)gameTime.ElapsedGameTime.TotalSeconds;
                velocity += acceleration * time;
                ballPosition += velocity * time;
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);

            spriteBatch.Begin();
            spriteBatch.Draw(greenBallTex, ballPosition, Color.White);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
