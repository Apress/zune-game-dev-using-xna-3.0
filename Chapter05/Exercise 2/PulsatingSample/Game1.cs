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

namespace PulsatingSample
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
        TimeSpan fallStartTime;
        float acceleration;
        float initialVelocity;
        Vector2 initialPosition;
        Vector2 ballPosition;

        Vector2 pulsingBallPosition;
        Vector2 pulsingBallOrigin;
        float pulseScale;
        float redGlowAmount;
        Color glowColor;

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
            acceleration = 150.0f;
            initialVelocity = 0.0f;
            initialPosition = Vector2.Zero;
            ballPosition = initialPosition;

            ballIsFalling = false;
            fallStartTime = TimeSpan.Zero;
            pulseScale = 1.0f;
            glowColor = Color.White;

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
            
            pulsingBallPosition = new Vector2(
                GraphicsDevice.Viewport.Width / 2,
                GraphicsDevice.Viewport.Height / 2);

            pulsingBallOrigin = new Vector2(
                greenBallTex.Width / 2,
                greenBallTex.Height / 2);
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
                fallStartTime = gameTime.TotalGameTime;
            }

            // Check for 'R' (for Reset). Resets the ball position
            if (kbState.IsKeyDown(Keys.R) && ballIsFalling == true)
            {
                ballIsFalling = false;
                ballPosition = initialPosition;
            }

            // Calculate the falling ball's position.
            if (ballIsFalling)
            {
                time = (float)gameTime.TotalGameTime.Subtract(fallStartTime).TotalMilliseconds;
                time /= 1000.0f;

                ballPosition.Y = (initialVelocity * time) +
                    initialPosition.Y +
                    (0.5f * acceleration * time * time);

                // Update the pulse scale
                pulseScale = (float)Math.Abs(Math.Sin(5.0f * time)) + 1.0f;

                // Get the pulse color
                redGlowAmount = 1.0f - (float)Math.Abs(Math.Sin(5.0f * time));
                glowColor = new Color(1.0f, redGlowAmount, redGlowAmount);
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
            spriteBatch.Draw(greenBallTex, pulsingBallPosition, null, glowColor, 
                1.0f, pulsingBallOrigin, pulseScale, SpriteEffects.None, 0.5f);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
