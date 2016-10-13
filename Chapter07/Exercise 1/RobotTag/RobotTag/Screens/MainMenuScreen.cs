using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using ZuneScreenManager;

namespace RobotTag
{
    /// <summary>
    /// Displays the main menu, which only has two menu items.
    /// </summary>
    public class MainMenuScreen : MenuScreen
    {
        #region Constructor(s)

        public MainMenuScreen()
            : base("Zune Tag: Main Menu")
        {
            MenuEntry menuCreate = new MenuEntry("Create Game");
            MenuEntry menuJoin = new MenuEntry("Join Game");

            // Wire up event handlers for the Menu Item Selected events
            menuCreate.Selected +=new EventHandler<EventArgs>(MenuCreateHandler);
            menuJoin.Selected += new EventHandler<EventArgs>(MenuJoinHandler);

            // Add the menu entries to the menu
            MenuEntries.Add(menuCreate);
            MenuEntries.Add(menuJoin);
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Fires when the MenuJoin item is selected (clicked). Creates a new Network Lobby screen.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void MenuJoinHandler(object sender, EventArgs e)
        {
            ScreenManager.AddScreen(new NetworkLobby(NetworkLobbyType.Join, ScreenManager.Game.Content));
        }

        /// <summary>
        /// Fires when the MenuCreate item is selected (clicked). Creates a new Network Lobby screen.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void MenuCreateHandler(object sender, EventArgs e)
        {
            ScreenManager.AddScreen(new NetworkLobby(NetworkLobbyType.Create, ScreenManager.Game.Content));
        }

        #endregion
    }
}