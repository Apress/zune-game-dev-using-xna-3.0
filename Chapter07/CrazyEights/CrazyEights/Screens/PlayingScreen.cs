using Microsoft.Xna.Framework;
using GameStateManager;
using CardLib;

namespace CrazyEights
{
    public class PlayingScreen : BaseScreen
    {
        PlayerViewComponent playerView;
        CrazyEightsGameManager gameManager;
        SuitSelectionMenu suitMenu;

        private bool isHost;

        public PlayingScreen(bool host)
        {
            isHost = host;
        }

        #region BaseScreen Overrides

        public override void Initialize()
        {
            playerView = new PlayerViewComponent(ScreenManager);            
            gameManager = new CrazyEightsGameManager(ScreenManager);
            Components.Add(gameManager);
            Components.Add(playerView);

            suitMenu = new SuitSelectionMenu();
            suitMenu.SuitSelected += new SuitSelectionMenu.SuitSelectedHandler(SuitSelected);

            base.Initialize();

            gameManager.AllPlayersJoined += 
                new CrazyEightsGameManager.AllPlayersJoinedHandler(AllPlayersJoined);
            gameManager.AllCardsDealt += 
                new CrazyEightsGameManager.AllCardsDealtHandler(AllCardsDealt);
            gameManager.CardsUpdated += 
                new CrazyEightsGameManager.CardsUpdatedHandler(CardsUpdated);
            gameManager.GameWon += 
                new CrazyEightsGameManager.GameWonHandler(GameWon);

            if (isHost)
            {
                gameManager.Host_SendPlayers();                                
            }            
        }

        public override void HandleInput(InputState input)
        {
            if (input.NewRightPress)
            {
                if (playerView.HasMove)
                    playerView.SelectNextCard();
            }

            if (input.NewLeftPress)
            {
                if (playerView.HasMove)
                    playerView.SelectPreviousCard();
            }

            if (input.NewBackPress)
            {
                if (ScreenManager.Network.Session != null)
                    ScreenManager.RemoveScreen(this);
            }

            if (input.MiddleButtonPressed && gameManager.Me.IsMyTurn)
            {
                if (playerView.HasMove)
                {
                    Card selected = playerView.SelectedCard;
                    if (selected.CardValue.Value == 8)
                    {
                        ScreenManager.AddScreen(suitMenu);
                    }
                    else
                        gameManager.PlayCard(selected);
                }
                else
                {
                    gameManager.RequestCard();
                }
            }

            base.HandleInput(input);
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, 
            bool coveredByOtherScreen)
        {
            gameManager.ReceiveNetworkData();
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
        }

        #endregion

        #region Event Handlers

        void GameWon(string playerName)
        {
            ScreenManager.RemoveScreen(this);
            ScreenManager.AddScreen(new GameOverScreen(playerName, gameManager.Me.Name));
        }

        void CardsUpdated()
        {
            playerView.SelectFirstPlayableCard();
        }

        void AllCardsDealt()
        {
            playerView.SelectFirstPlayableCard();
        }

        void AllPlayersJoined()
        {
            playerView.Player = gameManager.Me;            

            if (isHost)
            {                                
                gameManager.Host_NewRound();
            }
        }
        
        void SuitSelected(SuitSelectionEventArgs e)
        {
            gameManager.ChooseSuit(e.Suit);
            ScreenManager.RemoveScreen(suitMenu);

            gameManager.PlayCard(playerView.SelectedCard);
        }        

        #endregion
    }
}