using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.GamerServices;

namespace GameStateManager
{
    public class NetworkManager : GameComponent
    {
        #region Fields

        /// <summary>
        /// Gets the packet reader object.
        /// </summary>
        public PacketReader PacketReader
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the packet writer object.
        /// </summary>
        public PacketWriter PacketWriter
        {
            get;
            private set;
        }

        public LocalNetworkGamer Me
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the network session object.
        /// </summary>
        public NetworkSession Session
        {
            get;
            private set;
        }

        #endregion

        #region Events & Delegates

        public delegate void NetworkSessionsFoundHandler(AvailableNetworkSessionCollection availableSessions);
        public delegate void GameJoinedHandler();
        public delegate void GameJoinErrorHandler(Exception ex);

        public event NetworkSessionsFoundHandler NetworkSessionsFound;
        public event GameJoinedHandler GameJoined;
        public event GameJoinErrorHandler GameJoinError;

        #endregion

        #region Constructor(s)

        public NetworkManager(Game game)
            : base(game)
        {
            game.Components.Add(new GamerServicesComponent(game));            
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Resets the network state completely.
        /// </summary>
        public void KillSession()
        {
            if (Session != null && Session.IsDisposed == false)            
                Session.Dispose();                                            

            Session = null;
        }

        /// <summary>
        /// Creates a Zune network session
        /// (SystemLink, 1 local player)
        /// </summary>
        /// <param name="maxNetworkPlayers">The maximum number of players allowed in the session.</param>
        public void CreateZuneSession(int maxNetworkPlayers)
        {
            KillSession();

            try
            {
                Session = NetworkSession.Create(NetworkSessionType.SystemLink, 1, maxNetworkPlayers);                
                Me = Session.LocalGamers[0];
            }
            catch (NetworkNotAvailableException)
            {
                throw new NetworkNotAvailableException("Zune wireless is not enabled.");
            }
            catch (NetworkException ne)
            {
                throw ne;
            }

            if (Session == null)
                throw new NetworkException("The network session could not be created.");
        }

        /// <summary>
        /// Joins the first available session.
        /// </summary>
        public void JoinFirstAvailableSession()
        {
            using (AvailableNetworkSessionCollection availableSessions =
                NetworkSession.Find(NetworkSessionType.SystemLink, 1, null))
            {
                if (availableSessions.Count > 0)
                {
                    Session = NetworkSession.Join(availableSessions[0]);
                }
                else
                {
                    throw new NetworkException("No available network sessions.");
                }
            }
        }

        /// <summary>
        /// Attempts to join an available network session.
        /// </summary>
        /// <param name="session"></param>
        public void JoinSession(AvailableNetworkSession session)
        {
            try
            {
                NetworkSession.BeginJoin(session, new AsyncCallback(GameJoinedCallback), null);                
            }
            catch (Exception ex)
            {
                if (GameJoinError != null)
                    GameJoinError(ex);
            }
        }

        /// <summary>
        /// Called when a game is joined.
        /// </summary>
        /// <param name="result"></param>
        public void GameJoinedCallback(IAsyncResult result)
        {
            try
            {
                Session = NetworkSession.EndJoin(result);
                Me = Session.LocalGamers[0];

                if (GameJoined != null)
                    GameJoined();
            }
            catch (Exception ex)
            {
                GameJoinError(ex);
            }
        }

        /// <summary>
        /// Asynchronous: Begins to discover network sessions.
        /// </summary>
        public void BeginGetAvailableSessions()
        {
            // Destroy any existing connections
            KillSession();

            NetworkSession.BeginFind(NetworkSessionType.SystemLink, 1, null, new AsyncCallback(SessionsFoundCallback), null);
        }

        /// <summary>
        /// Called when network sessions are found.
        /// </summary>
        /// <param name="result"></param>
        public void SessionsFoundCallback(IAsyncResult result)
        {
            AvailableNetworkSessionCollection availableSessions = null;
            availableSessions = NetworkSession.EndFind(result);

            if (NetworkSessionsFound != null)
                NetworkSessionsFound(availableSessions);
        }        

        /// <summary>
        /// Starts the game.
        /// </summary>
        public void StartGame()
        {
            if (Session != null && Session.IsDisposed == false)            
                Session.StartGame();            
        }

        #endregion       

        #region GameComponent Overrides

        public override void Initialize()
        {
            PacketReader = new PacketReader();
            PacketWriter = new PacketWriter();
            Session = null;

            base.Initialize();
        }

        /// <summary>
        /// GameComponent update method. Pumps the underlying session object.
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {
            if (Session != null && Session.IsDisposed == false)
                Session.Update();

            base.Update(gameTime);
        }        

        #endregion
    }
}