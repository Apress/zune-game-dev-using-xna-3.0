using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Net;

using ZuneScreenManager;

namespace RobotTag
{
    /// <summary>
    /// This screen displays the score, or a message indicating that the game has quit prematurely due to a player quitting.
    /// </summary>
    public class GameOverScreen : GameScreen
    {
        #region Fields

        // Identifies the winner and loser
        private NetworkGamer _winner, _loser;
        
        // Stores the winning and losing score times
        private TimeSpan _winningTime, _losingTime;

        // The message to write onscreen
        private string _text = "";

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Creates a new GameOverScreen with statistics to display.
        /// </summary>
        /// <param name="winner">The winning NetworkGamer</param>
        /// <param name="loser">The losing NetworkGamer</param>
        /// <param name="winningTime">The winning TimeSpan</param>
        /// <param name="losingTime">The losing TimeSpan</param>
        public GameOverScreen(NetworkGamer winner, NetworkGamer loser, TimeSpan winningTime, TimeSpan losingTime)
        {
            // copy params to fields
            _winner = winner;
            _loser = loser;
            _winningTime = winningTime;
            _losingTime = losingTime;

            // Construct display text
            _text = _winner.Gamertag + " is the winner!\r\n"
                + _winner.Gamertag + ": " + _winningTime.ToString() + "\r\n"
                + _loser.Gamertag + ": " + _losingTime.ToString() + "\r\n"
                + "Press the middle button to quit.";
        }

        /// <summary>
        /// Creates a new GameOverScreen with a simple message.
        /// </summary>
        public GameOverScreen()
        {
            _text = "The game has ended.\r\nThe other player quit.";
        }

        #endregion

        #region Overridden GameScreen Implementation

        /// <summary>
        /// Exits the game when the middle button is pressed.
        /// </summary>
        /// <param name="input">Input State</param>
        public override void HandleInput(InputState input)
        {
            if (input.MenuSelect)
                ScreenManager.Game.Exit();

            base.HandleInput(input);
        }

        /// <summary>
        /// Draws the onscreen message.
        /// </summary>
        /// <param name="gameTime">Game Time</param>
        public override void Draw(GameTime gameTime)
        {
            ScreenManager.SpriteBatch.Begin();
            ScreenManager.SpriteBatch.DrawString(ScreenManager.Font, _text, Vector2.Zero, Color.White);
            ScreenManager.SpriteBatch.End();
            base.Draw(gameTime);
        }

        #endregion
    }
}