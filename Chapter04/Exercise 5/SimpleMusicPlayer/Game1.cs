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

namespace SimpleMusicPlayer
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        MediaLibrary mediaLibrary;
        InputState input;
        Texture2D albumArtTex, noArtTex;        
        Vector2 albumArtPosition;
        int currentSongIndex = -1;      
        string regularText, boldText;
        Vector2 boldTextPosition, regularTextPosition;
        SpriteFont font, fontBold;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            // Frame rate is 30 fps by default for Zune.
            TargetElapsedTime = TimeSpan.FromSeconds(1 / 30.0);
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            input = new InputState();
            mediaLibrary = new MediaLibrary();
            MediaPlayer.IsShuffled = true;
            MediaPlayer.Volume = 1.0f;
            regularText = "";
            boldText = "";
            boldTextPosition = new Vector2(5, 5);
            regularTextPosition = new Vector2(5, 35);
           
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

            font = Content.Load<SpriteFont>("Kootenay");
            fontBold = Content.Load<SpriteFont>("KootenayBold");
            noArtTex = Content.Load<Texture2D>("NoArt");
            albumArtTex = noArtTex;
            RefreshAlbumArt();
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            MediaPlayer.Stop();
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            input.Update();

            if (input.PlayPressed)
            {
                switch (MediaPlayer.State)
                {
                    case MediaState.Paused:
                        MediaPlayer.Resume();
                        break;                    
                    case MediaState.Playing:
                        MediaPlayer.Pause();
                        break;
                    case MediaState.Stopped:
                        PlayNextSong();                        
                        break;
                }
            }

            if (input.RightPressed)
            {
                PlayNextSong();                
            }

            if (input.LeftPressed)
            {
                PlayPreviousSong();                
            }

            if (input.UpPressed)
            {
                Guide.Show();
            }

            switch (MediaPlayer.State)
            {
                case MediaState.Stopped:
                    boldText = "Stopped";
                    regularText = "Press Play to play a song.";
                    break;
                case MediaState.Playing:
                    Song currentSong = MediaPlayer.Queue.ActiveSong;
                    
                    boldText = "Artist: " + currentSong.Artist.Name + 
                        "\r\nAlbum: " + currentSong.Album.Name;

                    regularText = currentSong.Name;
                    break;
                case MediaState.Paused:
                    boldText = "Paused";
                    regularText = "";
                    break;
            }         

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();
            try
            {
                spriteBatch.Draw(albumArtTex, albumArtPosition, Color.White);
                spriteBatch.DrawString(fontBold, boldText, boldTextPosition, Color.White);
                spriteBatch.DrawString(font, regularText, regularTextPosition, Color.White);                
            }
            catch
            {

            }

            spriteBatch.End();

            base.Draw(gameTime);
        }

        private void RefreshAlbumArt()
        {
            if (MediaPlayer.State == MediaState.Playing ||
                MediaPlayer.State == MediaState.Paused)
            {
                Song currentSong = MediaPlayer.Queue.ActiveSong;
                if (currentSong.Album.HasArt)
                {
                    albumArtTex = currentSong.Album.GetAlbumArt(this.Services);
                }
                else
                    albumArtTex = noArtTex;
            }
            else
            {
                albumArtTex = noArtTex;
            }

            albumArtPosition = new Vector2(120 - albumArtTex.Width / 2, 100);            
        }

        private void PlayNextSong()
        {            
            for (int songIndex = currentSongIndex + 1; songIndex <= mediaLibrary.Songs.Count; songIndex++)
            {
                if (songIndex >= mediaLibrary.Songs.Count)
                    songIndex = 0;

                if (mediaLibrary.Songs[songIndex].IsProtected == false)
                {
                    currentSongIndex = songIndex;
                    MediaPlayer.Play(mediaLibrary.Songs[currentSongIndex]);
                    break;
                }
            }
            RefreshAlbumArt();
        }

        private void PlayPreviousSong()
        {
            for (int songIndex = currentSongIndex - 1; songIndex >= -1; songIndex--)
            {
                if (songIndex < 0)
                    songIndex = mediaLibrary.Songs.Count - 1;

                if (mediaLibrary.Songs[songIndex].IsProtected == false)
                {
                    currentSongIndex = songIndex;
                    MediaPlayer.Play(mediaLibrary.Songs[currentSongIndex]);
                    break;
                }
            }
            RefreshAlbumArt();
        }
    }
}
