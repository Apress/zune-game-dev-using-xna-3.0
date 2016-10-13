using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonkeyFeeder.Components
{
    /// <summary>
    /// A drawable game component that oscillates in the X direction to
    /// simulate (poorly) waves arriving at the beach.
    /// </summary>
    public class Water : DrawableGameComponent
    {
        #region Fields

        private SpriteBatch spriteBatch;
        private Vector2 position;
        private Texture2D waterTex;

        #endregion

        #region Constructor(s)

        public Water(Game game)
            : base(game)
        {

        }

        #endregion

        #region Overridden GameComponent Methods

        public override void Initialize()
        {
            position = Vector2.Zero;
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            // Use a sine wave with an amplitude of 10 and a period of 1/1000
            position.X = 10.0f * (float)Math.Abs(
                Math.Sin(gameTime.TotalGameTime.TotalMilliseconds / 1000));

            base.Update(gameTime);
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            waterTex = Game.Content.Load<Texture2D>("Textures/water");
            base.LoadContent();
        }

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();
            spriteBatch.Draw(waterTex, position, Color.White);
            spriteBatch.End();
            base.Draw(gameTime);
        }

        #endregion
    }
}