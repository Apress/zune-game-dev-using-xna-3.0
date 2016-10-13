using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace GameStateManager
{
    /// <summary>
    /// Enum describes the screen transition state.
    /// </summary>
    public enum ScreenState
    {
        TransitionOn,
        Active,
        TransitionOff,
        Hidden,
    }


    /// <summary>
    /// Defines the base class for all screen objects.
    /// </summary>
    public abstract class BaseScreen
    {
        #region Fields                

        public List<LogicComponent> Components
        {
            get;
            private set;
        }

        /// <summary>
        /// Indicates how long the screen takes to
        /// transition on when it is activated.
        /// </summary>
        public TimeSpan TransitionOnTime
        {
            get;
            protected set;
        }

        /// <summary>
        /// Indicates how long the screen takes to
        /// transition off when it is deactivated.
        /// </summary>
        public TimeSpan TransitionOffTime
        {
            get;
            protected set;
        }


        /// <summary>
        /// Gets the current position of the screen transition, ranging
        /// from zero (fully active, no transition) to one (transitioned
        /// fully off to nothing).
        /// </summary>
        public float TransitionPosition
        {
            get;
            protected set;
        }

        /// <summary>
        /// Gets the current alpha of the screen transition, ranging
        /// from 255 (fully active, no transition) to 0 (transitioned
        /// fully off to nothing).
        /// </summary>
        public byte TransitionAlpha
        {
            get { return (byte)(255 - TransitionPosition * 255); }
        }

        protected bool otherScreenHasFocus;

        public bool IsInitialized
        {
            get;
            private set;
        }

        public bool IsContentLoaded
        {
            get;
            private set;
        }       

        /// <summary>
        /// Gets the current screen transition state.
        /// </summary>
        public ScreenState ScreenState
        {
            get;
            protected set;
        }        

        /// <summary>
        /// There are two possible reasons why a screen might be transitioning
        /// off. It could be temporarily going away to make room for another
        /// screen that is on top of it, or it could be going away for good.
        /// This property indicates whether the screen is exiting for real:
        /// if set, the screen will automatically remove itself as soon as the
        /// transition finishes.
        /// </summary>
        public bool IsExiting
        {
            get;
            protected internal set;
        }

        public bool IsPopup
        {
            get;
            protected set;
        }

        /// <summary>
        /// Checks whether this screen is active and can respond to user input.
        /// </summary>
        public bool IsActive
        {
            get
            {
                return !otherScreenHasFocus &&
                       (ScreenState == ScreenState.TransitionOn ||
                        ScreenState == ScreenState.Active);
            }
        }        

        /// <summary>
        /// Gets the manager that this screen belongs to.
        /// </summary>
        public ScreenManager ScreenManager
        {
            get;
            internal set;
        }        

        #endregion

        #region Constructor(s)

        public BaseScreen()
        {
            Components = new List<LogicComponent>();
            IsInitialized = false;
            IsContentLoaded = false;
            ScreenState = ScreenState.TransitionOn;
        }

        #endregion

        #region Public Virtual Methods

        /// <summary>
        /// Initialize content.
        /// </summary>
        public virtual void Initialize() 
        {
            if (!IsInitialized)
            {
                foreach (LogicComponent component in Components)
                {
                    component.Initialize();
                }
                IsInitialized = true;
            }
        }

        /// <summary>
        /// Load graphics content for the screen.
        /// </summary>
        public virtual void LoadContent() 
        {
            if (!IsContentLoaded)
            {
                // Initialize all components
                foreach (LogicComponent component in Components)
                {
                    DrawableComponent drawable = component as DrawableComponent;
                    if (drawable != null)                    
                        drawable.LoadContent();                    
                }
                IsContentLoaded = true;
            }
        }

        /// <summary>
        /// Unload content for the screen.
        /// </summary>
        public virtual void UnloadContent() { }

        /// <summary>
        /// Allows the screen to handle user input. Unlike Update, this method
        /// is only called when the screen is active, and not when some other
        /// screen has taken the focus.
        /// </summary>
        public virtual void HandleInput(InputState input) { }

        /// <summary>
        /// Allows the screen to run logic, such as updating the transition position.
        /// Unlike HandleInput, this method is called regardless of whether the screen
        /// is active, hidden, or in the middle of a transition.
        /// </summary>
        public virtual void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            this.otherScreenHasFocus = otherScreenHasFocus;

            if (IsExiting)
            {
                // If the screen is going away to die, it should transition off.
                ScreenState = ScreenState.TransitionOff;

                if (!UpdateTransition(gameTime, TransitionOffTime, 1))
                {
                    // When the transition finishes, remove the screen.
                    ScreenManager.RemoveScreen(this);
                }
            }
            else if (coveredByOtherScreen)
            {
                // If the screen is covered by another, it should transition off.
                if (UpdateTransition(gameTime, TransitionOffTime, 1))
                {
                    // Still busy transitioning.
                    ScreenState = ScreenState.TransitionOff;
                }
                else
                {
                    // Transition finished!
                    ScreenState = ScreenState.Hidden;
                }
            }
            else
            {
                // Otherwise the screen should transition on and become active.
                if (UpdateTransition(gameTime, TransitionOnTime, -1))
                {
                    // Still busy transitioning.
                    ScreenState = ScreenState.TransitionOn;
                }
                else
                {
                    // Transition finished!
                    ScreenState = ScreenState.Active;

                    // Update all components
                    foreach (LogicComponent component in Components)
                    {
                        component.Update(gameTime);
                    }
                }
            }
        }                

        /// <summary>
        /// This is called when the screen should draw itself.
        /// </summary>
        public virtual void Draw(GameTime gameTime) 
        {
            foreach (LogicComponent component in Components)
            {
                DrawableComponent drawable = component as DrawableComponent;
                if (drawable != null)
                    drawable.Draw(gameTime);
            }
        }


        #endregion

        #region Public Methods

        /// <summary>
        /// Tells the screen to go away. Unlike ScreenManager.RemoveScreen, which
        /// instantly kills the screen, this method respects the transition timings
        /// and will give the screen a chance to gradually transition off.
        /// </summary>
        public void ExitScreen()
        {
            if (TransitionOffTime == TimeSpan.Zero)
            {
                // If the screen has a zero transition time, remove it immediately.
                ScreenManager.RemoveScreen(this);
            }
            else
            {
                // Otherwise flag that it should transition off and then exit.
                IsExiting = true;
            }
        }


        #endregion

        #region Helper Methods

        /// <summary>
        /// Helper for updating the screen transition position.
        /// </summary>
        protected bool UpdateTransition(GameTime gameTime, TimeSpan time, int direction)
        {
            // How much should we move by?
            float transitionDelta;

            if (time == TimeSpan.Zero)
                transitionDelta = 1;
            else
                transitionDelta = (float)(gameTime.ElapsedGameTime.TotalMilliseconds /
                                          time.TotalMilliseconds);

            // Update the transition position.
            TransitionPosition += transitionDelta * direction;

            // Did we reach the end of the transition?
            if ((TransitionPosition <= 0) || (TransitionPosition >= 1))
            {
                TransitionPosition = MathHelper.Clamp(TransitionPosition, 0, 1);
                return false;
            }

            // Otherwise we are still busy transitioning.
            return true;
        }

        #endregion
    }
}
