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

namespace SpriteBlendModes
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        SpriteBlendMode blendMode;
        string blendText;
        Vector2 blendTextOrigin, blendTextPosition;

        Texture2D redBall, blueBall, greenBall;
        Texture2D redBallSolid, blueBallSolid, greenBallSolid;
        bool useSolids = false;

        SpriteFont normalFont;

        Vector2 redBallPosition, blueBallPosition, greenBallPosition;

        InputState input;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            // Frame rate is 30 fps by default for Zune.
            TargetElapsedTime = TimeSpan.FromSeconds(1 / 30.0);

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
            // TODO: Add your initialization logic here
            redBallPosition = new Vector2(32, 96);
            blueBallPosition = new Vector2(80, 96);
            greenBallPosition = new Vector2(56, 130);

            blendMode = SpriteBlendMode.None;

            blendTextPosition = new Vector2(120, 170);

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

            redBall = Content.Load<Texture2D>("redball");
            greenBall = Content.Load<Texture2D>("greenball");
            blueBall = Content.Load<Texture2D>("blueball");

            redBallSolid = Content.Load<Texture2D>("redball_solid");
            greenBallSolid = Content.Load<Texture2D>("greenball_solid");
            blueBallSolid = Content.Load<Texture2D>("blueball_solid");

            normalFont = Content.Load<SpriteFont>("Normal");
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

            if (input.NewPlayPress)
            {
                switch (blendMode)
                {
                    case SpriteBlendMode.Additive:
                        blendMode = SpriteBlendMode.AlphaBlend;
                        break;
                    case SpriteBlendMode.AlphaBlend:
                        blendMode = SpriteBlendMode.None;
                        break;
                    case SpriteBlendMode.None:
                        blendMode = SpriteBlendMode.Additive;
                        break;
                }
            }

            if (input.MiddleButtonPressed)
            {
                useSolids = !useSolids;
            }

            blendText = blendMode.ToString();
            blendTextOrigin = normalFont.MeasureString(blendText) / 2;

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            
            Color spriteTintColor = new Color(1.0f, 1.0f, 1.0f, 0.7f);
            Color fontTintColor = new Color(0.5f, 0.5f, 0.5f, 0.99f);
            
            // Begin the ball sprite batch
            spriteBatch.Begin(blendMode);

            if (useSolids)
            {
                spriteBatch.Draw(redBallSolid, redBallPosition, Color.White);
                spriteBatch.Draw(blueBallSolid, blueBallPosition, Color.White);
                spriteBatch.Draw(greenBallSolid, greenBallPosition, Color.White);
            }
            else
            {
                spriteBatch.Draw(redBall, redBallPosition, spriteTintColor);
                spriteBatch.Draw(blueBall, blueBallPosition, spriteTintColor);
                spriteBatch.Draw(greenBall, greenBallPosition, spriteTintColor);
            }

            spriteBatch.DrawString(normalFont, blendText, blendTextPosition,
                fontTintColor, 0.0f, blendTextOrigin, 1.0f, SpriteEffects.None, 0.5f);

            spriteBatch.End();

            // Begin the Text sprite batch (default blend mode: alpha blend)
            spriteBatch.Begin(SpriteBlendMode.Additive);

            
            
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
