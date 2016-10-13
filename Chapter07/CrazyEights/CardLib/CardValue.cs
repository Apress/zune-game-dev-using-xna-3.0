using System;

namespace CardLib
{
    public class CardValue
    {
        #region Fields

        public string ValueText
        {
            get;
            private set;
        }

        public int Value
        {
            get;
            private set;
        } 

        #endregion

        #region Constants

        public const int ACE = 1;
        public const int JACK = 11;
        public const int QUEEN = 12;
        public const int KING = 13;

        #endregion

        #region Constructor(s)

        public CardValue(int value)
        {
            // Check the card value's range
            if (value < 1 || value > KING)
            {
                throw new ArgumentException(
                    "Card value must be between 1 and 13 inclusive.");
            }
            else
            {
                Value = value;
                switch (Value)
                {
                    case ACE:
                        ValueText = "Ace";
                        break;
                    case 2:
                        ValueText = "Two";
                        break;
                    case 3:
                        ValueText = "Three";
                        break;
                    case 4:
                        ValueText = "Four";
                        break;
                    case 5:
                        ValueText = "Five";
                        break;
                    case 6:
                        ValueText = "Six";
                        break;
                    case 7:
                        ValueText = "Seven";
                        break;
                    case 8:
                        ValueText = "Eight";
                        break;
                    case 9:
                        ValueText = "Nine";
                        break;
                    case 10:
                        ValueText = "Ten";
                        break;
                    case JACK:
                        ValueText = "Jack";
                        break;
                    case QUEEN:
                        ValueText = "Queen";
                        break;
                    case KING:
                        ValueText = "King";
                        break;
                }
            }
        }

        #endregion

        #region Overrides

        public override string ToString()
        {
            return ValueText;
        }

        #endregion
    }
}