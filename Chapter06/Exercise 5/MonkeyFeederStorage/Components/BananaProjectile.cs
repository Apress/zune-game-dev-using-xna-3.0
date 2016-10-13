using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonkeyFeeder.Components
{
    /// <summary>
    /// This class encapsulates logic and art for a banana object.
    /// </summary>
    public class BananaProjectile : DrawableGameComponent
    {
        #region Private Fields

        // Graphics
        private SpriteBatch spriteBatch;
        private Texture2D bananaTex;    
    
        // Position, direction, bounding box and origin
        private Vector2 tossDirection;
        private Vector2 windDirection;        
        private Vector2 initialPosition;
        private Vector2 position;
        private Vector2 origin;
        private Vector2 adjustmentVector;
        private Rectangle boundingBox;

        // Rotation and scale factors for the texture
        private float rotation;
        private float scale;

        // Used to tell if the banana's been thrown
        private bool isThrown;

        #endregion

        #region Constants

        // How fast the banana rotates.
        const float rotationSpeed = 0.5f;

        // How much of an impact use of the touch pad
        // has on the direction of the banana.
        const float touchFactor = 0.03f;

        // How quickly the banana scales down.
        const float scaleRate = -0.005f;

        // How quickly the banana moves.
        const float bananaSpeed = 5.0f;

        #endregion

        #region Constructor(s)

        public BananaProjectile(Game game)
            : base(game)
        {
            
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the bounding box around the banana.
        /// </summary>
        public Rectangle BoundingBox
        {
            get
            {
                return boundingBox;
            }
        }

        /// <summary>
        /// Determines whether the banana has been thrown (true)
        /// or remains stationary (false).
        /// </summary>
        public bool Thrown
        {
            get
            {
                return isThrown;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Tells the banana it has been tossed (thrown).
        /// </summary>
        /// <param name="toss">The direction indicated by the needle</param>
        /// <param name="wind">The wind vector</param>
        public void Toss(Vector2 toss, Vector2 wind)
        {
            tossDirection = toss;
            windDirection = wind;
            position = initialPosition;
            isThrown = true;
            rotation = 0.0f;
        }

        /// <summary>
        /// Resets the banana's scale, rotation and position.
        /// </summary>
        public void Reset()
        {
            position = initialPosition;
            isThrown = false;
            rotation = 0.0f;
            scale = 1.0f;
        }

        /// <summary>
        /// Allows the main game to update this component
        /// with the current touch vector.
        /// </summary>
        /// <param name="touchVector">Obtained from Input class</param>
        public void Adjust(Vector2 touchVector)
        {
            adjustmentVector = touchVector;
        }

        #endregion

        #region Overridden GameComponent Methods

        public override void Initialize()
        {
            initialPosition = new Vector2(120, 300);
            position = initialPosition;
            boundingBox = new Rectangle();
            rotation = 0.0f;
            scale = 1.0f;
            adjustmentVector = Vector2.Zero;
            
            isThrown = false;            

            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Create the sprite batch & load content
            spriteBatch = new SpriteBatch(GraphicsDevice);
            bananaTex = Game.Content.Load<Texture2D>("Textures/banana");
            
            // Set up the bounding box, now that we know the
            // texture size.
            boundingBox.X = (int)position.X;
            boundingBox.Y = (int)position.Y;
            boundingBox.Width = bananaTex.Width;
            boundingBox.Height = bananaTex.Height;

            // Set the origin for the texture.
            origin = new Vector2(bananaTex.Width / 2, bananaTex.Height / 2);

            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            if (isThrown)
            {
                // Update the direction of the banana with the wind factor
                tossDirection += windDirection;

                // Adjust with touch, if any
                tossDirection.X += adjustmentVector.X * touchFactor;

                // Multiply by the scalar speed value
                position += tossDirection * bananaSpeed;

                // Rotate & scale
                rotation += rotationSpeed;
                scale += scaleRate;
            }

            // Update bounding box position. Width and height
            // do not need to be updated.
            boundingBox.X = (int)(position.X);
            boundingBox.Y = (int)(position.Y);

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            // Tint the banana green if touch is being used.
            Color tintColor = Color.White;
            if (adjustmentVector != Vector2.Zero && isThrown == true)
                tintColor = Color.Green;

            spriteBatch.Begin();
            spriteBatch.Draw(bananaTex, position, null, tintColor, 
                rotation, origin, scale, SpriteEffects.None, 0.5f);
            spriteBatch.End();
            base.Draw(gameTime);
        }

        #endregion        
    }
}