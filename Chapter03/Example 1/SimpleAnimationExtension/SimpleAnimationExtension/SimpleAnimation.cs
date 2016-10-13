using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace SimpleAnimationExtension
{
    /// <summary>
    /// This class represents an animated texture that plays continuously.
    /// </summary>
    public class SimpleAnimation
    {
        #region Private Fields

        private int _numFrames;
        private int _animationSpeed;
        private Texture2D _texture;        
        private int _currentFrameIndex;
        private Rectangle _sourceRect;
        private TimeSpan _lastFrameUpdate;        

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the number of frames in this animation.
        /// </summary>
        public int FrameCount
        {
            get
            {
                return _numFrames;
            }
        }

        /// <summary>
        /// Gets the speed of the animation in milliseconds. This is the
        /// rate at which the animation changes frames.
        /// </summary>
        public int AnimationSpeed
        {
            get
            {
                return _animationSpeed;
            }
        }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// This method constructs a new SimpleAnimation object. It is 
        /// called by the content processor.
        /// </summary>
        /// <param name="texture">The Texture2D object to associate with 
        /// this animation.</param>
        /// <param name="numFrames">The number of frames in the 
        /// animation.</param>
        /// <param name="animationSpeed">The animation speed, in 
        /// milliseconds.</param>
        public SimpleAnimation(Texture2D texture, int numFrames, 
            int animationSpeed)            
        {
            _texture = texture;
            _numFrames = numFrames;
            _animationSpeed = animationSpeed;
            _currentFrameIndex = 0;
            _lastFrameUpdate = TimeSpan.Zero;                      

            _sourceRect = new Rectangle();
            UpdateSourceRect();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// This method should be called with each game update. It 
        /// determines how much time has passed since the last update 
        /// and updates the frame accordingly.
        /// </summary>
        /// <param name="gameTime"></param>
        public void Update(GameTime gameTime)
        {
            // check to see if we need to advance the frame  
            double timeDiff = gameTime.TotalGameTime.Subtract(
                _lastFrameUpdate).TotalMilliseconds;

            if (timeDiff >= (double)_animationSpeed)
            {
                _currentFrameIndex ++;
                if (_currentFrameIndex >= _numFrames) // loop back over
                    _currentFrameIndex = 0;

                _lastFrameUpdate = gameTime.TotalGameTime;
            }
        }

        /// <summary>
        /// This method draws the texture to a SpriteBatch object using 
        /// the current source rectangle.
        /// </summary>
        /// <param name="spriteBatch">The SpriteBatch object 
        /// to draw to.</param>
        /// <param name="position">The position where you want to draw 
        /// the object.</param>
        public void Draw(SpriteBatch spriteBatch, Vector2 position)
        {
            UpdateSourceRect();
            spriteBatch.Draw(_texture, position, _sourceRect, Color.White);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Determines the source rectangle for the texture 
        /// based on the current frame.
        /// </summary>
        private void UpdateSourceRect()
        {
            int width = _texture.Width / _numFrames;
            int height = _texture.Height;
            int frameOffset = _currentFrameIndex * width;

            _sourceRect.X = frameOffset;
            _sourceRect.Y = 0;
            _sourceRect.Width = width;
            _sourceRect.Height = height;
        }

        #endregion
    }
}
