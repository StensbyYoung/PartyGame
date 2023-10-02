namespace TestGame
{
    public static class ShapeParameters
    {
        public const int SmallTextBoxWidthFg = 170;
        public const int SmallTextBoxHeightFg = 60;
        public const int SmallTextBoxWidthBg = 182;
        public const int SmallTextBoxHeightBg = 72;
        public const int SmallTextBoxWidthOffset = (SmallTextBoxWidthBg - SmallTextBoxWidthFg) / 2;
        public const int SmallTextBoxHeightOffset = (SmallTextBoxHeightBg - SmallTextBoxHeightFg) / 2;
    }

    public static class PositionCoords
    {
        public const int TopOfScreenYCoord = 32;
    }

    public static class DrawLayers
    {
        public const float Identifiers = 0f;
        public const float NameBoxFg = 0.1f;
        public const float NameBoxBg = 0.2f;
        public const float Background = 0.3f;
    }
    public enum GameStates
    {
        Init,
        PreGame,
        EnteringPlayer,
        Playing,
        EndGame
    }

    public enum Difficulty
    {
        Dafuq,
        Easy,
        Medium,
        Hard,
        GoblinMode
    }

    public enum PlayingMode
    {
        Standard = 5
    }
}