using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using CardLib;
using GameStateManager;

namespace CrazyEights
{
    public class PlayerViewComponent : DrawableComponent
    {
        #region Fields

        public CrazyEightsPlayer Player
        {
            get;
            set;
        }

        public Card SelectedCard
        {
            get
            {
                return Player.Cards[selectedIndex];
            }
        }

        public bool HasMove
        {
            get;
            private set;
        }

        #endregion

        #region Private Variables

        // Status text
        private string statusText = "";                

        // Positioning and measurement
        private Vector2 screenCenter;
        private Vector2 currentCardPosition;
        private Vector2 statusTextPosition;
        private Vector2 statusTextOrigin;
        private int screenWidth;
        
        // Currently selected card index
        private int selectedIndex = 0;

        // Content
        private SoundEffect cardSelectedSound;
        private Texture2D deckTexture;
        private Texture2D backgroundTex;
        
        // "Suit changed" overlay textures
        private Texture2D clubTex, diamondTex, heartTex, spadeTex;
        
        private Color overlayTintColor;        

        // For card animation
        private bool animatingSelected = false;
        private float animateDistance = 0.0f;
        private float stepValue = 0.0f;                

        #endregion

        #region Constants
        
        private const int CARD_WIDTH = 44;
        private const int CARD_HEIGHT = 60;
        private const int MAX_WIDTH = 220;

        #endregion

        #region Constructor(s)

        public PlayerViewComponent(ScreenManager screenManager)
            : base(screenManager)
        {
            currentCardPosition = new Vector2(98, 48);
            overlayTintColor = new Color(1.0f, 1.0f, 1.0f, 0.8f);
        }

        #endregion                        

        #region Public Methods        

        public void SelectNextCard()
        {
            if (HasMove)
            {
                int startIndex = selectedIndex;
                selectedIndex++;

                while (selectedIndex != startIndex)
                {
                    if (selectedIndex > Player.Cards.Count - 1) // loop over
                        selectedIndex = 0;

                    if (CrazyEightsGameManager.CanPlayCard(Player.Cards[selectedIndex]))
                    {
                        SelectCard(selectedIndex);
                        HasMove = true;
                        break;
                    }

                    selectedIndex++;
                }

                // no playable card found
                if (selectedIndex == startIndex && 
                    CrazyEightsGameManager.CanPlayCard(Player.Cards[selectedIndex]) == false)
                    HasMove = false;
            }
        }

        public void SelectPreviousCard()
        {
            if (HasMove)
            {
                int startIndex = selectedIndex;
                selectedIndex--;

                while (selectedIndex != startIndex)
                {
                    if (selectedIndex < 0) // loop over
                        selectedIndex = Player.Cards.Count - 1;

                    if (CrazyEightsGameManager.CanPlayCard(Player.Cards[selectedIndex]))
                    {
                        SelectCard(selectedIndex);
                        HasMove = true;
                        break;
                    }

                    selectedIndex--;
                }

                // no playable card found
                if (selectedIndex == startIndex &&
                    CrazyEightsGameManager.CanPlayCard(Player.Cards[selectedIndex]) == false)
                    HasMove = false;
            }
        }                

        public void SelectFirstPlayableCard()
        {
            StopAnimating();

            Card card;
            bool foundCard = false;
            for (int i = 0; i < Player.Cards.Count; i++)
            {
                card = Player.Cards[i];
                if (CrazyEightsGameManager.CanPlayCard(card))
                {
                    foundCard = true;
                    HasMove = true;
                    SelectCard(i);                    
                    break;
                }
            }

            if (foundCard == false)
            {
                HasMove = false; // no playable card found 
                SelectLastCard();
            }
        }

        #endregion

        #region DrawableComponent Overrides

        public override void Update(GameTime gameTime)
        {            
            if (animatingSelected)
            {
                stepValue += (float)gameTime.ElapsedGameTime.TotalSeconds / 0.2f;
                animateDistance = MathHelper.SmoothStep(0.0f, 20.0f, stepValue);
                if (stepValue >= 1.0f)
                {
                    animatingSelected = false;
                    stepValue = 0.0f;
                }
            }

            // Update status text
            if (Player != null)
            {
                if (Player.IsMyTurn)
                {
                    if (HasMove == false)
                    {
                        statusText = "You have no playable cards.\r\n" +
                            "Press MIDDLE to draw a card.";
                    }
                    else
                        statusText = "Select a card to play.";
                }
                else
                    statusText = "Waiting for other players...";
            }

            statusTextOrigin = ScreenManager.SmallFont.MeasureString(statusText) / 2;

            base.Update(gameTime);
        }        
        
        public override void Draw(GameTime gameTime)
        {
            SharedSpriteBatch.Instance.Draw(backgroundTex, Vector2.Zero, Color.White);            

            if (Player != null)
            {            
                int width;
                int startPosition;

                if (Player.Cards.Count > 5)
                {
                    width = MAX_WIDTH;
                    startPosition = 10;
                }
                else
                {
                    width = 150;
                    startPosition = 45;
                }

                float spacing = (float)(width - (CARD_WIDTH * Player.Cards.Count)) / 
                    (Player.Cards.Count - 1);

                float depth = 0.0f;
                Vector2 destination = new Vector2();
                Card card;
                Color tintColor;

                for (int i = 0; i < Player.Cards.Count; i++)
                {
                    card = Player.Cards[i];
                    destination.X = startPosition;
                    destination.Y = 200;

                    tintColor = CrazyEightsGameManager.CanPlayCard(card) ? Color.White : Color.Gray;

                    if (i == selectedIndex && Player.IsMyTurn)
                    {
                        destination.Y = 200 - animateDistance;

                        SharedSpriteBatch.Instance.Draw(SpriteSortMode.FrontToBack, deckTexture, 
                            destination, GetSourceRect(card), tintColor, 0.0f, Vector2.Zero, 1.0f, 
                            SpriteEffects.None, 1.0f);
                    }
                    else
                    {
                        depth += 0.001f;

                        SharedSpriteBatch.Instance.Draw(SpriteSortMode.FrontToBack, deckTexture, 
                            destination, GetSourceRect(card), tintColor, 0.0f, Vector2.Zero, 1.0f, 
                            SpriteEffects.None, depth);
                    }

                    startPosition += CARD_WIDTH + (int)spacing;
                }                
            }            

            // Draw the discarded card
            if (CrazyEightsGameManager.CurrentPlayCard != null)
            {
                SharedSpriteBatch.Instance.Draw(deckTexture, currentCardPosition, 
                    GetSourceRect(CrazyEightsGameManager.CurrentPlayCard), Color.White,
                    0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, 1.0f);
            }            

            // Draw the overlay if the suit has changed
            if (CrazyEightsGameManager.SuitChanged)
            {
                switch (CrazyEightsGameManager.ActiveSuit)
                {
                    case Suit.Clubs:
                        SharedSpriteBatch.Instance.Draw(clubTex, Vector2.Zero, overlayTintColor);
                        break;
                    case Suit.Diamonds:
                        SharedSpriteBatch.Instance.Draw(diamondTex, Vector2.Zero, overlayTintColor);
                        break;
                    case Suit.Hearts:
                        SharedSpriteBatch.Instance.Draw(heartTex, Vector2.Zero, overlayTintColor);
                        break;
                    case Suit.Spades:
                        SharedSpriteBatch.Instance.Draw(spadeTex, Vector2.Zero, overlayTintColor);
                        break;
                }
            }
      
            // Draw the status text, if any          
            SharedSpriteBatch.Instance.DrawString(ScreenManager.SmallFont, statusText, statusTextPosition,
                Color.White, 0.0f, statusTextOrigin, 1.0f, SpriteEffects.None, 1.0f);

            base.Draw(gameTime);
        }

        public override void LoadContent()
        {
            // Get screen dimensions
            Viewport viewport = ScreenManager.Graphics.Viewport;
            screenCenter = new Vector2(viewport.Width / 2, viewport.Height / 2);
            screenWidth = viewport.Width;

            // Load sprite batch and textures            
            deckTexture = ScreenManager.Content.Load<Texture2D>("Textures/deck");
            backgroundTex = ScreenManager.Content.Load<Texture2D>(
                "Textures/Screens/playingBackground");

            // Load in the card backgrounds for when an eight is played
            clubTex = ScreenManager.Content.Load<Texture2D>(
                "Textures/CardBackgrounds/clubBackground");
            diamondTex = ScreenManager.Content.Load<Texture2D>(
                "Textures/CardBackgrounds/diamondBackground");
            heartTex = ScreenManager.Content.Load<Texture2D>(
                "Textures/CardBackgrounds/heartBackground");
            spadeTex = ScreenManager.Content.Load<Texture2D>(
                "Textures/CardBackgrounds/spadeBackground");

            // Initialize text positions
            statusTextPosition = new Vector2(120, 155);
            statusTextOrigin = ScreenManager.SmallFont.MeasureString(statusText) / 2;

            cardSelectedSound = ScreenManager.Content.Load<SoundEffect>(
                "Sounds/CardSelect");

            base.LoadContent();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Attempts to select the card at the specified index.
        /// </summary>
        /// <param name="index"></param>
        private void SelectCard(int index)
        {
            StopAnimating();

            if (HasMove)
            {
                if (Player.IsMyTurn)
                    cardSelectedSound.Play();
                selectedIndex = index;
                animatingSelected = true;
            }
            else
            {
                selectedIndex = 0;
                animatingSelected = false;
            }
        }

        /// <summary>
        /// Sets the selected index to the last card.
        /// </summary>
        private void SelectLastCard()
        {
            selectedIndex = Player.Cards.Count - 1;
        }

        /// <summary>
        /// Resets animation.
        /// </summary>
        private void StopAnimating()
        {
            stepValue = 0.0f;
            animateDistance = 0.0f;
            animatingSelected = false;
        }

        private Rectangle GetSourceRect(Card card)
        {
            // Only draw defined cards.
            if (!card.IsDefined)
                throw new ArgumentException("Undefined cards cannot be drawn.");

            // Define the value (subtract 1 because arrays are zero-based)
            int cardColumn = card.CardValue.Value - 1;

            // Define the suit (subtract 1 because are enum has
            // the first defined suit starting at 1, not zero)
            int cardRow = (int)card.Suit - 1;

            // Calculate the X position, in pixels.
            int x = cardColumn * CARD_WIDTH;

            // Calculate the Y position, in pixels.
            int y = cardRow * CARD_HEIGHT;

            // Create the rectangle and return it.
            return new Rectangle(x, y, CARD_WIDTH, CARD_HEIGHT);
        }

        #endregion
    }
}