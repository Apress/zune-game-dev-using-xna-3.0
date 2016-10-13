using System;
using System.Collections.Generic;

namespace CardLib
{
    public class Deck
    {        
        #region Fields

        private Random random;

        public List<Card> Cards
        {
            get;
            private set;
        }

        public List<Card> DiscardedCards
        {
            get;
            private set;
        }

        #endregion        

        #region Constants

        public const int CARDS_IN_DECK = 52;

        #endregion

        #region Constructor(s)

        public Deck()
        {
            Cards = new List<Card>(CARDS_IN_DECK);
            DiscardedCards = new List<Card>();
            random = new Random();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Swaps a card at one position with the card at another position.
        /// </summary>
        /// <param name="cardIndex">The index of the first card</param>
        /// <param name="randomIndex">The index of the second card</param>
        private void SwapCardsByIndex(int index1, int index2)
        {
            // Get the two cards
            Card x, y;
            x = Cards[index1];
            y = Cards[index2];

            // Swap the cards
            Cards[index1] = y;
            Cards[index2] = x;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Creates a new, properly ordered deck.
        /// </summary>
        public void ResetDeck()
        {            
            Cards.Clear();

            // For each of the four suits (which are alphabetically ordered from 1 - 4)
            for (int suitIndex = (int)Suit.Clubs;
                suitIndex <= (int)Suit.Spades; suitIndex++)
            {
                // For each possible card index
                for (int cardIndex = 1; cardIndex <= CardValue.KING; cardIndex++)
                {
                    // Add the card
                    Cards.Add(new Card((Suit)suitIndex, new CardValue(cardIndex)));
                }
            }
        }

        /// <summary>
        /// Reloads the Deck with the current pile of discarded cards,
        /// keeping the topmost discarded card in place.
        /// </summary>
        public void ReloadFromDiscarded()
        {
            if (DiscardedCards.Count == 0)
            {
                throw new Exception("No cards have been discarded; cannot reload.");
            }

            Cards.Clear();
            Cards.AddRange(DiscardedCards);            

            // Get the last discarded card
            Card newCard = DiscardedCards[DiscardedCards.Count - 1];

            // Remove that discarded card from the deck
            Cards.Remove(newCard);

            // Clear the discarded card list and re-add
            DiscardedCards.Clear();
            DiscardedCards.Add(newCard);
        }        

        /// <summary>
        /// Knuth-Fisher-Yates shuffling algorithm:
        /// http://www.codinghorror.com/blog/archives/001015.html
        /// </summary>
        public void Shuffle()
        {            
            for (int cardIndex = Cards.Count - 1; cardIndex > 0; cardIndex--)
            {
                int randomIndex = random.Next(cardIndex + 1);
                SwapCardsByIndex(cardIndex, randomIndex);
            }
        }

        /// <summary>
        /// Attempts to deal a card from the deck.
        /// </summary>
        /// <returns>A Card object dealt from the deck.</returns>
        public Card Deal()
        {
            if (Cards.Count <= 0)
                ReloadFromDiscarded();

            Card card = Cards[Cards.Count - 1];
            Cards.Remove(card);
            return card;
        }

        /// <summary>
        /// Adds a card to the discard pile.
        /// </summary>
        /// <param name="card">The card to add.</param>
        public void Discard(Card card)
        {
            DiscardedCards.Add(card);
        }        

        #endregion        
    }
}