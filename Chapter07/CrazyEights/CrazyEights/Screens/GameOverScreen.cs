using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;

using GameStateManager;

namespace CrazyEights
{
    public class GameOverScreen : BaseScreen
    {
        #region Private Fields

        // Text
        private string text;
        private Vector2 textOrigin, textPosition;

        // Content
        private Texture2D backgroundTex;
        private SoundEffect sound;

        // Flag
        private bool isWinner;        

        #endregion

        #region Constructor(s)

        public GameOverScreen(string winnerName, string myName)
            : base()
        {
            if (winnerName == myName)
            {
                isWinner = true;
                text = "You won!\r\n";
            }
            else
            {
                isWinner = false;
                text = "You lost.\r\n";
            }

            text += winnerName + " wins!";
        }

        #endregion

        #region BaseScreen Overrides

        public override void LoadContent()
        {
            backgroundTex = ScreenManager.Content.Load<Texture2D>("Textures/Screens/gameOverScreen");
            textPosition = new Vector2(120, 160);
            textOrigin = ScreenManager.SmallFont.MeasureString(text) / 2;

            if (isWinner)
                sound = ScreenManager.Content.Load<SoundEffect>("Sounds/Win");
            else
                sound = ScreenManager.Content.Load<SoundEffect>("Sounds/Lose");

            sound.Play();

            base.LoadContent();
        }

        public override void Draw(GameTime gameTime)
        {
            SharedSpriteBatch.Instance.Draw(backgroundTex, Vector2.Zero, Color.White);
            SharedSpriteBatch.Instance.DrawString(ScreenManager.SmallFont, text, 
                textPosition, Color.White, 0.0f, textOrigin, 1.0f, 
                SpriteEffects.None, 0.0f);
            base.Draw(gameTime);
        }

        public override void HandleInput(InputState input)
        {
            if (input.MiddleButtonPressed)
                ScreenManager.Game.Exit();
        }

        #endregion
    }
}