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
using InputHandler;

namespace LandscapeGame
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteFont normal;

        Texture2D ballTex;        

        Vector2 ballPosition;
        Vector2 ballDirection, initialDirection;

        RenderTarget2D backTexture;
        bool isLandscape = false;
        string orientation;

        InputState input;

        public int ScreenWidth
        {
            get
            {
                if (isLandscape)
                    return 320;
                else
                    return 240;
            }
        }

        public int ScreenHeight
        {
            get
            {
                if (isLandscape)
                    return 240;
                else
                    return 320;
            }
        }

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            // Frame rate is 30 fps by default for Zune.
            TargetElapsedTime = TimeSpan.FromSeconds(1 / 30.0);

            // add the input control
            input = new InputState(this);
            Components.Add(input);
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // set an arbitrary initial direction
            // 2 units to the left, 7 units down
            initialDirection = new Vector2(2.0f, 7.0f);
            ballDirection = initialDirection;
            ballPosition = Vector2.Zero;

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

            normal = Content.Load<SpriteFont>("Normal");
            ballTex = Content.Load<Texture2D>("ball");
            backTexture = new RenderTarget2D(GraphicsDevice, 320, 240, 0, SurfaceFormat.Color);
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
            if (input.NewBackPress)
                this.Exit();

            if (input.MiddleButtonPressed)
            {
                isLandscape = !isLandscape;
                
                // reset ball as it might be out of bounds
                ballPosition = Vector2.Zero;
                ballDirection = initialDirection;
            }

            ballPosition += ballDirection;
            HandleCollisions();

            orientation = isLandscape ? "Orientation: Landscape" : "Orientation: Portrait";

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            if (isLandscape)
            {
                // Set the render target, which draws to the back texture
                GraphicsDevice.SetRenderTarget(0, backTexture);
            }

            // Code to draw the game
            graphics.GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();
            spriteBatch.Draw(ballTex, ballPosition, Color.White);
            spriteBatch.DrawString(normal, orientation, Vector2.Zero, Color.White);
            base.Draw(gameTime);
            spriteBatch.End();

            if (isLandscape)
            {
                GraphicsDevice.SetRenderTarget(0, null);
                spriteBatch.Begin();
                spriteBatch.Draw(backTexture.GetTexture(), new Vector2(120, 160), null,
                    Color.White, MathHelper.PiOver2, new Vector2(160, 120), 1.0f,
                    SpriteEffects.None, 0);
                spriteBatch.End();
            }
        }

        private void HandleCollisions()
        {
            if (ballPosition.X <= 0 || 
                ballPosition.X + ballTex.Width >= ScreenWidth)
            {
                // Flip the vector about X
                ballDirection = Vector2.Reflect(ballDirection, Vector2.UnitX);
            }

            if (ballPosition.Y <= 0 ||
                ballPosition.Y + ballTex.Height >= ScreenHeight)
            {
                // Flip the vector about Y
                ballDirection = Vector2.Reflect(ballDirection, Vector2.UnitY);
            }
        }
    }
}
