using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using CardLib;
using GameStateManager;

namespace CrazyEights
{
    public class SuitSelectionEventArgs : EventArgs
    {
        public Suit Suit
        {
            get;
            private set;
        }

        public SuitSelectionEventArgs(Suit suit)
            : base()
        {
            Suit = suit;
        }
    }

    public class SuitSelectionMenu : BaseMenuScreen
    {
        #region Fields and Events

        MenuEntry clubsEntry;
        MenuEntry diamondsEntry;
        MenuEntry heartsEntry;
        MenuEntry spadesEntry;

        Texture2D menuBackground;

        public delegate void SuitSelectedHandler(SuitSelectionEventArgs e);
        public event SuitSelectedHandler SuitSelected;

        #endregion

        #region Constructor(s)

        public SuitSelectionMenu()
            : base("Select Suit")
        {
            
        }

        #endregion

        #region Menu Overrides

        public override void LoadContent()
        {
            menuBackground = ScreenManager.Content.Load<Texture2D>(
                "Textures/Screens/suitSelectBackground");            
            base.LoadContent();
        }

        public override void Initialize()
        {
            MenuEntries.Clear();

            clubsEntry = new MenuEntry("Clubs", ScreenManager.LargeFont);
            diamondsEntry = new MenuEntry("Diamonds", ScreenManager.LargeFont);
            heartsEntry = new MenuEntry("Hearts", ScreenManager.LargeFont);
            spadesEntry = new MenuEntry("Spades", ScreenManager.LargeFont);

            clubsEntry.Selected += new EventHandler<EventArgs>(ClubsSelected);
            diamondsEntry.Selected += new EventHandler<EventArgs>(DiamondsSelected);
            heartsEntry.Selected += new EventHandler<EventArgs>(HeartsSelected);
            spadesEntry.Selected += new EventHandler<EventArgs>(SpadesSelected);

            MenuEntries.Add(clubsEntry);
            MenuEntries.Add(diamondsEntry);
            MenuEntries.Add(heartsEntry);
            MenuEntries.Add(spadesEntry);

            base.Initialize();
        }

        public override void Draw(GameTime gameTime)
        {
            SharedSpriteBatch.Instance.Draw(menuBackground, Vector2.Zero, Color.White);
            base.Draw(gameTime);
        }

        #endregion

        #region Event Handlers

        void ClubsSelected(object sender, EventArgs e)
        {
            if (SuitSelected != null)
                SuitSelected(new SuitSelectionEventArgs(Suit.Clubs));
        }

        void DiamondsSelected(object sender, EventArgs e)
        {
            if (SuitSelected != null)
                SuitSelected(new SuitSelectionEventArgs(Suit.Diamonds));
        }

        void HeartsSelected(object sender, EventArgs e)
        {
            if (SuitSelected != null)
                SuitSelected(new SuitSelectionEventArgs(Suit.Hearts));
        }
                
        void SpadesSelected(object sender, EventArgs e)
        {
            if (SuitSelected != null)
                SuitSelected(new SuitSelectionEventArgs(Suit.Spades));
        }

        #endregion
    }
}