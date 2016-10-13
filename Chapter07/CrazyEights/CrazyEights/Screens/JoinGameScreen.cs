using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Net;
using GameStateManager;

namespace CrazyEights
{
    public enum JoinScreenStatus
    {
        Finding,
        Joining,
        Joined,
        Error
    }

    public class JoinGameScreen : BaseScreen
    {
        #region Fields

        private Texture2D backgroundTex;
        private Texture2D listHighlightTex;        

        private string statusText;
        private int selectedSessionIndex = -1;
        private AvailableNetworkSessionCollection availableNetworkSessions = null;
        private JoinScreenStatus screenStatus = JoinScreenStatus.Finding;
        
        private Vector2 statusTextOrigin = Vector2.Zero;

        #endregion

        #region Event Handlers

        void NetworkSessionsFound(AvailableNetworkSessionCollection availableSessions)
        {
            availableNetworkSessions = availableSessions;
            if (availableNetworkSessions == null ||
                availableNetworkSessions.Count < 1)
            {
                selectedSessionIndex = -1;
                screenStatus = JoinScreenStatus.Error;
                statusText = "No sessions were found.\r\n" + 
                    "Please try again by pressing BACK.";
            }
            else
            {
                if (selectedSessionIndex == -1)
                    selectedSessionIndex = 0;
            }
        }

        void GameJoined()
        {
            screenStatus = JoinScreenStatus.Joined;
            statusText = "Game joined.";
            ScreenManager.AddScreen(new LobbyScreen());
            this.ExitScreen();
        }

        void GameJoinError(Exception ex)
        {
            statusText = "An error occured.\r\nPlease press BACK and try again.";
            screenStatus = JoinScreenStatus.Error;
        }  

        #endregion

        #region BaseScreen Overrides

        public override void Initialize()
        {            
            // Wire up events
            ScreenManager.Network.NetworkSessionsFound += 
                new NetworkManager.NetworkSessionsFoundHandler(NetworkSessionsFound);

            ScreenManager.Network.GameJoined += 
                new NetworkManager.GameJoinedHandler(GameJoined);

            ScreenManager.Network.GameJoinError += 
                new NetworkManager.GameJoinErrorHandler(GameJoinError);

            // Start looking for sessions
            ScreenManager.Network.KillSession();                        
            ScreenManager.Network.BeginGetAvailableSessions();

            base.Initialize();
        }

        public override void LoadContent()
        {
            backgroundTex = ScreenManager.Content.Load<Texture2D>
                ("Textures/Screens/joinGameScreen");
            listHighlightTex = ScreenManager.Content.Load<Texture2D>
                ("Textures/listhighlight");            

            statusText = "Please wait. Looking for games...";
            statusTextOrigin = ScreenManager.SmallFont.MeasureString(statusText) / 2;

            base.LoadContent();
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, 
            bool coveredByOtherScreen)
        {
            switch (screenStatus)
            {
                case JoinScreenStatus.Finding:
                    if (availableNetworkSessions == null ||
                        availableNetworkSessions.Count <= 0)
                    {
                        statusText = "Please wait. Looking for games...";
                        selectedSessionIndex = -1;
                    }
                    else
                    {
                        if (availableNetworkSessions.Count >= 1)
                        {
                            statusText = "Press the middle button to join.";
                        }
                    }
                    break;
                case JoinScreenStatus.Joining:
                    statusText = "Attempting to join game...";
                    break;
                case JoinScreenStatus.Joined:
                    statusText = "Game joined.";
                    break;
            }

            statusTextOrigin = ScreenManager.SmallFont.MeasureString(statusText) / 2;            

            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
        }

        public override void Draw(GameTime gameTime)
        {
            // Draw the background
            SharedSpriteBatch.Instance.Draw(backgroundTex, Vector2.Zero, Color.White);

            // Draw the network sessions
            if (availableNetworkSessions != null)
            {
                for (int sIndex = 0; sIndex < availableNetworkSessions.Count; sIndex++)
                {
                    AvailableNetworkSession session = availableNetworkSessions[sIndex];
                    DrawSessionInfo(sIndex, session.CurrentGamerCount, session.HostGamertag);
                }
            }

            // Draw the status text
            SharedSpriteBatch.Instance.DrawString(ScreenManager.SmallFont, statusText, 
                LobbyGameScreenElements.StatusMessagePosition, Color.White, 0.0f, 
                statusTextOrigin, 1.0f, SpriteEffects.None, 0.5f);
        }

        public override void HandleInput(InputState input)
        {
            // Kill the session and go back
            if (input.NewBackPress)
            {
                if (availableNetworkSessions != null)
                {
                    ScreenManager.Network.KillSession();
                    ScreenManager.RemoveScreen(this);
                }
            }

            // Scroll down in the list of sessions
            if (input.NewDownPress)
            {
                if (availableNetworkSessions != null && availableNetworkSessions.Count > 1)
                {
                    if (selectedSessionIndex < availableNetworkSessions.Count - 1)
                        selectedSessionIndex++;
                }
            }

            // Scroll down in the list of sessions
            if (input.NewUpPress)
            {
                if (selectedSessionIndex > 0)
                    selectedSessionIndex--;
            }

            // Attempt to join the selected game
            if (input.MiddleButtonPressed)
            {
                if (selectedSessionIndex >= 0 && availableNetworkSessions != null 
                    && availableNetworkSessions.Count > 0)
                {
                    screenStatus = JoinScreenStatus.Joining;
                    AvailableNetworkSession session = availableNetworkSessions[selectedSessionIndex];
                    ScreenManager.Network.JoinSession(session);
                }
            }
        }

        #endregion

        #region Private Methods

        private Vector2 GetHighlightPosition(int positionIndex)
        {
            Vector2 position = LobbyGameScreenElements.HighlightInitialPosition;
            position.Y += positionIndex * LobbyGameScreenElements.PLAYER_VERTICAL_SPACING;
            return position;
        }

        /// <summary>
        /// Draws the available network session at the specified index location.
        /// </summary>
        /// <param name="playerIndex">The index of the network session (determines order in visual list)</param>
        /// <param name="name">The player's gamertag</param>
        /// <param name="isReady">Whether or not the player is ready</param>
        private void DrawSessionInfo(int sessionIndex, int numGamers, string hostGamertag)
        {
            Vector2 namePosition = LobbyGameScreenElements.InitialTextListPosition;            
            namePosition.Y += LobbyGameScreenElements.PLAYER_VERTICAL_SPACING * sessionIndex;
            Vector2 statusPosition = LobbyGameScreenElements.InitialListStatusPosition;
            statusPosition.Y = namePosition.Y;

            string sessionStatus = numGamers.ToString() + " player";
            sessionStatus += numGamers == 1 ? "" : "s";

            Color sessionColor = sessionIndex == selectedSessionIndex ? Color.White : Color.Black;

            if (sessionIndex == selectedSessionIndex)
            {
                // draw the highlight before the text
                SharedSpriteBatch.Instance.Draw(listHighlightTex, GetHighlightPosition(sessionIndex), Color.White);
            }

            SharedSpriteBatch.Instance.DrawString(ScreenManager.SmallFont, hostGamertag, namePosition, sessionColor);
            SharedSpriteBatch.Instance.DrawString(ScreenManager.SmallFont, sessionStatus, statusPosition, sessionColor);
        }        

        #endregion  
    }
}