
/*
 * Original code borrowed from GameStateManagement sample.
 * Modified gently to play better with the Zune by Dan Waters
 * http://blogs.msdn.com/dawate
 */

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace GameStateManager
{
    /// <summary>
    /// The screen manager is a component which manages one or more GameScreen
    /// instances. It maintains a stack of screens, calls their Update and Draw
    /// methods at the appropriate times, and automatically routes input to the
    /// topmost active screen.
    /// </summary>
    /// <remarks>
    /// Taken from the GameStateManagement sample at creators.xna.com, slightly modified.
    /// </remarks>
    public class ScreenManager : DrawableGameComponent
    {
        #region Fields

        private List<BaseScreen> allScreens = new List<BaseScreen>();
        private List<BaseScreen> screensToUpdate = new List<BaseScreen>();

        private bool isInitialized;

        public InputState Input
        {
            get;
            private set;
        }

        public SpriteFont LargeFont
        {
            get;
            private set;
        }

        public SpriteFont SmallFont
        {
            get;
            private set;
        }

        public ContentManager Content
        {
            get;
            private set;
        }

        public GraphicsDevice Graphics
        {
            get;
            private set;
        }

        public NetworkManager Network
        {
            get;
            private set;
        }                

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Constructs a new screen manager component.
        /// </summary>
        public ScreenManager(Game game)
            : base(game)
        {
            this.Input = new InputState(game);
            this.Network = new NetworkManager(game);

            game.Components.Add(this.Input);
            game.Components.Add(this.Network);
        }

        #endregion

        #region GameComponenet Overrides 

        /// <summary>
        /// Initializes the screen manager component.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();

            isInitialized = true;
        }

        /// <summary>
        /// Load your graphics content.
        /// </summary>
        protected override void LoadContent()
        {
            // Load the default fonts
            Content = Game.Content;
            Graphics = Game.GraphicsDevice;            
            LargeFont = Content.Load<SpriteFont>("Fonts/KootenayLarge");
            SmallFont = Content.Load<SpriteFont>("Fonts/KootenaySmall");

            // Tell each of the screens to load their content.
            foreach (BaseScreen screen in allScreens)
            {
                screen.LoadContent();
            }
        }


        /// <summary>
        /// Unload your graphics content.
        /// </summary>
        protected override void UnloadContent()
        {
            // Tell each of the screens to unload their content.
            foreach (BaseScreen screen in allScreens)
            {
                screen.UnloadContent();
            }
        }

        /// <summary>
        /// Allows each screen to run logic.
        /// </summary>
        public override void Update(GameTime gameTime)
        {
            // Make a copy of the master screen list, to avoid confusion if
            // the process of updating one screen adds or removes others.
            screensToUpdate.Clear();
            screensToUpdate.AddRange(allScreens);

            bool otherScreenHasFocus = !Game.IsActive;
            bool coveredByOtherScreen = false;

            // Loop as long as there are screens waiting to be updated.
            while (screensToUpdate.Count > 0)
            {
                // Pop the topmost screen off the waiting list.
                BaseScreen screen = screensToUpdate[screensToUpdate.Count - 1];
                screensToUpdate.Remove(screen);

                // Update the screen.
                screen.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

                if (screen.ScreenState == ScreenState.TransitionOn ||
                    screen.ScreenState == ScreenState.Active)
                {
                    // If this is the first active screen we came across,
                    // give it a chance to handle input.
                    if (!otherScreenHasFocus)
                    {
                        screen.HandleInput(Input);

                        otherScreenHasFocus = true;
                    }

                    // If this is an active non-popup, inform any subsequent
                    // screens that they are covered by it.
                    if (!screen.IsPopup)
                        coveredByOtherScreen = true;
                }
            }
        }        


        /// <summary>
        /// Tells each screen to draw itself.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            foreach (BaseScreen screen in allScreens)
            {
                if (screen.ScreenState != ScreenState.Hidden)
                    screen.Draw(gameTime);
            }
        }


        #endregion

        #region Public Methods

        /// <summary>
        /// Adds a new screen to the screen manager.
        /// </summary>
        public void AddScreen(BaseScreen screen)
        {
            screen.ScreenManager = this;
            screen.IsExiting = false;

            screen.Initialize();

            // If we have a graphics device, tell the screen to load content.
            if (isInitialized)
            {
                screen.LoadContent();
            }

            allScreens.Add(screen);
        }


        /// <summary>
        /// Removes a screen from the screen manager. You should normally
        /// use GameScreen.ExitScreen instead of calling this directly, so
        /// the screen can gradually transition off rather than just being
        /// instantly removed.
        /// </summary>
        public void RemoveScreen(BaseScreen screen)
        {
            // If we have a graphics device, tell the screen to unload content.
            if (isInitialized)
            {
                screen.UnloadContent();
            }

            allScreens.Remove(screen);
            screensToUpdate.Remove(screen);
        }
      
        #endregion
    }
}