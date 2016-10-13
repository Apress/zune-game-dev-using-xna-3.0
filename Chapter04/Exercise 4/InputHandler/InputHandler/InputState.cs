using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace InputHandler
{
    /// <summary>
    /// Helper for reading input from gamepad. This class tracks both
    /// the current and previous state of the Zune Pad and provides some
    /// properties to abstract specific presses.
    /// </summary>
    public class InputState
    {
        #region Fields

        public GamePadState CurrentGamePadState;
        public GamePadState LastGamePadState;
        public KeyboardState CurrentKeyboardState;
        public KeyboardState LastKeyboardState;

        #endregion

        #region Initialization


        /// <summary>
        /// Constructs a new input state.
        /// </summary>
        public InputState()
        {
            CurrentGamePadState = new GamePadState();
            LastGamePadState = new GamePadState();
            CurrentKeyboardState = new KeyboardState();
            LastKeyboardState = new KeyboardState();
        }


        #endregion

        #region Properties


        /// <summary>
        /// Checks for a Middle Button press (A by default)
        /// </summary>
        public bool MiddleButtonPressed
        {
            get
            {
                return IsNewButtonPress(Buttons.A) ||
                    IsNewKeyPress(Keys.LeftControl);
            }
        }


        /// <summary>
        /// Checks for a press of Up (DPadUp by default)
        /// </summary>
        public bool UpPressed
        {
            get
            {
                return IsNewButtonPress(Buttons.DPadUp) ||
                    IsNewKeyPress(Keys.Up);
            }
        }

        /// <summary>
        /// Checks for a press of Down (DPadDown by default)
        /// </summary>
        public bool DownPressed
        {
            get
            {
                return IsNewButtonPress(Buttons.DPadDown) ||
                    IsNewKeyPress(Keys.Down);
            }
        }

        /// <summary>
        /// Checks for a press of Right (DPadRight by default)
        /// </summary>
        public bool RightPressed
        {
            get
            {
                return IsNewButtonPress(Buttons.DPadRight) ||
                    IsNewKeyPress(Keys.Right);
            }
        }

        /// <summary>
        /// Checks for a press of Left (DPadLeft by default)
        /// </summary>
        public bool LeftPressed
        {
            get
            {
                return IsNewButtonPress(Buttons.DPadLeft) ||
                    IsNewKeyPress(Keys.Left);
            }
        }

        /// <summary>
        /// Checks for a press of the Play button (B by default)
        /// </summary>
        public bool PlayPressed
        {
            get
            {
                return IsNewButtonPress(Buttons.B) ||
                    IsNewKeyPress(Keys.LeftAlt);
            }
        }

        /// <summary>
        /// Checks for a press of the Back button
        /// </summary>
        public bool BackPressed
        {
            get
            {
                return IsNewButtonPress(Buttons.Back) ||
                    IsNewKeyPress(Keys.Back);
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
            LastKeyboardState = CurrentKeyboardState;
            CurrentKeyboardState = Keyboard.GetState();
        }


        /// <summary>
        /// Checks if a button was newly pressed during this update.
        /// </summary>
        /// <param name="button">The button to check</param>
        /// <returns>True if the button is down; false otherwise</returns>
        public bool IsNewButtonPress(Buttons button)
        {
            return (CurrentGamePadState.IsButtonDown(button) &&
                LastGamePadState.IsButtonUp(button));
        }

        /// <summary>
        /// Checks if a key was newly pressed during this update.
        /// </summary>
        /// <param name="button">The key to check</param>
        /// <returns>True if the key is down; false otherwise</returns>
        public bool IsNewKeyPress(Keys key)
        {
            return (CurrentKeyboardState.IsKeyDown(key) &&
                LastKeyboardState.IsKeyUp(key));
        }

        /// <summary>
        /// Checks if a button is pressed down.
        /// </summary>
        /// <param name="button">The button to check</param>
        /// <returns>True if the button is down; false otherwise</returns>
        public bool IsButtonDown(Buttons button)
        {
            return CurrentGamePadState.IsButtonDown(button);
        }

        /// <summary>
        /// Checks if a key is pressed down.
        /// </summary>
        /// <param name="button">The key to check</param>
        /// <returns>True if the key is down; false otherwise</returns>
        public bool IsKeyDown(Keys key)
        {
            return CurrentKeyboardState.IsKeyDown(key);
        }

        #endregion
    }
}
