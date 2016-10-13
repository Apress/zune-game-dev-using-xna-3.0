using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using CardLib;
using GameStateManager;

namespace CrazyEights
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class CrazyEightsGame : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        ScreenManager screenManager;

        public CrazyEightsGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            // Frame rate is 30 fps by default for Zune.
            TargetElapsedTime = TimeSpan.FromSeconds(1 / 30.0);    

            screenManager = new ScreenManager(this);
            Components.Add(screenManager);

            // Start the game with the main menu screen.
            screenManager.AddScreen(new MainMenuScreen());
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            SharedSpriteBatch.Instance.Initialize(this);
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.DarkGreen);            

            base.Draw(gameTime);

            SharedSpriteBatch.Instance.End();
        }
    }
}
