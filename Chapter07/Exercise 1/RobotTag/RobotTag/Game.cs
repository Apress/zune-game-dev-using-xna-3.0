using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Net;

using ZuneScreenManager;

namespace RobotTag
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game : Microsoft.Xna.Framework.Game
    {
        // Fields
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        ScreenManager screenManager;        
        GamerServicesComponent gamerServices;        

        public Game()
        {
            graphics = new GraphicsDeviceManager(this);

            // Set buffer width and height (this is Zune specific)
            graphics.PreferredBackBufferHeight = 240;
            graphics.PreferredBackBufferWidth = 320;

            // Create a new ScreenManager component (ours) and a GamerServices component (Framework's)
            screenManager = new ScreenManager(this);
            gamerServices = new GamerServicesComponent(this);            

            // Add the components
            this.Components.Add(screenManager);
            this.Components.Add(gamerServices);            

            // Show the first screen of the game, the MainMenuScreen
            screenManager.AddScreen(new MainMenuScreen());

            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // Load in Tahoma and BlankTex, which are required for the ScreenManager's Font property and Fade functionality.
            SpriteFont tahoma = Content.Load<SpriteFont>("Tahoma");
            Texture2D blankTex = Content.Load<Texture2D>("Textures/blank");

            // Initialize the ScreenManager's content components.
            screenManager.InitializeRequiredContent(tahoma, blankTex);
        }

        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.Black);
            base.Draw(gameTime);
        }
    }
}
