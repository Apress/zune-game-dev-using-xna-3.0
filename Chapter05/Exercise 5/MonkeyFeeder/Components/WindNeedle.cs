using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonkeyFeeder.Components
{
    /// <summary>
    /// Encapsulates the logic and drawing for the graphical element
    /// that shows wind direction. 
    /// </summary>
    public class WindNeedle : DrawableGameComponent
    {
        #region Fields

        // Graphics
        private SpriteFont smallFont;
        private Texture2D needleTex;
        private SpriteBatch spriteBatch;                       

        // Position and rotation
        private float rotation;
        private Vector2 direction;
        private Vector2 position;
        private Vector2 origin; 
              
        // Wind speed
        private int windStrength;
        private string windStrengthText;

        // Wind speed text position and origin
        private Vector2 strengthTextPos;
        private Vector2 strengthTextOrigin;

        Random rnd;

        #endregion

        #region Constants

        const int windMax = 16;
        const int windMin = 0;
        const float windFactor = 0.03f;
        const float drawScale = 0.1f;

        #endregion

        #region Constructor(s)

        public WindNeedle(Game game)
            : base(game)
        {

        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the vector represented by the wind chart.
        /// </summary>
        public Vector2 Vector
        {
            get
            {
                float windScale = windFactor * ((float)windStrength / (float)windMax);
                return Vector2.Multiply(direction, windScale);
            }
        }

        /// <summary>
        /// Gets the magnitude of the wind strength.
        /// </summary>
        public int WindStrength
        {
            get
            {
                return windStrength;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Randomizes the wind direction and speed.
        /// </summary>
        public void Randomize()
        {            
            // randomize 180 degrees (pi) and shift 
            // 90 degrees counterclockwise (pi/2)
            rotation = ((float)rnd.NextDouble() * MathHelper.Pi) -
                MathHelper.PiOver2;

            // calculate the vector direction using sin/cos and normalize
            // to get a unit vector of length 1
            direction.X = (float)Math.Sin(rotation);
            direction.Y = -(float)Math.Cos(rotation);
            direction.Normalize();

            // randomize wind strength
            windStrength = rnd.Next(windMin, windMax);
            windStrengthText = windStrength + " MPH";
        }

        #endregion        

        #region Overridden GameComponent Methods

        public override void Initialize()
        {
            // found these coordinates manually in graphics editor
            position = new Vector2(31, 289);
            strengthTextPos = new Vector2(31, 300);
            rnd = new Random();
            Randomize();            
            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Create sprite batch, load content, calculate origin
            spriteBatch = new SpriteBatch(GraphicsDevice);
            
            needleTex = Game.Content.Load<Texture2D>("Textures/arrow");
            smallFont = Game.Content.Load<SpriteFont>("Fonts/Small");

            origin = new Vector2(needleTex.Width / 2, needleTex.Height / 2);            

            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            // Update the text origin for centering horizontally
            strengthTextOrigin.X = smallFont.MeasureString(windStrengthText).X / 2;
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();

            // Draw the needle
            spriteBatch.Draw(needleTex, position, null, Color.Red, 
                rotation, origin, drawScale, SpriteEffects.None, 0.5f);

            // Draw the strength text (e.g. "10 MPH")
            spriteBatch.DrawString(smallFont, windStrengthText, strengthTextPos, 
                Color.Black, 0.0f, strengthTextOrigin, 1.0f, SpriteEffects.None, 0.5f);

            spriteBatch.End();
            base.Draw(gameTime);
        }

        #endregion
    }
}