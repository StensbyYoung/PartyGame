using System;
using Microsoft.Xna.Framework;

namespace TestGame;

public class Player
{
    private readonly string name;
    private Color playerColor;
    private int HP;
    public int NameBoxCenterX, NameBoxCenterY;
    
    public Player(string c_playerName, Color c_color, PlayingMode c_mode)
    {
        name = c_playerName;
        playerColor = c_color;
        HP = (int) c_mode;
    }

    public string PlayerName
    {
        get{ return name; }
    }

    public int PlayerHP
    {
        get{ return HP; }
        set{ HP = value; }
    }
    
    public Color PlayerColor
    {
        get{ return playerColor; }
        set{ playerColor = value; }
    }
}