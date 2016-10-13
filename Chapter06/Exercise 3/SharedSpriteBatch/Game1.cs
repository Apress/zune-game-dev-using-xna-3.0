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

namespace SharedSpriteBatch
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        Texture2D red, blue, green;
    

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
            SharedSpriteBatch.Instance.Initialize(this);

            red = Content.Load<Texture2D>("redball_solid");
            blue = Content.Load<Texture2D>("blueball_solid");
            green = Content.Load<Texture2D>("greenball_solid");
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

            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
           
            // Using multiple blend modes
            SharedSpriteBatch.Instance.Draw(SpriteBlendMode.None, red, Vector2.Zero, Color.White);
            SharedSpriteBatch.Instance.Draw(SpriteBlendMode.Additive, green, new Vector2(50, 50), Color.White);
            SharedSpriteBatch.Instance.Draw(SpriteBlendMode.AlphaBlend, blue, new Vector2(100, 100), Color.White);

            // Using the same blend mode (additive)
            SharedSpriteBatch.Instance.Draw(SpriteBlendMode.Additive, red, new Vector2(0, 150), Color.White);
            SharedSpriteBatch.Instance.Draw(SpriteBlendMode.Additive, green, new Vector2(50, 200), Color.White);
            SharedSpriteBatch.Instance.Draw(SpriteBlendMode.Additive, blue, new Vector2(100, 250), Color.White);  

            SharedSpriteBatch.Instance.End();
            SharedSpriteBatch.Instance.debug = false;

            base.Draw(gameTime);
        }
    }
}
