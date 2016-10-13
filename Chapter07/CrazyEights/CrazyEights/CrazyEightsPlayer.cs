using System.Collections.Generic;
using CardLib;

namespace CrazyEights
{
    /// <summary>
    /// A list of these exists on all peers, but only the host keeps a 
    /// "master list" of all the players.
    /// </summary>
    public class CrazyEightsPlayer
    {
        #region Fields

        public string Name
        {
            get;
            private set;
        }

        public List<Card> Cards
        {
            get;
            private set;
        }

        public bool IsHost
        {
            get;
            private set;
        }

        public bool IsMyTurn
        {
            get;
            set;
        }

        #endregion

        #region Constructor(s)

        public CrazyEightsPlayer(string name, bool isHost)
        {
            Name = name;
            IsHost = isHost;
            IsMyTurn = false;
            Cards = new List<Card>();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Adds a card to this player's hand.
        /// </summary>
        /// <param name="card">The card to add.</param>
        public void DealCard(Card card)
        {
            Cards.Add(card);
        }

        #endregion

        #region Overriden Methods

        public override bool Equals(object obj)
        {
            CrazyEightsPlayer player = obj as CrazyEightsPlayer;
            if (player == null)            
                return false;              
            
            return player.Name == this.Name;
        }

        #endregion
    }
}