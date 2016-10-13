using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace InputHandler
{
    public class InputState : GameComponent
    {
        #region Private Fields

        // These fields contain current and previous
        // game pad states for the game pad & keyboard.
        private GamePadState currentGamePadState;
        private GamePadState lastGamePadState;
        private KeyboardState currentKeyboardState;
        private KeyboardState lastKeyboardState;
        
        // Touch vector fields, and Windows support
        private MouseState mouseState;
        private Vector2 windowCenter;
        private Vector2 windowSize;
        private Vector2 touchVector;        

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Creates a new input handler object.
        /// </summary>
        /// <param name="game">The Game class this component is assigned to.</param>
        public InputState(Game game)
            : base(game)
        {
            currentGamePadState = new GamePadState();
            lastGamePadState = new GamePadState();
            currentKeyboardState = new KeyboardState();
            lastKeyboardState = new KeyboardState();
            mouseState = new MouseState();
        }

        #endregion

        #region Public Utility Methods

        /// <summary>
        /// Checks if a button was newly pressed during this update.
        /// </summary>
        /// <param name="button">The button to check</param>
        /// <returns>True if the button is down; false otherwise</returns>
        public bool IsNewButtonPress(Buttons button)
        {
            return (currentGamePadState.IsButtonDown(button) &&
                lastGamePadState.IsButtonUp(button));
        }

        /// <summary>
        /// Checks if a key was newly pressed during this update.
        /// </summary>
        /// <param name="button">The key to check</param>
        /// <returns>True if the key is down; false otherwise</returns>
        public bool IsNewKeyPress(Keys key)
        {
            return (currentKeyboardState.IsKeyDown(key) &&
                lastKeyboardState.IsKeyUp(key));
        }

        /// <summary>
        /// Checks if a button is pressed down.
        /// </summary>
        /// <param name="button">The button to check</param>
        /// <returns>True if the button is down; false otherwise</returns>
        public bool IsButtonDown(Buttons button)
        {
            return currentGamePadState.IsButtonDown(button);
        }

        /// <summary>
        /// Checks if a key is pressed down.
        /// </summary>
        /// <param name="button">The key to check</param>
        /// <returns>True if the key is down; false otherwise</returns>
        public bool IsKeyDown(Keys key)
        {
            return currentKeyboardState.IsKeyDown(key);
        }

        #endregion

        #region Overridden GameComponent Methods

        public override void Initialize()
        {
            Viewport windowViewport = Game.GraphicsDevice.Viewport;
            windowSize = new Vector2(windowViewport.Width, windowViewport.Height);
            windowCenter = windowSize / 2;
            touchVector = Vector2.Zero;

            base.Initialize();
        }

        /// <summary>
        /// Called automatically by the game loop when added
        /// as a component.
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {
            // Set the previous input states to the current state.
            lastGamePadState = currentGamePadState;
            lastKeyboardState = currentKeyboardState;

            // Retrieve the current input states.
            currentGamePadState = GamePad.GetState(PlayerIndex.One);            
            currentKeyboardState = Keyboard.GetState();
            mouseState = Mouse.GetState();

            // If the mouse button is down, activate "touch" on windows
            if (mouseState.LeftButton == ButtonState.Pressed)
            {
                touchVector.X = (mouseState.X - windowCenter.X) / windowCenter.X;
                touchVector.Y = -(mouseState.Y - windowCenter.Y) / windowCenter.Y;                
            }
            else
                touchVector = Vector2.Zero;

            base.Update(gameTime);
        }        

        #endregion

        #region Properties

        public Vector2 TouchVector
        {
            get
            {
                #if ZUNE

                return currentGamePadState.ThumbSticks.Left;
                
                #endif

                #if WINDOWS

                return touchVector;

                #endif
            }
        }

        public bool MiddleButtonPressed
        {
            get
            {
                return IsNewButtonPress(Buttons.A) ||
                    IsNewKeyPress(Keys.Space);
            }
        }        

        public bool NewPlayPress
        {
            get
            {
                return IsNewButtonPress(Buttons.B) ||
                    IsNewKeyPress(Keys.Enter);
            }
        }

        public bool NewBackPress
        {
            get
            {
                return IsNewButtonPress(Buttons.Back) ||
                    IsNewKeyPress(Keys.Escape);
            }
        }

        public bool NewUpPress
        {
            get
            {
                return IsNewButtonPress(Buttons.DPadUp) ||
                    IsNewKeyPress(Keys.Up);
            }
        }

        public bool NewDownPress
        {
            get
            {
                return IsNewButtonPress(Buttons.DPadDown) ||
                    IsNewKeyPress(Keys.Down);
            }
        }        

        public bool NewRightPress
        {
            get
            {
                return IsNewButtonPress(Buttons.DPadRight) ||
                    IsNewKeyPress(Keys.Right);
            }
        }

        public bool NewLeftPress
        {
            get
            {
                return IsNewButtonPress(Buttons.DPadLeft) ||
                    IsNewKeyPress(Keys.Left);
            }
        }

        public bool UpPressed
        {
            get
            {
                return IsButtonDown(Buttons.DPadUp) ||
                    IsKeyDown(Keys.Up);
            }
        }


        public bool DownPressed
        {
            get
            {
                return IsButtonDown(Buttons.DPadDown) ||
                    IsKeyDown(Keys.Down);
            }
        }

        public bool RightPressed
        {
            get
            {
                return IsButtonDown(Buttons.DPadRight) ||
                    IsKeyDown(Keys.Right);
            }
        }

        public bool LeftPressed
        {
            get
            {
                return IsButtonDown(Buttons.DPadLeft) ||
                    IsKeyDown(Keys.Left);
            }
        }

        #endregion
    }
}
