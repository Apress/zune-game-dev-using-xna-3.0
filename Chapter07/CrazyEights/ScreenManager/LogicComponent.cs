using Microsoft.Xna.Framework;

namespace GameStateManager
{
    public abstract class LogicComponent
    {
        #region Fields

        /// <summary>
        /// The screen manager object which this component is tied to.
        /// </summary>
        public ScreenManager ScreenManager
        {
            get;
            private set;
        }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Creates a new logic component assigned to a screen manager.
        /// </summary>
        /// <param name="screenManager"></param>
        public LogicComponent(ScreenManager screenManager)
        {
            ScreenManager = screenManager;
        }

        #endregion

        #region Overridable Methods

        public virtual void Initialize() { }

        public virtual void Update(GameTime gameTime) { }
        
        #endregion

    }
}