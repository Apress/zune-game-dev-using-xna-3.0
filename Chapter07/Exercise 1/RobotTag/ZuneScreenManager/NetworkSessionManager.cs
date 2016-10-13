/*
 * This code is used to centrally manage network communications.
 * Original author: Dan Waters
 * http://blogs.msdn.com/dawate
 */

using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Net;

namespace ZuneScreenManager
{
    /// <summary>
    /// This class is responsible for managing network state in a game.
    /// </summary>
    public static class NetworkSessionManager
    {
        #region Fields

        private static NetworkSession networkSession;
        public static PacketReader PacketReader = new PacketReader();
        public static PacketWriter PacketWriter = new PacketWriter();

        #endregion

        #region Public Static Methods

        /// <summary>
        /// Gets the underlying network session object.
        /// </summary>
        public static NetworkSession NetworkSession
        {
            get
            {                
                return networkSession;
            }
        }

        /// <summary>
        /// Creates a SystemLink network session.
        /// </summary>
        /// <param name="maxNetworkPlayers">The maximum number of network players.</param>
        public static void CreateSession(int maxNetworkPlayers)
        {
            networkSession = NetworkSession.Create(NetworkSessionType.SystemLink, 1, maxNetworkPlayers);
        }

        /// <summary>
        /// Attempts to join the first available session, if any.
        /// </summary>
        public static void JoinFirstSession()
        {
            using (AvailableNetworkSessionCollection availableSessions =
                NetworkSession.Find(NetworkSessionType.SystemLink, 1, null))
            {
                if (availableSessions.Count > 0)
                {
                    networkSession = NetworkSession.Join(availableSessions[0]);
                }
            }
        }

        /// <summary>
        /// Changes the state of the NetworkSession object to Playing, which also fires the GameStarted event.
        /// </summary>
        public static void StartGame()
        {            
            if (networkSession != null)
                networkSession.StartGame();
        }

        /// <summary>
        /// Pumps the underlying session object.
        /// </summary>
        public static void Update()
        {
            if (networkSession != null)
                networkSession.Update();
        }

        #endregion
    }
}