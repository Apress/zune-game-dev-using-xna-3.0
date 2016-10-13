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

using InputHandler;

namespace MusicVisualizer
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Texture2D noTrackTex;        
        Texture2D pixelTex;
        Texture2D waveformBox;

        InputState input; 

        Vector2 waveformPosition;
        Vector2 waveformBoxOrigin;
        Vector2 screenCenter;
        const int waveformHeight = 50;
        bool useFillDisplay = true;

        VisualizationData visData;

        float frequencyRatio = 0f;
        Color tintColor;                            

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            // Frame rate is 30 fps by default for Zune.
            TargetElapsedTime = TimeSpan.FromSeconds(1 / 30.0);

            input = new InputState(this);
            Components.Add(input);
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            MediaPlayer.IsVisualizationEnabled = true;
            visData = new VisualizationData();

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

            noTrackTex = Content.Load<Texture2D>("notrackbg");            
            pixelTex = Content.Load<Texture2D>("pixel");
            waveformBox = Content.Load<Texture2D>("waveformbox");            

            waveformBoxOrigin = new Vector2(waveformBox.Width / 2, waveformBox.Height / 2);

            screenCenter = new Vector2(
                GraphicsDevice.Viewport.Width / 2,
                GraphicsDevice.Viewport.Height / 2);

            waveformPosition = new Vector2(screenCenter.X, screenCenter.Y - waveformBox.Height / 2);

            tintColor = Color.White;
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
            if (input.NewBackPress)
            {
                // Stop the media player if it is playing
                MediaPlayer.Stop();

                // Exit the game
                this.Exit();
            }

            // Show the guide when Play is pressed
            if (input.NewPlayPress)
            {
                if (Guide.IsVisible == false)
                {
                    Guide.Show();
                }
            }

            // Change the display when middle is pressed
            if (input.MiddleButtonPressed)
            {
                useFillDisplay = !useFillDisplay;
            }

            // Update visualization data
            MediaPlayer.GetVisualizationData(visData);

            // Determine the band with the maximum frequency
            float maxValue = visData.Frequencies[0];
            int maxIndex = 0;
            for (int i = 1; i < visData.Frequencies.Count; i++)
            {
                if (visData.Frequencies[i] > maxValue)
                {
                    maxValue = visData.Frequencies[i];
                    maxIndex = i;
                }
            }

            // Get a ratio of where this index is compared to the rest of the bands
            frequencyRatio = (float)maxIndex / (float)visData.Frequencies.Count;

            // Use this ratio linearly interpolate between two colors
            tintColor = Color.Lerp(Color.DarkGreen, Color.White, frequencyRatio);         

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();

            if (MediaPlayer.State == MediaState.Stopped)
            {
                // Draw the "No current song" background.
                spriteBatch.Draw(noTrackTex, Vector2.Zero, Color.White);
            }
            else
            {
                // Draw the waveform background box, rotated at its origin
                spriteBatch.Draw(waveformBox, screenCenter, null, Color.White, 
                    0.0f, waveformBoxOrigin, 1.0f, SpriteEffects.None, 0);                             

                for (int i = 0; i < visData.Samples.Count; i++)
                {
                    // Draw the frequency bar at the top using rectangles
                    spriteBatch.Draw(pixelTex,
                        new Rectangle((int)waveformPosition.X + (int)waveformBoxOrigin.X,
                            i + (int)waveformPosition.Y,
                            (int)(visData.Frequencies[i] * waveformHeight), 1),
                            tintColor); 

                    if (useFillDisplay)
                    {
                        // Draw samples using rectangles
                        spriteBatch.Draw(pixelTex,
                            new Rectangle((int)waveformPosition.X,
                                i + (int)waveformPosition.Y,
                                (int)(visData.Samples[i] * waveformHeight),
                                1),
                                Color.White);
                    }
                    else
                    {
                        // Draw samples using "pixels"
                        spriteBatch.Draw(pixelTex,                        
                            new Vector2(waveformPosition.X + visData.Samples[i] * waveformHeight, 
                            waveformPosition.Y + i),                            

                        Color.White);
                    }                    
                }                
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
