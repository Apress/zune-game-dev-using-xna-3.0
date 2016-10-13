/*
 * Original code borrowed from GameStateManagement sample.
 * Modified gently to play better with the Zune by Dan Waters
 * http://blogs.msdn.com/dawate
 */

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameStateManager
{
    /// <summary>
    /// Helper class represents a single entry in a MenuScreen. By default this
    /// just draws the entry text string, but it can be customized to display menu
    /// entries in different ways. This also provides an event that will be raised
    /// when the menu entry is selected.
    /// </summary>
    public class MenuEntry
    {
        #region Fields

        private SpriteFont font;

        /// <summary>
        /// Tracks a fading selection effect on the entry.
        /// </summary>
        /// <remarks>
        /// The entries transition out of the selection effect when they are deselected.
        /// </remarks>
        private float selectionFade;

        /// <summary>
        /// Gets or sets the text of this menu entry.
        /// </summary>
        public string Text
        {
            get;
            set;
        }

        #endregion

        #region Events and Triggers

        /// <summary>
        /// Event raised when the menu entry is selected.
        /// </summary>
        public event EventHandler<EventArgs> Selected;

        /// <summary>
        /// Method for raising the Selected event.
        /// </summary>
        protected internal virtual void OnSelectEntry()
        {
            if (Selected != null)
                Selected(this, EventArgs.Empty);
        }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Constructs a new menu entry with the specified text.
        /// </summary>
        public MenuEntry(string text, SpriteFont font)
        {
            Text = text;
            this.font = font;
        }

        #endregion

        #region Public Virtual Methods

        /// <summary>
        /// Updates the menu entry.
        /// </summary>
        public virtual void Update(BaseMenuScreen screen, bool isSelected,
                                                      GameTime gameTime)
        {
            // When the menu selection changes, entries gradually fade between
            // their selected and deselected appearance, rather than instantly
            // popping to the new state.
            float fadeSpeed = (float)gameTime.ElapsedGameTime.TotalSeconds * 4;

            if (isSelected)
                selectionFade = Math.Min(selectionFade + fadeSpeed, 1);
            else
                selectionFade = Math.Max(selectionFade - fadeSpeed, 0);
        }


        /// <summary>
        /// Draws the menu entry. This can be overridden to customize the appearance.
        /// </summary>
        public virtual void Draw(BaseMenuScreen screen, Vector2 position,
                                 bool isSelected, GameTime gameTime)
        {
            // Draw the selected entry in yellow, otherwise white.
            Color color = isSelected ? Color.Yellow : Color.White;

            // Pulsate the size of the selected menu entry.
            double time = gameTime.TotalGameTime.TotalSeconds;

            float pulsate = (float)Math.Sin(time * 6) + 1;

            float scale = 1 + pulsate * 0.05f * selectionFade;

            // Modify the alpha to fade text out during transitions.
            color = new Color(color.R, color.G, color.B, screen.TransitionAlpha);

            // Draw text, centered on the middle of each line.

            Vector2 origin = font.MeasureString(Text) / 2;

            SharedSpriteBatch.Instance.DrawString(font, Text, position, color, 0,
                                   origin, scale, SpriteEffects.None, 0);
        }


        /// <summary>
        /// Queries how much space this menu entry requires.
        /// </summary>
        public virtual int GetHeight(BaseMenuScreen screen)
        {
            return font.LineSpacing;
        }


        #endregion
    }
}