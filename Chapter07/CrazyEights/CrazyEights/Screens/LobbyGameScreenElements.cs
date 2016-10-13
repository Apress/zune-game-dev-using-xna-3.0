using Microsoft.Xna.Framework;

namespace CrazyEights
{
    /// <summary>
    /// Used to define the placement of specific graphical elements on the screen.
    /// </summary>
    /// <remarks>This is specific to the artistic layout of the game and helps us position game elements accordingly.</remarks>
    public static class LobbyGameScreenElements
    {
        public static Vector2 PlayerListPosition;
        public static Vector2 InitialTextListPosition;
        public static Vector2 InitialListStatusPosition;
        public static Vector2 StatusMessagePosition;
        public static Vector2 HighlightInitialPosition;

        public const int PLAYER_VERTICAL_SPACING = 20;
        public const int LIST_OUTLINE_OFFSET = 4;
       
        static LobbyGameScreenElements()
        {
            PlayerListPosition = new Vector2(14, 82);
            InitialTextListPosition = new Vector2(16, 82);
            InitialListStatusPosition = new Vector2(155, 82);
            StatusMessagePosition = new Vector2(120, 202);
            HighlightInitialPosition = PlayerListPosition;
            
            HighlightInitialPosition.X -= LIST_OUTLINE_OFFSET;
            HighlightInitialPosition.Y -= LIST_OUTLINE_OFFSET;
        }
    }
}