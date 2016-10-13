using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using GameStateManager;

namespace CrazyEights
{
    public class MainMenuScreen : BaseMenuScreen
    {
        MenuEntry createGameEntry;
        MenuEntry joinGameEntry;
        MenuEntry quitEntry;

        Texture2D backgroundTex;

        public MainMenuScreen()
            : base("Main Menu")
        {

        }

        public override void LoadContent()
        {
            backgroundTex = ScreenManager.Content.Load<Texture2D>
                ("Textures/Screens/menuBackground");

            createGameEntry = new MenuEntry("Create Game", ScreenManager.LargeFont);
            joinGameEntry = new MenuEntry("Join Game", ScreenManager.LargeFont);
            quitEntry = new MenuEntry("Quit", ScreenManager.LargeFont);

            createGameEntry.Selected += new EventHandler<EventArgs>(CreateSelected);
            joinGameEntry.Selected += new EventHandler<EventArgs>(JoinSelected);
            quitEntry.Selected += new EventHandler<EventArgs>(QuitSelected);

            MenuEntries.Add(createGameEntry);
            MenuEntries.Add(joinGameEntry);
            MenuEntries.Add(quitEntry);

            base.LoadContent();
        }

        public override void Draw(GameTime gameTime)
        {
            SharedSpriteBatch.Instance.Draw(backgroundTex, Vector2.Zero, Color.White);
            base.Draw(gameTime);
        }

        #region Event Handlers

        void QuitSelected(object sender, EventArgs e)
        {
            ScreenManager.Game.Exit();
        }

        void JoinSelected(object sender, EventArgs e)
        {
            ScreenManager.AddScreen(new JoinGameScreen());
        }

        void CreateSelected(object sender, EventArgs e)
        {
            ScreenManager.AddScreen(new CreateGameScreen());
        }

        #endregion
    }
}