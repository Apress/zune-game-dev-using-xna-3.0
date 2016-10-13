using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace OutBreak
{
    #region Public Enumerations

    public enum LastObjectBounced
    {
        Paddle,
        Top,
        Wall,
        Right,
        Block,
        None
    }

    public enum GameState
    {
        Intro,
        Ready,        
        Playing,        
        GameOver
    }

    #endregion

    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        #region Private Fields

        // Default XNA fields
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        
        // Game element textures
        private Texture2D blockTex, paddleTex, ballTex;
        
        // Background textures
        private Texture2D introBackgroundTex, darkBackgroundTex, gameOverTex;

        // Font
        private SpriteFont normalFont;
        
        // Gameplay objects
        private List<Block> blocks;
        private InputState input;
        private GameState gameState;
        private LastObjectBounced lastObjectBounced;
        private TimeSpan screenTimer;

        // Gameplay helper variables
        private int paddleSpeed;
        private int score;
        private int numLives;
        private int scoreMultiplier;
        private bool isPaused;        

        // Game math objects
        Vector2 initialBallDirection;
        Vector2 ballDirection;
        Vector2 ballPosition;
        Vector2 paddlePosition;
        
        // Text display position vectors
        Vector2 textReadyLivesPosition;
        Vector2 textCountdownPosition;
        Vector2 textFinalScorePosition;
        Vector2 textPlayingScorePosition;                        

        #endregion

        #region Public Constants

        // Gameplay constants
        public const int MAX_LIVES = 3;
        public const int BASE_SCORE = 2;
        public const int SCREEN_DISPLAY_TIME = 3;
        public const int PADDLE_SPEED = 4;
        public const int PADDLE_Y = 280;
        public const int BALL_Y = 270;
        public const int BLOCK_WIDTH = 30;
        public const int BLOCK_HEIGHT = 10;

        #endregion

        #region Constructor(s)

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            // Frame rate is 30 fps by default for Zune.
            TargetElapsedTime = TimeSpan.FromSeconds(1 / 30.0);
        }

        #endregion

        #region XNA Methods

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // Set the screen and buffer size
            graphics.PreferredBackBufferWidth = 240;
            graphics.PreferredBackBufferHeight = 320;
            graphics.ApplyChanges();

            // Initialize game objects
            input = new InputState();            
            screenTimer = new TimeSpan();
            blocks = new List<Block>();
            isPaused = false;

            initialBallDirection = new Vector2(0.5f, -7.0f);            
            paddleSpeed = PADDLE_SPEED;

            // Initialize the screen element positions
            textReadyLivesPosition = new Vector2(10, 160);
            textCountdownPosition = new Vector2(10, 220);
            textFinalScorePosition = new Vector2(120, 250);
            textPlayingScorePosition = new Vector2(10, 295);

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

            blockTex = Content.Load<Texture2D>("block");
            paddleTex = Content.Load<Texture2D>("paddle");
            ballTex = Content.Load<Texture2D>("ball");
            introBackgroundTex = Content.Load<Texture2D>("outbreak_background");
            darkBackgroundTex = Content.Load<Texture2D>("outbreak_bg_dark");
            gameOverTex = Content.Load<Texture2D>("gameover");
            normalFont = Content.Load<SpriteFont>("Normal");

            // Move to the New Game state.
            // Requires that the textures be loaded so we can center objects.
            MoveToIntroState();
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

            #if ZUNE
            isPaused = Guide.IsVisible;
            #endif

            if (!isPaused)
            {
                input.Update();
                HandleInput(gameTime);

                switch (gameState)
                {
                    case GameState.Intro:
                        // No processing for this game state
                        break;
                    case GameState.Ready:
                        if (gameTime.TotalGameTime.Subtract(screenTimer).Seconds >= SCREEN_DISPLAY_TIME)
                            gameState = GameState.Playing;
                        break;
                    case GameState.Playing:
                        HandleCollisions(gameTime);
                        ballPosition = Vector2.Add(ballPosition, ballDirection);
                        break;
                }
            }       

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();

            switch (gameState)
            {
                case GameState.Intro:
                    spriteBatch.Draw(introBackgroundTex, Vector2.Zero, Color.White);
                    break;
                case GameState.Ready:
                    spriteBatch.Draw(darkBackgroundTex, Vector2.Zero, Color.White);
                    spriteBatch.DrawString(normalFont, "Lives: " + numLives + "\r\nScore: " + score, textReadyLivesPosition, Color.White);
                    int remainingSeconds = SCREEN_DISPLAY_TIME - (gameTime.TotalGameTime.Seconds - screenTimer.Seconds) + 1;
                    spriteBatch.DrawString(normalFont, "Get Ready... " + remainingSeconds, textCountdownPosition, Color.White);
                    break;
                case GameState.Playing:
                    if (!isPaused)
                    {
                        spriteBatch.Draw(paddleTex, paddlePosition, Color.White);
                        spriteBatch.Draw(ballTex, ballPosition, Color.White);

                        foreach (Block block in blocks)
                        {
                            if (block.IsVisible)
                                spriteBatch.Draw(blockTex, block.Position, block.Color);
                        }

                        spriteBatch.DrawString(normalFont, "score: " + score + " (x" + scoreMultiplier + ")", textPlayingScorePosition, Color.White);
                    }
                    break;
                case GameState.GameOver:
                    spriteBatch.Draw(gameOverTex, Vector2.Zero, Color.White);
                    string scoreText = "Your Score: " + score;
                    Vector2 textOrigin = normalFont.MeasureString(scoreText) / 2;
                    spriteBatch.DrawString(normalFont, scoreText, textFinalScorePosition, Color.White, 0.0f, textOrigin, 1.0f, SpriteEffects.None, 0.5f);
                    break;
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }

        #endregion

        #region Helper Methods

        private void ResetBlocks()
        {
            // Random RGB color values
            Random rnd = new Random();
            float red, green, blue;
            Color randomColor;

            int numRows = 5;
            int numColumns = 8;

            int x = 0;
            int y = 0;

            blocks.Clear();
            for (int row = 0; row < numRows; row++)
            {                       
                for (int col = 0; col < numColumns; col++)
                {
                    red = (float)rnd.NextDouble();
                    green = (float)rnd.NextDouble();
                    blue = (float)rnd.NextDouble();
                    randomColor = new Color(red, green, blue);

                    blocks.Add(new Block(new Vector2(x, y), randomColor, true));

                    x += BLOCK_WIDTH;
                }
                x = 0;
                y += BLOCK_HEIGHT;
            }            
        }

        private void HandleCollisions(GameTime gameTime)
        {
            Rectangle ballBoundingBox = new Rectangle((int)ballPosition.X, (int)ballPosition.Y, ballTex.Width, ballTex.Height);
            Rectangle blockBoundingBox = new Rectangle();
            Rectangle paddleBoundingBox = new Rectangle((int)paddlePosition.X, (int)paddlePosition.Y, paddleTex.Width, paddleTex.Height);            

            float ballCenterX = 0.0f;
            float paddleCenterX = 0.0f;
            float distance = 0.0f;
            float ratio = 0.0f;

            // check collisions with blocks
            foreach (Block block in blocks)
            {
                blockBoundingBox.X = (int)block.Position.X;
                blockBoundingBox.Y = (int)block.Position.Y;
                blockBoundingBox.Width = BLOCK_WIDTH;
                blockBoundingBox.Height = BLOCK_HEIGHT;

                if (block.IsVisible)
                {
                    if (ballBoundingBox.Intersects(blockBoundingBox))
                    {
                        // the ball hit a block - increment score
                        block.IsVisible = false;
                        if (lastObjectBounced == LastObjectBounced.Block)
                            scoreMultiplier *= 2;

                        score += BASE_SCORE * scoreMultiplier;

                        // do a quick search to see if there are any blocks left
                        bool allBlocksGone = true;
                        foreach (Block b in blocks)
                        {
                            if (b.IsVisible)
                            {
                                allBlocksGone = false;
                                break;
                            }
                        }

                        if (allBlocksGone)
                        {
                            MoveToLevelComplete();
                        }
                        else
                        {
                            // reflect in Y direction
                            ballDirection = Vector2.Reflect(ballDirection, Vector2.UnitY);
                            lastObjectBounced = LastObjectBounced.Block;
                        }
                    }
                }
            }

            // check collisions with the paddle
            if (ballBoundingBox.Intersects(paddleBoundingBox))
            {
                if (lastObjectBounced != LastObjectBounced.Paddle)
                {
                    // bounce the direction vector in the Y direction
                    ballDirection = Vector2.Reflect(ballDirection, Vector2.UnitY);

                    // Determine how far from the center of the paddle the ball hit
                    ballCenterX = ballPosition.X + (float)(ballTex.Width / 2.0f);
                    paddleCenterX = paddlePosition.X + (float)(paddleTex.Width / 2.0f);
                    distance = ballCenterX - paddleCenterX;
                    ratio = distance / (paddleTex.Width / 2);

                    ballDirection.X = ballDirection.X + (ratio * 2);
                    scoreMultiplier = 1;

                    lastObjectBounced = LastObjectBounced.Paddle;
                }
            }

            // check left & right collisions
            if (ballPosition.X <= 0 || ballPosition.X >= GraphicsDevice.Viewport.Width - ballTex.Width)
            {
                if (lastObjectBounced != LastObjectBounced.Wall)
                {
                    // bounce the direction vector in the X direction
                    ballDirection = Vector2.Reflect(ballDirection, Vector2.UnitX);
                    lastObjectBounced = LastObjectBounced.Wall;
                }
            }

            // check top collision (screen boundary)
            if (ballPosition.Y <= 0)
            {
                if (lastObjectBounced != LastObjectBounced.Top)
                {
                    ballDirection = Vector2.Reflect(ballDirection, Vector2.UnitY);
                    lastObjectBounced = LastObjectBounced.Top;
                    scoreMultiplier = 1;
                }
            }

            // check bottom collision (player loses a life)
            if (ballPosition.Y >= GraphicsDevice.Viewport.Height)
            {
                numLives--;
                if (numLives < 1)
                    MoveToGameOverState();
                else
                {
                    MoveToReadyState(gameTime);
                }
            }
        }

        private void HandleInput(GameTime gameTime)
        {                        
            switch (gameState)
            {
                case GameState.Intro:
                    // wait for the play button to be pressed
                    if (input.PlayPressed)
                    {
                        MoveToReadyState(gameTime);
                    }
                    break;
                case GameState.Playing:
                    // Check Left
                    if (input.LeftIsDown)
                    {
                        // Check boundary to the left
                        if (paddlePosition.X > 0)
                            paddlePosition.X -= paddleSpeed;
                    }

                    // Check Right
                    if (input.RightIsDown)
                    {
                        if (paddlePosition.X < GraphicsDevice.Viewport.Width - paddleTex.Width)
                            paddlePosition.X += paddleSpeed;
                    }

                    // Bring up the Guide
                    #if ZUNE
                    if (input.PlayPressed)
                    {
                        Guide.Show();
                    }
                    #endif

                    break;
                case GameState.GameOver:
                    if (input.MiddleButtonPressed || input.PlayPressed)
                        MoveToIntroState();
                    break;
                default:
                    break;

            }
        }

        private void ResetPositions()
        {
            try
            {
                // Put the paddle and ball in the middle of the screen.
                int screenWidth = GraphicsDevice.Viewport.Width;
                paddlePosition = new Vector2(screenWidth / 2 - paddleTex.Width / 2, PADDLE_Y);
                ballPosition = new Vector2(screenWidth / 2 - ballTex.Width / 2, BALL_Y);
                ballDirection = initialBallDirection;
            }
            catch
            {
                throw new Exception("Don't call ResetPositions until after the content is loaded.");
            }
        }

        #endregion

        #region Game State Transitions

        private void MoveToIntroState()
        {
            score = 0;
            scoreMultiplier = 1;
            numLives = MAX_LIVES;
            ballDirection = initialBallDirection;
            lastObjectBounced = LastObjectBounced.Paddle;
            ResetBlocks();
            ResetPositions();

            gameState = GameState.Intro;
        }

        private void MoveToReadyState(GameTime gameTime)
        {
            screenTimer = gameTime.TotalGameTime;
            scoreMultiplier = 1;
            ResetPositions();
            gameState = GameState.Ready;
        }

        private void MoveToPlayingState()
        {
            ResetPositions();
            gameState = GameState.Playing;
        }

        private void MoveToGameOverState()
        {
            gameState = GameState.GameOver;
        }                              

        private void MoveToLevelComplete()
        {
            gameState = GameState.GameOver;
        }

        #endregion        
    }
}
