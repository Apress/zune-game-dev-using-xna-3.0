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
using MonkeyFeeder.Components;

namespace MonkeyFeeder
{
    /// <summary>
    /// Defines the three possible states for the game.
    /// </summary>
    public enum GameState
    {
        Start,
        Playing,
        GameOver
    }

    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class MonkeyFeederGame : Microsoft.Xna.Framework.Game
    {
        #region Fields

        // Default fields
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        // content
        Texture2D startTex;
        Texture2D beachTex;
        Texture2D gameOverTex;
        SpriteFont smallFont;

        // game components
        InputState input;
        Water waterComponent;
        TossArrow tossArrow;
        WindNeedle windNeedle;
        Monkey monkey;
        BananaProjectile banana;

        GameState gameState;

        // score related variables
        private int score;
        string finalScoreText;
        private Vector2 scoreTextPosition;
        private Vector2 gameOverScorePos;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Creates a new instance of the MonkeyFeederGame class and 
        /// adds game components to it.
        /// </summary>
        public MonkeyFeederGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            // Frame rate is 30 fps by default for Zune.
            TargetElapsedTime = TimeSpan.FromSeconds(1 / 30.0);

            // add components
            input = new InputState(this);
            waterComponent = new Water(this);
            tossArrow = new TossArrow(this);
            windNeedle = new WindNeedle(this);            
            monkey = new Monkey(this);
            banana = new BananaProjectile(this);

            // Make sure to add drawable components
            // in the order you want them drawn.
            Components.Add(input);
            Components.Add(waterComponent);
            Components.Add(tossArrow);
            Components.Add(windNeedle);            
            Components.Add(monkey);
            Components.Add(banana);
        }

        #endregion

        #region Overridden XNA Methods

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // Set up graphics device, show mouse cursor on Windows
            graphics.PreferredBackBufferWidth = 240;
            graphics.PreferredBackBufferHeight = 320;
            graphics.ApplyChanges();

            #if WINDOWS
            IsMouseVisible = true;
            #endif

            // Initialize game state to Start
            SetGameState(GameState.Start);

            // Set up placement vectors
            scoreTextPosition = new Vector2(10, 10);
            gameOverScorePos = new Vector2(10, 100);

            // Init remaining fields
            score = 0;
            finalScoreText = "";

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

            // Load in textures and fonts used in the main game loop.
            beachTex = Content.Load<Texture2D>("Textures/beachbg");
            startTex = Content.Load<Texture2D>("Textures/startscreen");
            gameOverTex = Content.Load<Texture2D>("Textures/gameover");
            smallFont = Content.Load<SpriteFont>("Fonts/Small");
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (input.NewBackPress)
                this.Exit();

            // Determine what to do based on the game state
            switch (gameState)
            {
                case GameState.Start:
                    // Wait for a button press to transition to Playing
                    if (input.NewPlayPress || input.MiddleButtonPressed)
                    {
                        SetGameState(GameState.Playing);
                    }
                    break;
                case GameState.GameOver:
                    // Display some cheeky text
                    finalScoreText = "You fed the monkey " + score + " time";
                    finalScoreText += score != 1 ? "s." : ".";

                    if (score == 0)
                        finalScoreText += "\r\nMonkey still hungry.";
                    else if (score < 5)
                        finalScoreText += "\r\nWhy tease monkey?";
                    else if (score < 15)
                        finalScoreText += "\r\nMmm, thank you.";
                    else if (score < 25)
                        finalScoreText += "\r\nYou do good!";
                    else if (score < 50)
                        finalScoreText += "\r\nWow! Monkey stuffed!";
                    else if (score < 75)
                        finalScoreText += "\r\nWicked Sick!";
                    else
                        finalScoreText += "\r\nNO MORE! I BEG!";

                    // Wait for a button press to transition back to Start
                    if (input.NewPlayPress || input.MiddleButtonPressed)
                    {
                        SetGameState(GameState.Start);
                    }
                    break;


                case GameState.Playing:
                    // Apply touch pad to banana's trajectory and check collisions
                    if (banana.Thrown)
                    {
                        banana.Adjust(input.TouchVector);

                        // Monkey fed
                        if (banana.BoundingBox.Intersects(monkey.BoundingBox))
                        {
                            score++;
                            banana.Reset();
                            tossArrow.Reset();
                            windNeedle.Randomize();
                        }

                        // Banana completely missed - game over!
                        if (banana.BoundingBox.Right < 0 || banana.BoundingBox.Left > 320
                            || banana.BoundingBox.Bottom < 0)
                        {
                            SetGameState(GameState.GameOver);
                        }
                    }

                    else
                    {
                        // Toss the banana
                        if (input.NewPlayPress)
                        {
                            tossArrow.Pause();
                            banana.Toss(tossArrow.Vector, windNeedle.Vector);
                        }
                    }
                    break;
            }                           

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();

            // Determine what to draw based on the game state.
            switch (gameState)
            {
                case GameState.Start:
                    // Just draw the background
                    spriteBatch.Draw(startTex, Vector2.Zero, Color.White);
                    break;
                case GameState.Playing:
                    // Draw the background and the score text.
                    // The components draw themselves.
                    spriteBatch.Draw(beachTex, Vector2.Zero, Color.White);
                    spriteBatch.DrawString(smallFont, "Score: " + score.ToString(), 
                        scoreTextPosition, Color.Black);
                    break;
                case GameState.GameOver:
                    // Draw the background and score text.
                    spriteBatch.Draw(gameOverTex, Vector2.Zero, Color.White);
                    spriteBatch.DrawString(smallFont, finalScoreText, 
                        gameOverScorePos, Color.White);
                    break;
            }            
            
            spriteBatch.End();

            base.Draw(gameTime);
        }

        #endregion

        #region Utility Methods

        /// <summary>
        /// This method takes care of game state transitions
        /// in a tidy fashion.
        /// </summary>
        /// <param name="state"></param>
        private void SetGameState(GameState state)
        {
            // Enable or disable all the drawable components
            // using this variable.
            bool enableComponents = false;

            switch (state)
            {
                case GameState.Start:
                    enableComponents = false;                    
                    break;
                case GameState.Playing:
                    enableComponents = true;
                    score = 0;
                    banana.Reset();
                    tossArrow.Reset();
                    windNeedle.Randomize();
                    break;
                case GameState.GameOver:
                    enableComponents = false;
                    break;
            }

            // Apply enable/disable to the Enabled and Visible properties
            // of all the DrawableGameComponents in the game.
            waterComponent.Enabled = waterComponent.Visible = enableComponents;
            windNeedle.Enabled = windNeedle.Visible = enableComponents;
            tossArrow.Enabled = tossArrow.Visible = enableComponents;
            monkey.Enabled = monkey.Visible = enableComponents;
            banana.Enabled = banana.Visible = enableComponents;

            // Set the game state.
            gameState = state;
        }

        #endregion
    }
}
