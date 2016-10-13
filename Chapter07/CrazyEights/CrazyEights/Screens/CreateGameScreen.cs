using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Net;

using GameStateManager;

namespace CrazyEights
{
    public class CreateGameScreen : BaseScreen
    {
        #region Fields

        private Texture2D backgroundTex; 
       
        private string statusText = "";
        private Vector2 statusTextOrigin;

        private bool createFailed = false;
        private bool allPlayersReady = false;
        private bool atLeastTwoPlayers = false;

        #endregion

        #region Constants

        private const int MAX_PLAYERS = 5;

        #endregion

        #region Event Handlers

        void GameStarted(object sender, GameStartedEventArgs e)
        {
            ScreenManager.RemoveScreen(this);
            ScreenManager.AddScreen(new PlayingScreen(true));
        }

        #endregion

        #region BaseScreen Overrides

        public override void Initialize()
        {
            try
            {
                ScreenManager.Network.KillSession();
                ScreenManager.Network.CreateZuneSession(MAX_PLAYERS);
                ScreenManager.Network.Session.LocalGamers[0].IsReady = true;
                
                ScreenManager.Network.Session.GameStarted += 
                    new EventHandler<GameStartedEventArgs>(GameStarted);
            }
            catch (NetworkNotAvailableException)
            {
                statusText = "No network available.\r\n" +
                    "Enable Wireless on the Zune and try again.";
                createFailed = true;
            }
            catch (NetworkException)
            {
                statusText = "Session could not be created\r\n" + 
                    "due to network issues.";
                createFailed = true;
            }
            catch (Exception)
            {
                statusText = "An unexpected error occured.";
                createFailed = true;
            }

            base.Initialize();
        }

        public override void LoadContent()
        {
            backgroundTex = ScreenManager.Content.Load<Texture2D>
                ("Textures/Screens/newGameScreen");  
          
            base.LoadContent();
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, 
            bool coveredByOtherScreen)
        {           
            // Determine readiness to start the game
            allPlayersReady = true;
            foreach (NetworkGamer gamer in ScreenManager.Network.Session.AllGamers)
            {
                if (gamer.IsReady == false)
                {
                    allPlayersReady = false;
                    break;
                }
            }

            // Determine the status text
            if (ScreenManager.Network.Session.AllGamers.Count >= 2)
                atLeastTwoPlayers = true;
            else
                atLeastTwoPlayers = false;

            if (allPlayersReady && atLeastTwoPlayers)
                statusText = "READY TO START\r\nPRESS THE MIDDLE BUTTON!";
            else
                statusText = "Waiting for other players...";

            statusTextOrigin = 
                ScreenManager.SmallFont.MeasureString(statusText) / 2;


            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
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
                if (allPlayersReady && atLeastTwoPlayers)
                {
                    if (ScreenManager.Network.Session != null)
                        ScreenManager.Network.Session.StartGame();
                }
            }
        }

        public override void Draw(GameTime gameTime)
        {
            SharedSpriteBatch.Instance.Draw(backgroundTex, Vector2.Zero, Color.White);

            if (createFailed == false)
            {
                int playerIndex = 0;
                foreach (NetworkGamer gamer in ScreenManager.Network.Session.AllGamers)
                {
                    DrawGamerInfo(playerIndex, gamer.Gamertag, gamer.IsHost, gamer.IsReady);
                    playerIndex++;
                }
            }

            SharedSpriteBatch.Instance.DrawString(ScreenManager.SmallFont, statusText, 
                LobbyGameScreenElements.StatusMessagePosition, Color.White,
                0.0f, statusTextOrigin, 1.0f, SpriteEffects.None, 0.5f);

            base.Draw(gameTime);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Draws the gamer at the specified index location.
        /// </summary>
        /// <param name="playerIndex">The index of the player 
        /// (determines order in visual list)</param>
        /// <param name="name">The player's gamertag</param>
        /// <param name="isReady">Whether or not the player is ready</param>
        private void DrawGamerInfo(int playerIndex, string name, bool isHost, bool isReady)
        {
            Vector2 namePosition = LobbyGameScreenElements.InitialTextListPosition;
            namePosition.Y += LobbyGameScreenElements.PLAYER_VERTICAL_SPACING * playerIndex;
            Vector2 statusPosition = LobbyGameScreenElements.InitialListStatusPosition;
            statusPosition.Y = namePosition.Y;

            string readyStatus = isReady ? "Ready" : "Not Ready";
            Color readyColor = isReady ? Color.White : Color.LightGray;

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