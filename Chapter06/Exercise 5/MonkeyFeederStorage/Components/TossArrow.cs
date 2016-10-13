using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonkeyFeeder.Components
{
    /// <summary>
    /// Encapsulates the logic and graphics for a
    /// rotating needle used to define a launch angle.
    /// </summary>
    public class TossArrow : DrawableGameComponent
    {
        #region Fields

        private SpriteBatch spriteBatch;
        private Texture2D arrowTex;

        private float rotation;
        private Vector2 position;
        private Vector2 origin;
        private int rotationDirection = 1;

        private bool isMoving = true;        

        #endregion

        #region Constants

        const float rotationSpeed = 0.05f;
        const float maxRotation = 1.0f;
        const float minRotation = -1.0f;

        #endregion

        #region Properties

        /// <summary>
        /// Gets a y-negated vector representation of the
        /// direction the arrow is pointing.
        /// </summary>
        public Vector2 Vector
        {
            get
            {
                double angle = (double)rotation;
                return new Vector2(
                    (float)Math.Sin(angle),
                    -(float)Math.Cos(angle)
                    );
            }
        }

        #endregion

        #region Constructor(s)

        public TossArrow(Game game)
            : base(game)
        {

        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Returns the arrow to its original state.
        /// </summary>
        public void Reset()
        {
            isMoving = true;
            rotation = 0.0f;
            rotationDirection = 1;
        }

        /// <summary>
        /// Stops the arrow from moving.
        /// </summary>
        public void Pause()
        {
            isMoving = false;
        }

        #endregion

        #region Overridden GameComponent Methods

        public override void Initialize()
        {
            // Initialize the default position (as calculated by
            // looking at the graphic in a graphics program)
            position = new Vector2(120, 320);
            rotation = 0.0f;                   

            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Create sprite batch, load texture, calculate origin
            spriteBatch = new SpriteBatch(GraphicsDevice);
            arrowTex = Game.Content.Load<Texture2D>("Textures/arrow");
            origin = new Vector2(arrowTex.Width / 2, arrowTex.Height / 2);

            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            if (isMoving)
            {
                // Flip the direction of the rotation if it's at either extreme
                if (rotation >= maxRotation)
                    rotationDirection = -1;

                if (rotation <= minRotation)
                    rotationDirection = 1;

                // Update rotation
                rotation += (rotationSpeed * rotationDirection);
            }
                
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();
            spriteBatch.Draw(arrowTex, position, null, Color.White, 
                rotation, origin, 0.2f, SpriteEffects.None, 0.5f);
            spriteBatch.End();
            
            base.Draw(gameTime);
        }

        #endregion
    }
}