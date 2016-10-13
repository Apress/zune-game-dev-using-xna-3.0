using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

namespace SuperSimpleXNAGame
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Texture2D blockTexture;
        Vector2 blockPosition;
        float blockRotation;

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
            int width = graphics.GraphicsDevice.Viewport.Width;
            int height = graphics.GraphicsDevice.Viewport.Height;

            // Center the block
            blockPosition = new Vector2(width / 2, height / 2);

            // Initialize rotation
            blockRotation = 0.0f;

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

            blockTexture = Content.Load<Texture2D>("Textures/Block");
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
                        
            blockPosition.X += (float)Math.Cos(gameTime.TotalRealTime.TotalSeconds);
            blockPosition.Y += -(float)Math.Sin(gameTime.TotalRealTime.TotalSeconds);

            // Update keyboard state
            if (Keyboard.GetState().IsKeyDown(Keys.Left))
                blockPosition.X -= 1.0f;
            if (Keyboard.GetState().IsKeyDown(Keys.Right))
                blockPosition.X += 1.0f;
            if (Keyboard.GetState().IsKeyDown(Keys.Up))
                blockPosition .Y -= 1.0f;
            if (Keyboard.GetState().IsKeyDown(Keys.Down))
                blockPosition.Y += 1.0f;

            // Update rotation
            blockRotation += -0.02f;

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.Black);

            // TODO: Add your drawing code here
            spriteBatch.Begin();
            spriteBatch.Draw(blockTexture, blockPosition, null, Color.White,
                blockRotation, Vector2.Zero, 1.0f, SpriteEffects.None, 0.5f);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
