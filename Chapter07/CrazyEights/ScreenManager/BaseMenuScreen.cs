/*
 * Original code borrowed from GameStateManagement sample.
 * Modified gently to play better with the Zune by Dan Waters
 * http://blogs.msdn.com/dawate
 */

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace GameStateManager
{
    /// <summary>
    /// Base class for screens that contain a menu of options. The user can
    /// move up and down to select an entry, or cancel to back out of the screen.
    /// </summary>
    public abstract class BaseMenuScreen : BaseScreen
    {
        #region Fields

        List<MenuEntry> menuEntries = new List<MenuEntry>();
        int selectedEntry = 0;
        string menuTitle;

        // Sound Effects
        SoundEffect selectionChangedSound;
        SoundEffect selectedSound;

        /// <summary>
        /// Gets the list of menu entries, so derived classes can add
        /// or change the menu contents.
        /// </summary>
        protected IList<MenuEntry> MenuEntries
        {
            get
            {
                return menuEntries;
            }
        }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Constructor.
        /// </summary>
        public BaseMenuScreen(string menuTitle)
        {
            this.menuTitle = menuTitle;            

            TransitionOnTime = TimeSpan.FromSeconds(0.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);
        }

        #endregion

        #region Virtual Methods

        /// <summary>
        /// Handler for when the user has chosen a menu entry.
        /// </summary>
        protected virtual void OnSelectEntry(int entryIndex)
        {
            menuEntries[selectedEntry].OnSelectEntry();
        }

        #endregion

        #region BaseScreen Overrides

        /// <summary>
        /// Responds to user input, changing the selected entry and accepting
        /// or cancelling the menu.
        /// </summary>
        public override void HandleInput(InputState input)
        {
            // Move to the previous menu entry?
            if (input.MenuUp)
            {
                selectedEntry--;

                if (selectedEntry < 0)
                    selectedEntry = menuEntries.Count - 1;
                else
                    selectionChangedSound.Play();
            }

            // Move to the next menu entry?
            if (input.MenuDown)
            {
                selectedEntry++;

                if (selectedEntry >= menuEntries.Count)
                    selectedEntry = 0;
                else
                    selectionChangedSound.Play();
            }

            // Accept or cancel the menu?
            if (input.MenuSelect)
            {
                selectedSound.Play();
                OnSelectEntry(selectedEntry);
            }
        }

        public override void LoadContent()
        {
            selectedSound = ScreenManager.Content.Load<SoundEffect>("MenuSounds/SelectChanged");
            selectionChangedSound = ScreenManager.Content.Load<SoundEffect>("MenuSounds/Selected");

            base.LoadContent();
        }

        /// <summary>
        /// Updates the menu.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            // Update each nested MenuEntry object.
            for (int i = 0; i < menuEntries.Count; i++)
            {
                bool isSelected = IsActive && (i == selectedEntry);
                menuEntries[i].Update(this, isSelected, gameTime);
            }
        }

        /// <summary>
        /// Draws the menu.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {                        
            Vector2 position = new Vector2(120, 110);

            // Make the menu slide into place during transitions, using a
            // power curve to make things look more interesting (this makes
            // the movement slow down as it nears the end).
            float transitionOffset = (float)Math.Pow(TransitionPosition, 2);

            if (ScreenState == ScreenState.TransitionOn)
                position.X -= transitionOffset * 256;
            else
                position.X += transitionOffset * 512;            

            // Draw each menu entry in turn.
            for (int i = 0; i < menuEntries.Count; i++)
            {
                MenuEntry menuEntry = menuEntries[i];
                bool isSelected = IsActive && (i == selectedEntry);
                menuEntry.Draw(this, position, isSelected, gameTime);
                position.Y += menuEntry.GetHeight(this);
            }
        }

        #endregion
    }
}