using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Net;

using GameStateManager;

namespace CrazyEights
{
    public class LobbyScreen : BaseScreen
    {
        #region Fields

        private Texture2D backgroundTex;        
        private string statusText;
        private Vector2 statusTextOrigin = Vector2.Zero;

        #endregion

        #region Event Handlers

        void GameStarted(object sender, GameStartedEventArgs e)
        {
            ScreenManager.RemoveScreen(this);
            ScreenManager.AddScreen(new PlayingScreen(false));
        }

        void SessionEnded(object sender, NetworkSessionEndedEventArgs e)
        {
            ScreenManager.RemoveScreen(this);
        }

        #endregion

        #region BaseScreen Overrides

        public override void Initialize()
        {
            // Wire up events
            ScreenManager.Network.Session.GameStarted += 
                new EventHandler<GameStartedEventArgs>(GameStarted);

            ScreenManager.Network.Session.SessionEnded += 
                new EventHandler<NetworkSessionEndedEventArgs>(SessionEnded);            

            base.Initialize();
        }

        public override void LoadContent()
        {
            backgroundTex = ScreenManager.Content.Load<Texture2D>
                ("Textures/Screens/lobbyScreen");
            
            statusText = "When you're ready,\r\npress the MIDDLE button.";
            statusTextOrigin = ScreenManager.SmallFont.MeasureString(statusText) / 2;

            base.LoadContent();
        }

        public override void Draw(GameTime gameTime)
        {
            // Draw background
            SharedSpriteBatch.Instance.Draw(backgroundTex, Vector2.Zero, Color.White);

            // Draw players in session
            int playerIndex = 0;
            foreach (NetworkGamer gamer in ScreenManager.Network.Session.AllGamers)
            {
                DrawGamerInfo(playerIndex, gamer.Gamertag, gamer.IsHost, gamer.IsReady);
                playerIndex++;
            }

            // Draw status text
            SharedSpriteBatch.Instance.DrawString(ScreenManager.SmallFont, statusText, 
                LobbyGameScreenElements.StatusMessagePosition, Color.White,
                0.0f, statusTextOrigin, 1.0f, SpriteEffects.None, 0.5f);

            base.Draw(gameTime);
        }

        public override void HandleInput(InputState input)
        {
            if (input.NewBackPress)
            {
                ScreenManager.Network.KillSession();
                ScreenManager.RemoveScreen(this);
            }

            if (input.MiddleButtonPressed)
            {
                if (ScreenManager.Network.Session != null)
                {
                    try
                    {
                        ScreenManager.Network.Session.LocalGamers[0].IsReady = true;
                        statusText = "Waiting for the host to start.";
                    }
                    catch
                    {
                        this.ExitScreen();
                    }
                }
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Draws the gamer at the specified index location.
        /// </summary>
        /// <param name="playerIndex">The index of the player (determines order in visual list)</param>
        /// <param name="name">The player's gamertag</param>
        /// <param name="isHost">Whether the player is the host or not</param>
        /// <param name="isReady">Whether or not the player is ready</param>
        private void DrawGamerInfo(int playerIndex, string name, bool isHost, bool isReady)
        {
            Vector2 namePosition = LobbyGameScreenElements.InitialTextListPosition;
            namePosition.Y += LobbyGameScreenElements.PLAYER_VERTICAL_SPACING * playerIndex;
            Vector2 statusPosition = LobbyGameScreenElements.InitialListStatusPosition;
            statusPosition.Y = namePosition.Y;

            string readyStatus = isReady || isHost ? "Ready" : "Not Ready";
            Color readyColor = isReady || isHost ? Color.White : Color.LightGray;

            if (isHost)
                name += " (host)";

            SharedSpriteBatch.Instance.DrawString(ScreenManager.SmallFont, 
                name, namePosition, readyColor);

            SharedSpriteBatch.Instance.DrawString(ScreenManager.SmallFont, 
                readyStatus, statusPosition, readyColor);
        }

        #endregion 
    }
}