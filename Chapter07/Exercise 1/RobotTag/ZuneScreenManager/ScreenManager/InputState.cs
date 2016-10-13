/*
 * Original code borrowed from GameStateManagement sample.
 * Modified gently to play better with the Zune by Dan Waters
 * http://blogs.msdn.com/dawate
 */

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace ZuneScreenManager
{
    /// <summary>
    /// Helper for reading input from gamepad. This class tracks both
    /// the current and previous state of both input devices, and implements query
    /// properties for high level input actions such as "move up through the menu"
    /// or "pause the game".
    /// </summary>
    public class InputState
    {
        #region Fields

        public GamePadState CurrentGamePadState;
        public GamePadState LastGamePadState;

        #endregion

        #region Initialization


        /// <summary>
        /// Constructs a new input state.
        /// </summary>
        public InputState()
        {
            CurrentGamePadState = new GamePadState();
            LastGamePadState = new GamePadState();
        }


        #endregion

        #region Properties


        /// <summary>
        /// Checks for a Menu Up input action (DPadUp by default)
        /// </summary>
        public virtual bool MenuUp
        {
            get
            {
                return IsNewButtonPress(Buttons.DPadUp);
            }
        }


        /// <summary>
        /// /// Checks for a Menu Down input action (DPadDown by default)
        /// </summary>
        public virtual bool MenuDown
        {
            get
            {
                return IsNewButtonPress(Buttons.DPadDown);
            }
        }


        /// <summary>
        /// /// Checks for a Menu Select input action (A [big button down] by default)
        /// </summary>
        public virtual bool MenuSelect
        {
            get
            {
                return IsNewButtonPress(Buttons.A);
            }
        }


        /// <summary>
        /// /// Checks for a Menu Cancel input action (B [play] by default)
        /// </summary>
        public bool MenuCancel
        {
            get
            {
                return IsNewButtonPress(Buttons.B);
            }
        }


        /// <summary>
        /// /// Checks for a PauseGame input action (B by default)
        /// </summary>
        public bool PauseGame
        {
            get
            {
                return IsNewButtonPress(Buttons.B);
            }
        }


        #endregion

        #region Methods


        /// <summary>
        /// Reads the latest state of the gamepad.
        /// </summary>
        public void Update()
        {
            LastGamePadState = CurrentGamePadState;
            CurrentGamePadState = GamePad.GetState(PlayerIndex.One);
        }


        /// <summary>
        /// Checks if a button was newly pressed during this update.
        /// </summary>
        public bool IsNewButtonPress(Buttons button)
        {
            return (CurrentGamePadState.IsButtonDown(button) &&
                    LastGamePadState.IsButtonUp(button));
        }

        /// <summary>
        /// Checks if a button is pressed down.
        /// </summary>
        /// <param name="button"></param>
        /// <returns></returns>
        public bool IsButtonDown(Buttons button)
        {
            return CurrentGamePadState.IsButtonDown(button);
        }

        #endregion
    }
}