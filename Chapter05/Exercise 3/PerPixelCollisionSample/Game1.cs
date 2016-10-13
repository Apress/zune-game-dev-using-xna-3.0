using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

namespace PerPixelCollisionSample
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Texture2D greenBallTex, otherBallTex;
        SpriteFont normalFont;
        InputState input;

        string statusText = "";

        Vector2 greenBallPos, otherBallPos;
        Color[] greenBallColorData;
        Color[] otherBallColorData;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            greenBallPos = new Vector2(100, 100);
            otherBallPos = new Vector2(150, 150);
            input = new InputState();

            graphics.PreferredBackBufferWidth = 240;
            graphics.PreferredBackBufferHeight = 320;
            graphics.ApplyChanges();

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            greenBallTex = Content.Load<Texture2D>("greenball");
            otherBallTex = Content.Load<Texture2D>("otherball");
            normalFont = Content.Load <SpriteFont>("Normal");

            greenBallColorData = new Color[greenBallTex.Width * greenBallTex.Height];
            otherBallColorData = new Color[otherBallTex.Width * otherBallTex.Height];

            greenBallTex.GetData(greenBallColorData);
            otherBallTex.GetData(otherBallColorData);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            input.Update();
            if (input.BackPressed)
                this.Exit();
            
            // Move the Other ball (face) based on user input.
            if (input.LeftIsDown)
                otherBallPos.X -= 2.0f;
            if (input.RightIsDown)
                otherBallPos.X += 2.0f;
            if (input.UpIsDown)
                otherBallPos.Y -= 2.0f;
            if (input.DownIsDown)
                otherBallPos.Y += 2.0f;

            if (CheckPerPixelCollision())
                statusText = "<YES> Collision detected!";
            else
                statusText = "<NO> No collision detected.";

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();
            spriteBatch.Draw(greenBallTex, greenBallPos, Color.White);
            spriteBatch.Draw(otherBallTex, otherBallPos, Color.White);
            spriteBatch.DrawString(normalFont, statusText, Vector2.Zero, Color.White);
            spriteBatch.End();

            base.Draw(gameTime);
        }

        private bool CheckPerPixelCollision()
        {
            // Get bounding rectangles for each object
            Rectangle greenBoundingBox = new Rectangle((int)greenBallPos.X, (int)greenBallPos.Y,
                greenBallTex.Width, greenBallTex.Height);

            Rectangle otherBoundingBox = new Rectangle((int)otherBallPos.X, (int)otherBallPos.Y,
                otherBallTex.Width, otherBallTex.Height);

            // Determine the rectangle of intersection and
            // dereference its properties for performance.
            Rectangle collisionRegion = Rectangle.Intersect(greenBoundingBox, otherBoundingBox);
            int left = collisionRegion.Left;
            int right = collisionRegion.Right;
            int top = collisionRegion.Top;
            int bottom = collisionRegion.Bottom;

            Color greenBallCurrentColor, otherBallCurrentColor;
            int greenBallColorIndex, otherBallColorIndex;
   
            // Loop horizontally through the collision region.
            for (int row = top; row < bottom; row++)
            {
                for (int column = left; column < right; column++)
                {
                    greenBallColorIndex = GetColorIndex(greenBoundingBox, row, column);
                    otherBallColorIndex = GetColorIndex(otherBoundingBox, row, column);
                    
                    greenBallCurrentColor = greenBallColorData[greenBallColorIndex];
                    otherBallCurrentColor = otherBallColorData[otherBallColorIndex];

                    if (greenBallCurrentColor.A != 0 && otherBallCurrentColor.A != 0)
                        return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Takes a bounding box, row, and column and creates a one-dimensional index.
        /// </summary>
        private int GetColorIndex(Rectangle boundingBox, int row, int column)
        {
            int index = 0;

            // How many rows down is the pixel?
            index += (row - boundingBox.Top) * boundingBox.Width;

            // How far from the left is the pixel?
            index += column - boundingBox.Left;             

            return index;            
        }
    }
}
