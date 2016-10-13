using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Input;

using ZuneScreenManager;

namespace RobotTag
{
    /// <summary>
    /// This screen is responsible for all gameplay aspects of the game, 
    /// including the sending and receiving of network packets.
    /// </summary>
    public class PlayingScreen : GameScreen
    {
        #region Fields
        
        private const int ROBOT_MOVEMENT_DISTANCE = 3;
        private const int MAX_ROUNDS = 4;
        
        // TimeSpans used to determine scoring
        private TimeSpan localScore, remoteScore, roundTime, roundStartTime;
        
        private bool isMyTurn = false;
        private int currentRound = 1;
        string statusText = "";

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Creates a new PlayingScreen, initializes time spans and wires events
        /// </summary>
        public PlayingScreen()
        {
            // Host gets the first turn
            isMyTurn = NetworkSessionManager.NetworkSession.IsHost;
            currentRound = 1;

            // initialize all the timespans to zero
            localScore = remoteScore = roundTime = roundStartTime = TimeSpan.Zero;            
            
            // Subscribe to the GamerLeft event
            NetworkSessionManager.NetworkSession.GamerLeft += new EventHandler<GamerLeftEventArgs>(PlayerLeft);
        }

        #endregion

        #region Overridden GameScreen Implementation

        /// <summary>
        /// Gameplay update. Updates the network session, checks for collision, and handles scoring.
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="otherScreenHasFocus"></param>
        /// <param name="coveredByOtherScreen"></param>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            UpdateNetworkSession();

            if (roundStartTime == TimeSpan.Zero)
                roundStartTime = gameTime.TotalGameTime;

            // Update time
            roundTime = gameTime.TotalGameTime.Subtract(roundStartTime);

            // Check for Win state
            Robot localRobot, remoteRobot;
            localRobot = NetworkSessionManager.NetworkSession.LocalGamers[0].Tag as Robot;
            remoteRobot = NetworkSessionManager.NetworkSession.RemoteGamers[0].Tag as Robot;
            if (Robot.Collision(localRobot, remoteRobot))
            {
                // Hang just a sec to account for network latency
                System.Threading.Thread.Sleep(100);
                UpdateWinner(gameTime);
            }

            // Update text
            statusText = "Round " + currentRound.ToString() + "\r\nEvade time: " + roundTime.ToString();
            if (isMyTurn)
                statusText += "\r\nRUN AWAY!";
            else
                statusText += "\r\nCHASE THE OTHER PLAYER!";
            statusText += "\r\nYour time: " + localScore.ToString() + "\r\nOpponent time: " + remoteScore.ToString();

            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
        }

        /// <summary>
        /// Draws the robots onscreen, as well as the HUD (aka "Unsightly Looking Debug Message")
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Draw(GameTime gameTime)
        {
            // Clear to black
            ScreenManager.GraphicsDevice.Clear(Color.Black);

            ScreenManager.SpriteBatch.Begin();

            // Draw all the robots in the network session
            foreach (NetworkGamer gamer in NetworkSessionManager.NetworkSession.AllGamers)
            {
                Robot robot = gamer.Tag as Robot;
                robot.Draw(ScreenManager.SpriteBatch);
            }

            // Draw the "HUD" (lol)
            ScreenManager.SpriteBatch.DrawString(ScreenManager.Font, statusText, Vector2.Zero, Color.LightGray);

            ScreenManager.SpriteBatch.End();

            base.Draw(gameTime);
        }

        /// <summary>
        /// Moves your robot when you push Up, Down, Left, or Right
        /// </summary>
        /// <param name="input"></param>
        public override void HandleInput(InputState input)
        {
            LocalNetworkGamer gamer = NetworkSessionManager.NetworkSession.LocalGamers[0];
            Robot robot = gamer.Tag as Robot;

            if (input.IsButtonDown(Buttons.DPadLeft)
                && robot.Position.X >= robot.Bounds.Left)
            {
                robot.Move(-ROBOT_MOVEMENT_DISTANCE, 0);
            }

            if (input.IsButtonDown(Buttons.DPadRight)
                && robot.Position.X <= robot.Bounds.Right)
            {
                robot.Move(ROBOT_MOVEMENT_DISTANCE, 0);
            }

            if (input.IsButtonDown(Buttons.DPadUp)
                && robot.Position.Y >= robot.Bounds.Top)
            {
                robot.Move(0, -ROBOT_MOVEMENT_DISTANCE);
            }

            if (input.IsButtonDown(Buttons.DPadDown)
                && robot.Position.Y <= robot.Bounds.Bottom)
            {
                robot.Move(0, ROBOT_MOVEMENT_DISTANCE);
            }
        }

        #endregion                

        #region Event Handlers

        /// <summary>
        /// Fired when a gamer leaves. Transitions to the Game Over screen,
        /// signaling that the game was prematurely finished.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void PlayerLeft(object sender, GamerLeftEventArgs e)
        {
            GameOver(true);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Updates the network session. Sends the local status to the network, 
        /// then receives updates from other network gamers, 
        /// and applies those updates locally.
        /// </summary>
        private void UpdateNetworkSession()
        {
            // Update local robot and send its position to the other player
            LocalNetworkGamer localGamer = NetworkSessionManager.NetworkSession.LocalGamers[0];
            
            // Write the robot position into a network packet and send it
            Robot robot = localGamer.Tag as Robot;
            NetworkSessionManager.PacketWriter.Write(robot.Position);
            localGamer.SendData(NetworkSessionManager.PacketWriter, SendDataOptions.InOrder);

            // Pump the network session
            NetworkSessionManager.Update();

            // Receive data from the network session
            while (localGamer.IsDataAvailable)
            {
                NetworkGamer sender;
                localGamer.ReceiveData(NetworkSessionManager.PacketReader, out sender);

                if (sender.IsLocal) // skip local gamers
                    continue;

                // Get the sender's robot
                Robot remoteRobot = sender.Tag as Robot;
                remoteRobot.Position = NetworkSessionManager.PacketReader.ReadVector2();
            }
        }

        /// <summary>
        /// Occurs when there is a collision. If 4 rounds have passed, the game ends. 
        /// Otherwise, the score is accumulated, turns change, and play moves to the next round.
        /// </summary>
        /// <param name="gameTime"></param>
        private void UpdateWinner(GameTime gameTime)
        {
            if (currentRound >= MAX_ROUNDS)
            {
                GameOver(false);
            }
            else
            {
                if (isMyTurn)
                {
                    localScore = localScore.Add(roundTime);
                }
                else
                {
                    remoteScore = remoteScore.Add(roundTime);
                }

                // New round
                currentRound++;                
                
                roundStartTime = gameTime.TotalGameTime;

                Robot localRobot = NetworkSessionManager.NetworkSession.LocalGamers[0].Tag as Robot;
                Robot remoteRobot = NetworkSessionManager.NetworkSession.RemoteGamers[0].Tag as Robot;

                localRobot.ResetPosition();
                remoteRobot.ResetPosition();

                isMyTurn = !isMyTurn;
            }
        }

        /// <summary>
        /// Occurs when the game is over. 
        /// </summary>
        /// <param name="quitEarly">If QuitEarly is true, a simple message (user quit) will be displayed instead of the scoreboard.</param>
        private void GameOver(bool quitEarly)
        {
            if (!quitEarly)
            {
                // Determine who had the high score
                NetworkGamer localGamer = NetworkSessionManager.NetworkSession.LocalGamers[0];
                NetworkGamer remoteGamer = NetworkSessionManager.NetworkSession.RemoteGamers[0];

                if (localScore > remoteScore)
                    ScreenManager.AddScreen(new GameOverScreen(localGamer, remoteGamer, localScore, remoteScore));
                else
                    ScreenManager.AddScreen(new GameOverScreen(remoteGamer, localGamer, remoteScore, localScore));
            }
            else
            {
                ScreenManager.AddScreen(new GameOverScreen());
            }

            this.ExitScreen();
        }

        #endregion        
    }
}