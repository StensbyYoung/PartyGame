using System;
using Microsoft.Xna.Framework;

namespace TestGame;

public class Player
{
    private static int numPlayers = 0;
    //private readonly int ID;
    private int HP, ID, nameBoxCenterX, nameBoxCenterY, nameBoxRow, nameBoxCol;
    private readonly string name;
    private Color playerColor;
    
    // Constructor
    public Player(string c_playerName, Color c_color, PlayingMode c_mode)
    {
        ID = numPlayers++;
        name = c_playerName;
        playerColor = c_color;
        HP = (int) c_mode;
        nameBoxCenterX = nameBoxCenterY = 0;
        nameBoxRow = nameBoxCol = 1;
    }

    // Finalizer (Destructor)
    ~Player()
    {
        numPlayers--;
    }

    public int PlayerID
    {
        get{ return ID; }
        set{ ID = value; }
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
        set{ numPlayers = value; }
    }

    public int NameBoxCenterX
    {
        get{ return nameBoxCenterX; }
        set{ nameBoxCenterX = value; }
    }

    public int NameBoxCenterY
    {
        get{ return nameBoxCenterY; }
        set{ nameBoxCenterY = value; }
    }

    public int NameBoxRow
    {
        get{ return nameBoxRow; }
        set{ nameBoxRow = value; }
    }

    public int NameBoxCol
    {
        get{ return nameBoxCol; }
        set{ nameBoxCol = value; }
    }
}