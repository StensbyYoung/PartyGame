using System.ComponentModel;

namespace TestGame;

public static class ShapeParameters
{
    public const int SmallTextBoxWidthFg = 170;
    public const int SmallTextBoxHeightFg = 60;
    public const int SmallTextBoxWidthBg = 182;
    public const int SmallTextBoxHeightBg = 72;
    public const int SmallTextBoxWidthOffset = (SmallTextBoxWidthBg - SmallTextBoxWidthFg) / 2;
    public const int SmallTextBoxHeightOffset = (SmallTextBoxHeightBg - SmallTextBoxHeightFg) / 2;

    public const int HpBoxBgWidth = 182;
    public const int HpBoxBgHeight = 28;
    public const int SingleHpBoxWidth = 34;
    public const int SingleHpBoxHeight = 24;
    public const int SingleHpOffset = 2;
}

public static class PositionCoords
{
    public const int TopOfScreenYCoord = 32;
    public const int FirstNameBoxYCoord = 300;
    public const int NameBoxXCoord = ShapeParameters.SmallTextBoxWidthBg / 2;
    public const int FrameOffset = 20;
}

public static class Variables
{
    public const int MaxPlayers = 30;
    public const int MaxNameBoxColumns = 5;
    public const int MaxNameBoxRows = MaxPlayers / MaxNameBoxColumns;
}

public static class DrawLayers
{
    public const float Identifiers = 0f;
    public const float NameBoxFg = 0.1f;
    public const float NameBoxBg = 0.2f;
    public const float Background = 0.3f;
}

public enum TimerValues
{
    OneSecond = 1000,
    TwoSeconds = 2000,
    ThreeSeconds = 3000,
    FourSeconds = 4000,
    FiveSeconds = 5000,
}

public enum ResolutionCorrection
{
    TwoKRes = -100,
}

public enum GameStates
{
    Init,
    PreGame,
    RemovePlayer,
    EnterPlayer,
    Playing,
    Pause,
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