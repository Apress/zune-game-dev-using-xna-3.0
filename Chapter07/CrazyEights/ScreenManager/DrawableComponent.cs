using Microsoft.Xna.Framework;

namespace GameStateManager
{
    public class DrawableComponent : LogicComponent
    {
        public DrawableComponent(ScreenManager screenManager)
            : base(screenManager)
        {

        }

        public virtual void LoadContent() { }

        public virtual void Draw(GameTime gameTime) { }        
    }
}