namespace TestGame
{
    public static class ShapeParameters
    {
        public const int NameBoxWidthFg = 170;
        public const int NameBoxHeightFg = 60;
        public const int NameBoxWidthBg = 182;
        public const int NameBoxHeightBg = 72;
        public const int NameBoxWidthOffset = (NameBoxWidthBg - NameBoxWidthFg) / 2;
        public const int NameBoxHeightOffset = (NameBoxHeightBg - NameBoxHeightFg) / 2;
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