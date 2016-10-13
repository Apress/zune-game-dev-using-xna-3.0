using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;

namespace SharedSpriteBatch
{
    /// <summary>
    /// Thanks to Shane DeSeranno for providing the code for this class.
    /// Represents a sprite batch object that can be shared across
    /// game components. Exposes methods modeled after the SpriteBatch
    /// class's overloads of Draw and DrawString.
    /// The Begin method is called internally by this class.
    /// The End method should always be called by the main game class.
    /// </summary>
    public class SharedSpriteBatch
    {
        #region Fields

        private SpriteBatch _spriteBatch;
        private SpriteBlendMode _mode = SpriteBlendMode.None;
        private bool _hasBegun = false;
        public bool debug = true;

        private static SharedSpriteBatch instance = null;

        #endregion

        #region Properties

        public GraphicsDevice GraphicsDevice 
        { 
            get 
            { 
                return _spriteBatch.GraphicsDevice; 
            }
        }

        public static SharedSpriteBatch Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new SharedSpriteBatch();
                }
                return instance;
            }
        }

        #endregion

        #region Constructor(s)

        private SharedSpriteBatch()
        {
            
        }              

        #endregion

        #region Public Methods

        public void Initialize(Game game)
        {
            _spriteBatch = new SpriteBatch(game.GraphicsDevice);
        }

        //Should only be called by the main update loop
        public void End()
        {
            if (_hasBegun)
            {
                if (debug)
                    Debug.WriteLine("End");
                _spriteBatch.End();
                _hasBegun = false;                
            }
        }

        #endregion

        #region Private Methods

        private void Begin(SpriteBlendMode spriteBlendMode)
        {
            //Started, but changing
            if (_mode != spriteBlendMode)
            {
                End();
            }

            if (!_hasBegun)
            {
                _mode = spriteBlendMode;
                _spriteBatch.Begin(_mode);
                if (debug)
                    Debug.WriteLine("Begin: " + _mode);
                _hasBegun = true;
            }
        }

        #endregion

        #region Draw Methods

        public void Draw(SpriteBlendMode spriteBlendMode, Texture2D texture, Vector2 position, Rectangle? srcRect, Color color,
            float rotation, Vector2 origin, float scale, SpriteEffects effects, float layer)
        {
            Begin(spriteBlendMode);
            _spriteBatch.Draw(texture, position, srcRect, color, rotation, origin, scale, effects, layer);
        }

        public void Draw(Texture2D texture, Vector2 position, Rectangle? srcRect, Color color,
            float rotation, Vector2 origin, float scale, SpriteEffects effects, float layer)
        {
            Draw(SpriteBlendMode.AlphaBlend, texture, position, srcRect, color, rotation, origin, scale, effects, layer);
        }

        public void Draw(SpriteBlendMode spriteBlendMode, Texture2D texture, Vector2 position, Color color)
        {
            Begin(spriteBlendMode);
            _spriteBatch.Draw(texture, position, color);
        }

        public void Draw(Texture2D texture, Vector2 position, Color color)
        {
            Draw(SpriteBlendMode.AlphaBlend, texture, position, color);
        }

        public void Draw(Texture2D texture, Rectangle destRect, Rectangle srcRect, Color color)
        {
            Begin(SpriteBlendMode.AlphaBlend);
            _spriteBatch.Draw(texture, destRect, srcRect, color);
        }

        public void Draw(SpriteBlendMode spriteBlendMode, Texture2D texture2D, Rectangle rectangle, Color color)
        {
            Begin(spriteBlendMode);
            _spriteBatch.Draw(texture2D, rectangle, color);
        }

        public void Draw(Texture2D texture2D, Rectangle rectangle, Color color)
        {
            Draw(SpriteBlendMode.AlphaBlend, texture2D, rectangle, color);
        }

        #endregion

        #region DrawString Methods

        public void DrawString(SpriteBlendMode spriteBlendMode, SpriteFont font, string text, Vector2 vector2, Color color)
        {
            Begin(spriteBlendMode);
            _spriteBatch.DrawString(font, text, vector2, color);
        }

        public void DrawString(SpriteFont font, string text, Vector2 vector2, Color color)
        {
            DrawString(SpriteBlendMode.AlphaBlend, font, text, vector2, color);
        }

        public void DrawString(SpriteBlendMode spriteBlendMode, SpriteFont font, string menuTitle, Vector2 titlePosition, Color titleColor, float rotation, Vector2 titleOrigin, float titleScale, SpriteEffects spriteEffects, float layer)
        {
            Begin(spriteBlendMode);
            _spriteBatch.DrawString(font, menuTitle, titlePosition, titleColor, rotation, titleOrigin, titleScale, spriteEffects, layer);
        }

        public void DrawString(SpriteFont font, string menuTitle, Vector2 titlePosition, Color titleColor, float rotation, Vector2 titleOrigin, float titleScale, SpriteEffects spriteEffects, float layer)
        {
            DrawString(SpriteBlendMode.AlphaBlend, font, menuTitle, titlePosition, titleColor, rotation, titleOrigin, titleScale, spriteEffects, layer);
        }

        #endregion
    }

}