using System;

namespace CardLib
{
    public class Card : IComparable
    {
        #region Fields

        public Suit Suit
        {
            get;
            private set;
        }

        public CardValue CardValue
        {
            get;
            private set;
        }

        public bool IsDefined
        {
            get;
            private set;
        }

        public bool IsShown // Not used in Crazy Eights
        {
            get;
            private set;
        }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// This method constructs a new Card object
        /// with a defined suit and value.
        /// </summary>
        /// <param name="suit">The suit of the card.</param>
        /// <param name="value">The value of the card.</param>
        public Card(Suit suit, CardValue value)
        {
            Suit = suit;
            CardValue = value;
            IsDefined = true;
            IsShown = false;
        }

        /// <summary>
        /// Constructs an undefined card.
        /// </summary>
        public Card()
        {
            Suit = Suit.Undefined;
            CardValue = null;
            IsDefined = false;
        }

        public Card(int value)
        {
            Suit = (Suit)(value / 13);
            
            int cardValue = value % 13;
            if (cardValue == 0)
                cardValue = CardValue.KING;

            CardValue = new CardValue(cardValue);
            
            IsDefined = true;
        }

        #endregion

        #region Overridden Methods and Operators

        public override string ToString()
        {
            return string.Concat(CardValue.ToString(), " of ", GetSuitName());
        }

        public static bool operator >(Card x, Card y)
        {
            return x.CardValue.Value > y.CardValue.Value;
        }

        public static bool operator <(Card x, Card y)
        {
            return x.CardValue.Value < y.CardValue.Value;
        }

        #endregion

        #region Public Methods

        public int Serialize()
        {
            int row = (int)Suit;
            return (row * 13) + (CardValue.Value % 13);
        }

        /// <summary>
        /// Gets a text-friendly version of the suit
        /// </summary>
        /// <returns></returns>
        public string GetSuitName()
        {
            switch (Suit)
            {                
                case Suit.Clubs:
                    return "Clubs";
                case Suit.Diamonds:
                    return "Diamonds";
                case Suit.Hearts:
                    return "Hearts";
                case Suit.Spades:
                    return "Spades";
                default:
                    return "Undefined";
            }
        }

        #endregion

        #region IComparable

        /// <summary>
        /// This allows cards to be compared to other cards, based on the value.
        /// </summary>
        /// <param name="value">The Card object to compare to.</param>
        /// <returns></returns>
        public int CompareTo(object value)
        {
            Card card = value as Card;

            if (card == null)
            {
                throw new ArgumentException(
                    "Can only compare Cards to other Cards.");
            }

            if (this < card)
                return -1;
            else if (this == card)
                return 0;
            else
                return 1;
        }        

        #endregion
    }
}
