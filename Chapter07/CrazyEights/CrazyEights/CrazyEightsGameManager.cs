using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Net;
using GameStateManager;
using CardLib;

namespace CrazyEights
{
    public class CrazyEightsGameManager : LogicComponent
    {
        #region Fields

        public CrazyEightsPlayer Me
        {
            get;
            private set;
        }

        public Deck Deck
        {
            get;
            private set;
        }

        public int TurnIndex
        {
            get;
            private set;
        }

        public static Card CurrentPlayCard
        {
            get;
            private set;
        }

        public List<CrazyEightsPlayer> Players
        {
            get;
            private set;
        }

        public static bool SuitChanged
        {
            get;
            private set;
        }

        // This field is marked private
        private static Suit chosenSuit;

        public static Suit ActiveSuit
        {
            get
            {
                if (SuitChanged)
                    return chosenSuit;
                else
                    return CurrentPlayCard.Suit;
            }
        }

        #endregion

        #region Events and Delegates

        public delegate void AllPlayersJoinedHandler();
        public delegate void AllCardsDealtHandler();
        public delegate void CardsUpdatedHandler();
        public delegate void GameWonHandler(string playerName);

        public event AllPlayersJoinedHandler AllPlayersJoined;
        public event AllCardsDealtHandler AllCardsDealt;
        public event CardsUpdatedHandler CardsUpdated;
        public event GameWonHandler GameWon;

        #endregion

        #region Constructor(s)

        public CrazyEightsGameManager(ScreenManager screenManager)
            : base(screenManager)
        {
            Players = new List<CrazyEightsPlayer>();
            Deck = new Deck();
            TurnIndex = -1;            
        }

        #endregion

        #region LogicComponent Overrides

        public override void Initialize()
        {
            NetworkMessenger.Initialize(ScreenManager);            

            Deck.ResetDeck();
            Deck.Shuffle();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Sets the common card to a discarded card.
        /// </summary>
        /// <param name="card">The card to deal.</param>
        public void Discard(Card card)
        {
            Deck.Discard(card);      
            CurrentPlayCard = Deck.DiscardedCards[Deck.DiscardedCards.Count - 1];
        }

        /// <summary>
        /// Gets a player object by name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public CrazyEightsPlayer GetPlayerByName(string name)
        {
            foreach (CrazyEightsPlayer player in Players)
            {
                if (player.Name == name)
                    return player;
            }

            throw new Exception("Player '" + name + "' not found.");
        }

        #endregion

        #region Host-side Networking Methods

        /// <summary>
        /// Starts a new round, deals all cards, 
        /// and discards the first play card.
        /// </summary>
        public void Host_NewRound()
        {
            Deck.ResetDeck();
            Deck.Shuffle();

            foreach (CrazyEightsPlayer player in Players)
            {
                for (int i = 0; i < 8; i++)
                {
                    NetworkMessenger.Host_DealCard(Deck.Deal(), player);
                }
            }

            NetworkMessenger.Host_Discard(Deck.Deal());
            NetworkMessenger.Host_ReadyToPlay();

            AdvanceTurn();
        }

        /// <summary>
        /// Sends player data.
        /// </summary>
        public void Host_SendPlayers()
        {            
            foreach (NetworkGamer gamer in ScreenManager.Network.Session.AllGamers)
            {
                NetworkMessenger.Host_SendPlayer(gamer);
            }
        }

        /// <summary>
        /// Deals a new card and sends it back to the requesting player.
        /// </summary>
        /// <param name="playerName"></param>
        public void Host_CardRequested(string playerName)
        {
            Card dealtCard = Deck.Deal();
            NetworkMessenger.Host_DealCard(dealtCard, GetPlayerByName(playerName));
        }

        #endregion

        #region Receive Data

        public void ReceiveNetworkData()
        {
            int card = -1;
            string name = "";

            foreach (LocalNetworkGamer gamer in ScreenManager.Network.Session.LocalGamers)
            {
                while (gamer.IsDataAvailable)
                {
                    NetworkGamer sender;
                    gamer.ReceiveData(ScreenManager.Network.PacketReader, out sender);

                    // Interpret the message type
                    NetworkMessageType message = 
                        (NetworkMessageType)ScreenManager.Network.PacketReader.ReadByte();

                    switch (message)
                    {
                        case NetworkMessageType.HostDealCard:
                            name = ScreenManager.Network.PacketReader.ReadString();
                            card = ScreenManager.Network.PacketReader.ReadInt32();

                            CrazyEightsPlayer player = GetPlayerByName(name);
                            if (player.Equals(Me) || Me.IsHost)
                            {
                                player.DealCard(new Card(card));
                                if (CardsUpdated != null)
                                    CardsUpdated();
                            }
                            break;
                        
                        case NetworkMessageType.HostAllCardsDealt:
                            if (AllCardsDealt != null)
                                AllCardsDealt();
                            break;

                        case NetworkMessageType.HostDiscard:
                            card = ScreenManager.Network.PacketReader.ReadInt32();
                            Discard(new Card(card));

                            if (CardsUpdated != null)
                                CardsUpdated();

                            break;

                        case NetworkMessageType.HostSetTurn:
                            int turn = ScreenManager.Network.PacketReader.ReadInt32();
                            TurnIndex = turn;

                            if (Players[turn].Equals(Me))
                            {
                                Me.IsMyTurn = true;                                
                            }
                            else                            
                                Me.IsMyTurn = false;

                            if (CardsUpdated != null)
                                CardsUpdated();

                            break;

                        case NetworkMessageType.HostSendPlayer:
                            name = ScreenManager.Network.PacketReader.ReadString();
                            bool isHost = ScreenManager.Network.PacketReader.ReadBoolean();

                            CrazyEightsPlayer newPlayer = new CrazyEightsPlayer(name, isHost);
                            Players.Add(newPlayer);
                            if (newPlayer.Name == ScreenManager.Network.Me.Gamertag)
                            {
                                this.Me = newPlayer;                                
                            }

                            if (Players.Count == ScreenManager.Network.Session.AllGamers.Count)
                            {
                                if (AllPlayersJoined != null)
                                    AllPlayersJoined();
                            }
                            break;

                        case NetworkMessageType.PlayCard:
                            card = ScreenManager.Network.PacketReader.ReadInt32();

                            CurrentPlayCard = new Card(card);
                            if (CurrentPlayCard.CardValue.Value != 8)
                                SuitChanged = false;

                            if (CardsUpdated != null)
                                CardsUpdated();

                            if (Me.IsHost)
                            {
                                Deck.Discard(CurrentPlayCard);                                
                            }

                            AdvanceTurn();

                            break;
                        case NetworkMessageType.RequestCard:
                            name = ScreenManager.Network.PacketReader.ReadString();
                            if (Me.IsHost)                            
                                Host_CardRequested(name);                                                            
                            break;

                        case NetworkMessageType.SuitChosen:
                            Suit suit = (Suit)ScreenManager.Network.PacketReader.ReadByte();
                            SuitChanged = true;
                            chosenSuit = suit;
                            if (CardsUpdated != null)
                                CardsUpdated();
                            break;

                        case NetworkMessageType.GameWon:
                            name = ScreenManager.Network.PacketReader.ReadString();
                            if (GameWon != null)
                                GameWon(name);
                            break;
                    }
                }
            }
        }

        #endregion

        #region Peer Networking Methods

        public void PlayCard(Card card)
        {
            Me.Cards.Remove(card);            
            if (Me.Cards.Count <= 0)            
                NetworkMessenger.GameWon(Me.Name);            
            else
                NetworkMessenger.PlayCard(card);
        }

        public void RequestCard()
        {
            NetworkMessenger.RequestCard(Me);            
        }

        public void ChooseSuit(Suit suit)
        {
            NetworkMessenger.SendChosenSuit(suit);
        }

        #endregion

        #region Public Static Methods

        public static bool CanPlayCard(Card chosenCard)
        {
            if (CurrentPlayCard == null)
                return false;

            if (chosenCard.Suit == ActiveSuit)
                return true;
            if (chosenCard.CardValue.Value == CurrentPlayCard.CardValue.Value)
                return true;
            if (chosenCard.CardValue.Value == 8)
                return true;

            return false;
        }

        #endregion

        #region Private Methods

        private void AdvanceTurn()
        {
            TurnIndex++;

            // This will reset TurnIndex to zero when the turn
            // is equal to Players.Count and needs to reset.
            TurnIndex = TurnIndex % Players.Count;            
            
            NetworkMessenger.Host_SetTurn(TurnIndex);
        }

        #endregion
    }
}