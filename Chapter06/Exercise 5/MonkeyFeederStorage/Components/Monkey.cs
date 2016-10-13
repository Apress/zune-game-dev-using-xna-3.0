using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonkeyFeeder.Components
{
    /// <summary>
    /// Represents an animated monkey.
    /// </summary>
    public class Monkey : DrawableGameComponent
    {
        #region Fields

        // Sprite batch and texture
        SpriteBatch spriteBatch;        
        Texture2D monkeyTex;

        // Position/bounding box
        Vector2 position;
        Vector2 origin;
        Rectangle boundingBox;
        
        // Animation specific fields
        Rectangle sourceRectangle;
        int currentFrame;
        const int numFrames = 5;
        const int animIntervalMilliseconds = 50;
        TimeSpan lastFrameUpdate;

        #endregion

        #region Constructor(s)

        public Monkey(Game game)
            : base(game)
        {

        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the bounding box for the monkey.
        /// </summary>
        public Rectangle BoundingBox
        {
            get
            {
                return boundingBox;
            }
        }

        #endregion       

        #region Overridden GameComponent Methods

        public override void Initialize()
        {
            // Initialize variables
            boundingBox = new Rectangle();
            lastFrameUpdate = TimeSpan.Zero;
            sourceRectangle = new Rectangle();
            currentFrame = 0;
            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Create sprite batch and load textures
            spriteBatch = new SpriteBatch(GraphicsDevice);            
            monkeyTex = Game.Content.Load<Texture2D>("Textures/monkeysheet");            

            // Calculate origin and position (Dependent on texture size)
            origin = new Vector2((monkeyTex.Width / numFrames) / 2, monkeyTex.Height / 2);
            position = new Vector2(120, monkeyTex.Height / 2 + 5);

            UpdateSourceRectangle();

            // Set up bounding box
            boundingBox.X = (int)(position.X);
            boundingBox.Y = (int)(position.Y);
            boundingBox.Width = monkeyTex.Width / numFrames;
            boundingBox.Height = monkeyTex.Height;

            // Shrink the targetable area (bounding box) of the 
            // monkey by 20px on each side
            // This makes hitting the monkey more challenging.
            boundingBox.Inflate(-20, -20);

            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            // Check to see if the frame needs to be advanced.
            if (gameTime.TotalGameTime.Subtract(lastFrameUpdate).Milliseconds >= 
                animIntervalMilliseconds)
            {                
                currentFrame++;

                // Loop if last frame
                if (currentFrame > numFrames - 1)
                    currentFrame = 0;

                UpdateSourceRectangle();
                lastFrameUpdate = gameTime.TotalGameTime;
            }
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();
            
            // Draw using current source rectangle.
            spriteBatch.Draw(monkeyTex, position, sourceRectangle, Color.White, 
                0.0f, origin, 1.0f, SpriteEffects.None, 0.5f); 
           
            spriteBatch.End();
            base.Draw(gameTime);
        }

        #endregion

        #region Utility Methods

        /// <summary>
        /// Calculates the source rectangle based on the current frame.
        /// </summary>
        private void UpdateSourceRectangle()
        {
            sourceRectangle.X = currentFrame * (monkeyTex.Width / numFrames);
            sourceRectangle.Y = 0;
            sourceRectangle.Width = monkeyTex.Width / numFrames;
            sourceRectangle.Height = monkeyTex.Height;
        }

        #endregion
    }
}