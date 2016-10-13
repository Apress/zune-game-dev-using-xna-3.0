using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

using ZuneScreenManager;

namespace RobotTag
{
    /// <summary>
    /// Represents a player-controlled entity in the Tag game.
    /// The Robot has some basic moves and exposes some properties about its position, 
    /// movement limits, and supports collision with other Robots.
    /// </summary>
    public class Robot
    {
        #region Fields

        // Positioning
        private Vector2 position;        
        private int screenWidth;
        private int screenHeight;
        public Rectangle bounds;               

        // Used to determine robot color and position
        private bool isHost;

        // Content
        private ContentManager content;
        private Texture2D texRobot;

        // The hue given to the robot
        private Color robotColor;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the current position of the robot
        /// </summary>
        public Vector2 Position
        {
            get
            {
                return position;
            }
            set
            {
                position = value;
            }
        }

        /// <summary>
        /// Gets or sets the limiting rectangle for this robot's movement.
        /// </summary>
        public Rectangle Bounds
        {
            get
            {
                return bounds;
            }
        }

        #endregion

        #region Constructor(s)

        /// <summary>
        /// Creates and initializes a new Robot.
        /// </summary>
        /// <param name="host">Whether this robot is tied to the Host</param>
        /// <param name="width">The screen width</param>
        /// <param name="height">The screen height</param>
        /// <param name="contentManager">Content Manager to use for loading content</param>
        public Robot(bool host, int width, int height, ContentManager contentManager)
        {
            // copy params to fields
            isHost = host;
            content = contentManager;
            screenWidth = width;
            screenHeight = height;            

            // Load the robot texture
            texRobot = content.Load<Texture2D>("Textures/robot");

            // Set the movement limiting rectangle
            bounds = new Rectangle(0, 0, screenWidth - texRobot.Width, screenHeight - texRobot.Height);

            // Move the robot to its initial position
            ResetPosition();

            // Set the robot color
            if (isHost)
                robotColor = Color.Red;
            else
                robotColor = Color.Blue;
        }

        #endregion

        #region Public Static Methods

        /// <summary>
        /// Used to determine if there is a collision between two Robots.
        /// </summary>
        /// <param name="r1">A robot</param>
        /// <param name="r2">Another robot</param>
        /// <returns>True if there is a collision; false otherwise.</returns>
        public static bool Collision(Robot r1, Robot r2)
        {
            Rectangle rect1 = new Rectangle((int)r1.position.X, (int)r1.position.Y, r1.texRobot.Width, r1.texRobot.Height);
            Rectangle rect2 = new Rectangle((int)r2.position.X, (int)r2.position.Y, r2.texRobot.Width, r2.texRobot.Height);

            return rect1.Intersects(rect2);
        }

        #endregion        

        #region Public Methods

        /// <summary>
        /// Moves the robot by a certain x and y.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void Move(int x, int y)
        {
            position.X += x;
            position.Y += y;
        }

        /// <summary>
        /// Draws the robot.
        /// </summary>
        /// <param name="spriteBatch">The SpriteBatch to use</param>
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texRobot, position, robotColor);
        }        

        /// <summary>
        /// Resets the robot position to game starting position
        /// </summary>
        public void ResetPosition()
        {
            int initialX, initialY;

            // Position the robot in the center (x)
            // This is half the screen minus half the robot texture.
            initialX = screenWidth / 2 - texRobot.Width / 2;

            if (isHost)
            {
                // The host will be at the top
                initialY = 0;
            }
            else
            {
                // Other player will be at the bottom
                initialY = screenHeight - texRobot.Height;
            }

            // Set position
            position.X = initialX;
            position.Y = initialY;
        }

        #endregion        
    }
}