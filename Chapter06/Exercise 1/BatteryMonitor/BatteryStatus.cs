using System;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace BatteryMonitor
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class BatteryStatus : Microsoft.Xna.Framework.DrawableGameComponent
    {
        #region Fields

        // Content
        SpriteBatch spriteBatch;
        SpriteFont normalFont;
        Texture2D batteryChargingTex;
        Texture2D batteryCriticalTex;
        Texture2D batteryHighTex;
        Texture2D batteryLowTex;
        Texture2D batteryNoneTex;
        Texture2D batteryUnknownTex;
        Texture2D currentBatteryStatusTex;

        // Position
        Vector2 batteryStatusTextPosition;
        Vector2 batteryStatusIconPosition;
        Vector2 batteryStatusTextureOrigin;        

        // Status Messages
        string batteryStatusText = "";

        #endregion

        #region Constructor(s)

        public BatteryStatus(Game game)
            : base(game)
        {
            // Wire up the PowerStatusChanged event.
            PowerStatus.PowerStateChanged += new EventHandler(PowerStatusChangedHandler);
        }

        #endregion

        #region Event Handlers

        void PowerStatusChangedHandler(object sender, EventArgs e)
        {
            ResetBatteryStatusUI();
        }

        #endregion

        #region Overridden GameComponent Methods

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            batteryStatusTextPosition = new Vector2(10, 50);
            batteryStatusIconPosition = new Vector2(120, 200);            

            base.Initialize();
        }

        protected override void LoadContent()
        {
            normalFont = Game.Content.Load<SpriteFont>("Normal");
            batteryChargingTex = Game.Content.Load<Texture2D>("Textures/Battery/battery_charging");
            batteryCriticalTex = Game.Content.Load<Texture2D>("Textures/Battery/battery_critical");
            batteryHighTex = Game.Content.Load<Texture2D>("Textures/Battery/battery_high");
            batteryLowTex = Game.Content.Load<Texture2D>("Textures/Battery/battery_low");
            batteryNoneTex = Game.Content.Load<Texture2D>("Textures/Battery/battery_nobattery");
            batteryUnknownTex = Game.Content.Load<Texture2D>("Textures/Battery/battery_unknown");
            currentBatteryStatusTex = batteryUnknownTex;

            batteryStatusTextureOrigin =
                new Vector2(currentBatteryStatusTex.Width / 2, currentBatteryStatusTex.Height / 2);

            ResetBatteryStatusUI();

            base.LoadContent();
        }

        public override void Draw(GameTime gameTime)
        {
            if (spriteBatch == null)
                spriteBatch = (SpriteBatch)Game.Services.GetService(typeof(SpriteBatch));
            
            // Draw the status text in white
            spriteBatch.DrawString(normalFont, batteryStatusText, 
                batteryStatusTextPosition, Color.White);

            // Draw the battery icon using the calculated origin
            spriteBatch.Draw(currentBatteryStatusTex, batteryStatusIconPosition, null, 
                Color.White, 0.0f, batteryStatusTextureOrigin, 1.0f, SpriteEffects.None, 0.5f);

            base.Draw(gameTime);
        }

        #endregion

        #region Private Methods

        private void ResetBatteryStatusUI()
        {
            StringBuilder sb = new StringBuilder();

            // Write the battery's status (Charging, High, etc).
            sb.Append("Battery Status: ");
            
            // Check all the battery states
            if ((PowerStatus.BatteryChargeStatus & BatteryChargeStatus.Charging)
                == BatteryChargeStatus.Charging)
            {
                sb.AppendLine("Charging");
                currentBatteryStatusTex = batteryChargingTex;
            }
            else
            {
                switch (PowerStatus.BatteryChargeStatus)
                {
                    case BatteryChargeStatus.Critical:
                        sb.AppendLine("Critical");
                        currentBatteryStatusTex = batteryCriticalTex;
                        break;
                    case BatteryChargeStatus.High:
                        sb.AppendLine("High");
                        currentBatteryStatusTex = batteryHighTex;
                        break;
                    case BatteryChargeStatus.Low:
                        sb.AppendLine("Low");
                        currentBatteryStatusTex = batteryLowTex;
                        break;
                    case BatteryChargeStatus.NoSystemBattery:
                        sb.AppendLine("No Battery");
                        currentBatteryStatusTex = batteryNoneTex;
                        break;
                    case BatteryChargeStatus.Unknown:
                        sb.AppendLine("Unknown");
                        currentBatteryStatusTex = batteryUnknownTex;
                        break;
                    case 0:
                        sb.AppendLine("Normal");
                        currentBatteryStatusTex = batteryHighTex;
                        break;
                    default:
                        sb.AppendLine("?" + PowerStatus.BatteryChargeStatus.ToString());
                        currentBatteryStatusTex = batteryUnknownTex;
                        break;
                }
            }

            // Write the amount remaining            
            sb.AppendLine(PowerStatus.BatteryLifePercent + "% remaining");

            // Get the power line connection status
            switch (PowerStatus.PowerLineStatus)
            {
                case PowerLineStatus.Online:
                    sb.AppendLine("Connected");
                    break;
                case PowerLineStatus.Offline:
                    sb.AppendLine("Not Connected");
                    break;
                case PowerLineStatus.Unknown:
                    sb.AppendLine("Connection Unknown");
                    break;
            }

            // Assign the contents of the stringbuilder to the member variable
            batteryStatusText = sb.ToString();
        }

        #endregion
    }
}