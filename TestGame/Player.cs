using System;
using Microsoft.Xna.Framework;

namespace TestGame;

public class Player
{
    private static int numPlayers = 0;
    private readonly int ID;
    private int HP;
    private readonly string name;
    private Color playerColor;
    public int NameBoxCenterX, NameBoxCenterY;
    
    // Constructor
    public Player(string c_playerName, Color c_color, PlayingMode c_mode)
    {
        ID = numPlayers++;
        name = c_playerName;
        playerColor = c_color;
        HP = (int) c_mode;
    }

    // Finalizer (Destructor)
    ~Player()
    {
        numPlayers--;
    }

    public int PlayerID
    {
        get{ return ID; }
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

    public static int NumberOfPlayers
    {
        get{ return numPlayers; }
    }
}