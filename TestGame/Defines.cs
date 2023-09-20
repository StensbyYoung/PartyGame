namespace TestGame
{
    public static class ShapeParameters
    {
        public const int NameBoxWidthFg = 185;
        public const int NameBoxHeightFg = 65;
        public const int NameBoxWidthBg = 200;
        public const int NameBoxHeightBg = 80;
        public const int NameBoxOffset = 250;
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