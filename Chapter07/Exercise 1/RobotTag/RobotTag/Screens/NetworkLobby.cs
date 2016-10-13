using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.GamerServices;

using ZuneScreenManager;

namespace RobotTag
{
    /// <summary>
    /// A simple enumeration used to determine whether this screen 
    /// joins or creates a network session.
    /// </summary>
    public enum NetworkLobbyType
    {
        Create, Join
    }

    /// <summary>
    /// This screen is responsible for creating (if host) and joining (otherwise) 
    /// a NetworkSession and preparing the players to start playing.
    /// </summary>
    public class NetworkLobby : GameScreen
    {
        #region Fields

        private string statusText;
        
        // Needed to instantiate the Robot object
        private ContentManager contentManager; 
        
        private NetworkLobbyType lobbyType;

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Creates a new NetworkLobby screen
        /// </summary>
        /// <param name="type">Create or Join</param>
        /// <param name="content">Needed to instantiate the Robot class, 
        /// which is attached to the Tag of each joining network player.</param>
        public NetworkLobby(NetworkLobbyType type, ContentManager content)
        {
            statusText = "";
            lobbyType = type;
            contentManager = content;

            // Try to create or join the session.
            try
            {
                switch (lobbyType)
                {
                    case NetworkLobbyType.Create:
                        NetworkSessionManager.CreateSession(2);
                        break;
                    case NetworkLobbyType.Join:
                        NetworkSessionManager.JoinFirstSession();
                        break;
                }
            }
            catch (NetworkNotAvailableException)
            {
                statusText = "Error: Wireless is not enabled.";
            }
            catch
            {
                statusText = "An unknown error occured.";
            }

            // Wire network session events
            if (NetworkSessionManager.NetworkSession != null)
            {
                NetworkSessionManager.NetworkSession.GamerJoined += 
                    new EventHandler<GamerJoinedEventArgs>(GamerJoined);

                NetworkSessionManager.NetworkSession.GameStarted += 
                    new EventHandler<GameStartedEventArgs>(GameStarted);
            }
        } 

        #endregion

        #region Overridden GameScreen Implementation

        /// <summary>
        /// Called when the screen updates
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="otherScreenHasFocus"></param>
        /// <param name="coveredByOtherScreen"></param>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            // Update the network session
            NetworkSessionManager.Update();

            // Configure display
            switch (lobbyType)
            {
                case NetworkLobbyType.Create: // what the host sees
                    statusText = "Session created.\r\nPlayers in room: ";
                    statusText += GetGamerListString(NetworkSessionManager.NetworkSession.AllGamers);
                    if (NetworkSessionManager.NetworkSession.AllGamers.Count == 2)
                    {
                        statusText += "\r\n\r\nPress the middle button to start the game.";
                    }
                    break;
                case NetworkLobbyType.Join: // what the other player sees
                    if (NetworkSessionManager.NetworkSession == null)
                        statusText = "No sessions found.";
                    else
                    {
                        statusText = "Session joined.\r\nPlayers in room: ";
                        statusText += GetGamerListString(NetworkSessionManager.NetworkSession.AllGamers);
                    }
                    break;
            }

            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
        }

        /// <summary>
        /// Draws the status message onscreen.
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Draw(GameTime gameTime)
        {
            ScreenManager.SpriteBatch.Begin();
            ScreenManager.SpriteBatch.DrawString(ScreenManager.Font, statusText, 
                Vector2.Zero, Color.White);
            ScreenManager.SpriteBatch.End();

            base.Draw(gameTime);
        }

        /// <summary>
        /// Handles input. Starts the game when the host presses MenuSelect.
        /// </summary>
        /// <param name="input"></param>
        public override void HandleInput(InputState input)
        {
            if (lobbyType == NetworkLobbyType.Create) // only the host can start the game
            {
                if (NetworkSessionManager.NetworkSession != null)
                {
                    if (input.MenuSelect)
                    {
                        NetworkSessionManager.StartGame();
                    }
                }
            }
            base.HandleInput(input);
        }

        #endregion

        #region Event Handlers

        void GameStarted(object sender, GameStartedEventArgs e)
        {
            ScreenManager.AddScreen(new PlayingScreen());
        }

        void GamerJoined(object sender, GamerJoinedEventArgs e)
        {
            e.Gamer.Tag = new Robot(e.Gamer.IsHost, 240, 320, contentManager);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Helper to get a string list of the gamers in session.
        /// </summary>
        /// <param name="gamers"></param>
        /// <returns></returns>
        private string GetGamerListString(GamerCollection<NetworkGamer> gamers)
        {
            string gamerString = "";
            foreach (NetworkGamer gamer in gamers)
            {
                gamerString += "\r\n" + gamer.Gamertag;
                if (gamer.IsHost)
                    gamerString += " (host)";
            }
            return gamerString;
        }

        #endregion        
    }
}