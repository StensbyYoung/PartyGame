﻿using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Runtime.InteropServices;
//using MonoGame.Extended;

namespace TestGame;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    private SpriteFont StandardFont;
    private SpriteFont StandardBoldFont;
    private Texture2D RoundedRectangleFg;
    private Texture2D RoundedRectangleBg;
    private Texture2D HpBg;
    private Texture2D SingleHpPoint;
    private Texture2D Pixel;
    private StringBuilder TextBox = new ();
    private List<string> AllPlayers = new ();
    private GameStates State = GameStates.Init;
    private int DeviceResY;
    private int DeviceResX;
    private List<Player> PlayerObjectList = new ();

    private string ProjectDirectoryPath;
    private string FilePath_LoserDB;
    private int loserID, hitPlayer, rowCount, colCount;
    private bool GhostingLock_space, GhostingLock_down, GhostingLock_enter;
    private bool GameIsPaused, FirstRun, WriteToDataBase;
    private int ElapsedTime;

    private const string SDL = "SDL2.dll";
    [DllImport(SDL, CallingConvention = CallingConvention.Cdecl)]
    private static extern void SDL_MaximizeWindow(IntPtr window);

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        Window.AllowUserResizing = true;
        Window.TextInput += TextInputHandler;
        DeviceResY = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
        DeviceResX = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
        _graphics.PreferredBackBufferWidth = DeviceResX;
        _graphics.PreferredBackBufferHeight = DeviceResY;
        Window.Position = new Point(0,0);
        MaximizeWindow();
        _graphics.ApplyChanges();

        ProjectDirectoryPath = Environment.CurrentDirectory;
        FilePath_LoserDB = Path.Combine(ProjectDirectoryPath, "LoserDataBase.txt");

        GhostingLock_space = GhostingLock_down = GhostingLock_enter = false;
        GameIsPaused = FirstRun = WriteToDataBase = true;
        ElapsedTime = 0;

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        Pixel = new Texture2D(GraphicsDevice, 1, 1);
        Pixel.SetData(new[] { Color.White} );
        StandardFont = Content.Load<SpriteFont>("Fonts/standardFont");
        StandardBoldFont = Content.Load<SpriteFont>("Fonts/standardBoldFont");
        RoundedRectangleFg = Content.Load<Texture2D>("Images/roundedRectangleFg");
        RoundedRectangleBg = Content.Load<Texture2D>("Images/roundedRectangleBg");
        SingleHpPoint = Content.Load<Texture2D>("Images/HpPoint");
        HpBg = Content.Load<Texture2D>("Images/HpBg");
    }

    protected override void Update(GameTime gameTime)
    {
        if(GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
        {
            Exit();
        }

        switch(State)
        {
            case GameStates.Init:
                if(FirstRun)
                {
                    TestRows(1);
                    FirstRun = false;
                }
                if(Keyboard.GetState().IsKeyDown(Keys.Space))
                {
                    colCount = 1;
                    rowCount = 1;
                    State = GameStates.PreGame;
                }
                break;
            case GameStates.PreGame:
                if(Keyboard.GetState().IsKeyDown(Keys.C))
                {
                    TextBox.Clear();
                }
                if(Keyboard.GetState().IsKeyUp(Keys.Enter))
                {
                    GhostingLock_enter = false;
                }
                if(Keyboard.GetState().IsKeyDown(Keys.Enter) && !GhostingLock_enter)
                {
                    GhostingLock_enter = true;
                    TextBox.Clear();
                    State = GameStates.EnterPlayer;
                }

                if(Keyboard.GetState().IsKeyDown(Keys.P))
                {
                    TextBox.Clear();

                    foreach(var p in PlayerObjectList)
                    {
                        SetNameBoxCoords(p);
                    }
                    
                    State = GameStates.Pause;
                }

                if(Keyboard.GetState().IsKeyDown(Keys.R))
                {
                    ResetGame();
                }

                if(Keyboard.GetState().IsKeyDown(Keys.Subtract) || Keyboard.GetState().IsKeyDown(Keys.Back))
                {
                    TextBox.Clear();
                    State = GameStates.RemovePlayer;
                }
                break;
            case GameStates.RemovePlayer:
                if(Keyboard.GetState().IsKeyDown(Keys.Enter))
                {
                    GhostingLock_enter = true;
                    if(int.TryParse(TextBox.ToString(), out int RemoveIdx))
                    {
                        if(RemoveIdx >= PlayerObjectList.Count)
                        {
                            State = GameStates.PreGame;
                            break;
                        }
                        PlayerObjectList.RemoveAt(RemoveIdx);
                        AllPlayers.RemoveAt(RemoveIdx);
                        Player.NumberOfPlayers--;
                        for(int i = RemoveIdx; i < PlayerObjectList.Count; i++)
                        {
                            PlayerObjectList[i].PlayerID--;
                        }
                    }
                    State = GameStates.PreGame;
                }
                break;
            case GameStates.EnterPlayer:
                if(Keyboard.GetState().IsKeyUp(Keys.Enter))
                {
                    GhostingLock_enter = false;
                }
                if(Keyboard.GetState().IsKeyDown(Keys.Enter) && !GhostingLock_enter)
                {
                    GhostingLock_enter = true;
                    if(TextBox.ToString() != "")
                    {
                        string FilteredText = TextBox.ToString().Replace("\n","").Replace("\r","");
                        AddPlayer(FilteredText);
                        PlayerObjectList.Add(new Player(FilteredText, Color.Black, PlayingMode.Standard));
                    }
                    State = GameStates.PreGame;
                }
                break;
            case GameStates.Playing:
                if(Keyboard.GetState().IsKeyUp(Keys.Down))
                {
                    GhostingLock_down = false;
                }
                if(Keyboard.GetState().IsKeyUp(Keys.Space))
                {
                    GhostingLock_space = false;
                }
                if(Keyboard.GetState().IsKeyDown(Keys.Space) && !GhostingLock_space)
                {
                    GhostingLock_space = true;
                    GameIsPaused = true;
                    State = GameStates.Pause;
                    break;
                }
                if(!GameIsPaused)
                {
                    ElapsedTime += gameTime.ElapsedGameTime.Milliseconds;
                    if(Keyboard.GetState().IsKeyDown(Keys.Down) && !GhostingLock_down)
                    {
                        GhostingLock_down = true;
                        ElapsedTime = 0;
                        hitPlayer = Functions.GetRandomNumber(0, PlayerObjectList.Count);
                        if(--PlayerObjectList[hitPlayer].PlayerHP == 0)
                        {
                            loserID = PlayerObjectList[hitPlayer].PlayerID;
                            State = GameStates.EndGame;
                        }
                    }
                    else if(ElapsedTime >= (int)TimerValues.ThreeSeconds)
                    {
                        ElapsedTime = 0;
                        hitPlayer = Functions.GetRandomNumber(0, PlayerObjectList.Count);
                        if(--PlayerObjectList[hitPlayer].PlayerHP == 0)
                        {
                            loserID = PlayerObjectList[hitPlayer].PlayerID;
                            State = GameStates.EndGame;
                        }
                    }
                }
                if(Keyboard.GetState().IsKeyDown(Keys.Home))
                {
                    State = GameStates.PreGame;
                }
                break;
            case GameStates.Pause:
                if(Keyboard.GetState().IsKeyUp(Keys.Space))
                {
                    GhostingLock_space = false;
                }
                if(Keyboard.GetState().IsKeyDown(Keys.Space) && !GhostingLock_space)
                {
                    GhostingLock_space = true;
                    GameIsPaused = false;
                    State = GameStates.Playing;
                }
                break;
            case GameStates.EndGame:
                if(WriteToDataBase)
                {
                    WriteToDataBase = false;
                    DataBase.Write(PlayerObjectList[loserID].PlayerName, FilePath_LoserDB);
                }
                if(Keyboard.GetState().IsKeyDown(Keys.Home) || Keyboard.GetState().IsKeyDown(Keys.Space))
                {
                    ResetGame();
                    State = GameStates.PreGame;
                }
                break;
            default:
                break;
        }

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        _spriteBatch.Begin();

        _spriteBatch.DrawString(StandardBoldFont, "Elapsed time in ms: " + ElapsedTime, new Vector2(DeviceResX / 2, DeviceResY - 200), Color.Black);
        _spriteBatch.DrawString(StandardBoldFont, "State: " + State, new Vector2(20, 10), Color.Black);
        _spriteBatch.DrawString(StandardBoldFont, "X res: " + DeviceResX.ToString(), new Vector2(20, 40), Color.Black);
        _spriteBatch.DrawString(StandardBoldFont, "Y res: " + DeviceResY.ToString(), new Vector2(20, 70), Color.Black);

        switch(State)
        {
            case GameStates.Init:
                //DrawSmallTextBox(DeviceResX/4, DeviceResY/4, Color.Fuchsia);
                DrawPlayerBoxes();
                break;
            case GameStates.PreGame:
                _spriteBatch.DrawString(StandardBoldFont, "Players: ", new Vector2(20, 100), Color.Black);
                _spriteBatch.DrawString(StandardBoldFont, "Number of players: " + Player.NumberOfPlayers, new Vector2(20, 130), Color.Black);
                foreach(var p in PlayerObjectList)
                {
                    _spriteBatch.DrawString(StandardBoldFont, p.PlayerName, new Vector2(20, 160 + 30 * p.PlayerID), p.PlayerColor);
                }
                break;
            case GameStates.EnterPlayer:
                _spriteBatch.DrawString(StandardBoldFont, "Elapsed time in ms: " + ElapsedTime, new Vector2(DeviceResX / 2, DeviceResY - 200), Color.Black);
                _spriteBatch.DrawString(StandardBoldFont, "Players: ", new Vector2(20, 100), Color.Black);
                _spriteBatch.DrawString(StandardBoldFont, "Enter player name: ", new Vector2((DeviceResX / 2) - (15 * 15 + 10 * 3), (DeviceResY / 2)), Color.Black);
                _spriteBatch.DrawString(StandardBoldFont, TextBox, new Vector2(DeviceResX / 2, DeviceResY / 2), Color.Black);
                _spriteBatch.DrawString(StandardBoldFont, TextBox.Length.ToString(), new Vector2(DeviceResX / 2, (DeviceResY / 2) + 40), Color.Black);
                break;
            case GameStates.Playing:
                //DrawSmallTextBox(DeviceResX/2, ShapeParameters.SmallTextBoxHeightBg, Color.Black);
                //DrawSmallTextBox(DeviceResX/2 - 100, 2 * ShapeParameters.SmallTextBoxHeightBg, Color.Black);
                DrawPlayerBoxes();       
                /*
                foreach(var p in PlayerObjectList)
                {
                    DrawNameBox(PositionCoords.NameBoxXCoord, PositionCoords.FirstNameBoxYCoord, p);
                }
                */
                break;
            case GameStates.Pause:
                _spriteBatch.DrawString(StandardBoldFont, "GAME IS PAUSED", new Vector2(DeviceResX / 2, DeviceResY / 2), Color.Black);
                break;
            case GameStates.EndGame:
                _spriteBatch.DrawString(StandardBoldFont, "The loser is : " + PlayerObjectList[loserID].PlayerName, new Vector2(DeviceResX / 2, DeviceResY / 2), Color.Black);
                break;
            default:
                break;
        }

        if(AllPlayers.Count > 0)
        {
            _spriteBatch.DrawString(StandardBoldFont, string.Join(", ", AllPlayers), new Vector2(140, 100), Color.Black);
        }
        
        _spriteBatch.End();
        base.Draw(gameTime);
    }

    

    private void AddPlayer(string player)
    {
        AllPlayers.Add(player);       
    }

    private void TextInputHandler(object sender, TextInputEventArgs args)
    {
        var pressedKey = args.Key;
        var character = args.Character;

        if(character == (char)Keys.Back)
        {
            var index = TextBox.Length - 1;
            if(index >= 0)
            {
                TextBox.Remove(index, 1);
            }
        }
        else
        {
            TextBox.Append(character);
        }
    }

    private void MaximizeWindow()
    {
        SDL_MaximizeWindow(Window.Handle);
    }

    private void DrawSmallTextBox(int boxCenterX, int boxCenterY, Color edgeClr)
    {
        int posXBg = boxCenterX - (ShapeParameters.SmallTextBoxWidthBg / 2);
        int posYBg = boxCenterY - (ShapeParameters.SmallTextBoxHeightBg / 2);
        int posXFg = boxCenterX - (ShapeParameters.SmallTextBoxWidthFg / 2);
        int posYFg = boxCenterY - (ShapeParameters.SmallTextBoxHeightFg / 2);
        Vector2 posBg = new (posXBg, posYBg);
        Vector2 posFg = new (posXFg, posYFg);
        _spriteBatch.Draw(RoundedRectangleBg, posBg, edgeClr);
        _spriteBatch.Draw(RoundedRectangleFg, posFg, Color.BlanchedAlmond);
    }

    private void DrawNameBox(Player p)
    {
        var len = StandardBoldFont.MeasureString(p.PlayerName);
        var xCoordString = p.NameBoxCenterX - (len.Length() / 2);
        var yCoordString = p.NameBoxCenterY - ShapeParameters.SmallTextBoxHeightFg / 4;

        DrawSmallTextBox(p.NameBoxCenterX, p.NameBoxCenterY, Color.Black);
        _spriteBatch.DrawString(StandardBoldFont, p.PlayerName, new Vector2(xCoordString, yCoordString), Color.Black);
    }

    private void DrawHpBox(int coordX, int coordY, int HP)
    {
        int anchorX = coordX - (ShapeParameters.SmallTextBoxWidthBg / 2);
        int anchorY = coordY + (ShapeParameters.SmallTextBoxHeightBg / 2);
        
        Vector2 bg = new (anchorX, anchorY);
        _spriteBatch.Draw(HpBg, bg, Color.Black);

        switch(HP)
        {
            case 5:
                Vector2 HpBox5 = new (anchorX + 5 * ShapeParameters.SingleHpOffset + 4 * ShapeParameters.SingleHpBoxWidth, anchorY + ShapeParameters.SingleHpOffset);
                _spriteBatch.Draw(SingleHpPoint, HpBox5, Color.LawnGreen); 
                goto case 4;
            case 4:
                Vector2 HpBox4 = new (anchorX + 4 * ShapeParameters.SingleHpOffset + 3 * ShapeParameters.SingleHpBoxWidth, anchorY + ShapeParameters.SingleHpOffset);
                _spriteBatch.Draw(SingleHpPoint, HpBox4, Color.LawnGreen);
                goto case 3;
            case 3:
                Vector2 HpBox3 = new (anchorX + 3 * ShapeParameters.SingleHpOffset + 2 * ShapeParameters.SingleHpBoxWidth, anchorY + ShapeParameters.SingleHpOffset);
                _spriteBatch.Draw(SingleHpPoint, HpBox3, Color.LawnGreen);
                goto case 2;
            case 2:
                Vector2 HpBox2 = new (anchorX + 2 * ShapeParameters.SingleHpOffset + 1 * ShapeParameters.SingleHpBoxWidth, anchorY + ShapeParameters.SingleHpOffset);
                _spriteBatch.Draw(SingleHpPoint, HpBox2, Color.LawnGreen);
                goto case 1;
            case 1:
                Vector2 HpBox1 = new (anchorX + 1 * ShapeParameters.SingleHpOffset, anchorY + ShapeParameters.SingleHpOffset);
                _spriteBatch.Draw(SingleHpPoint, HpBox1, Color.LawnGreen);
                break;            
            default:
                break;
        }
        
        
        //_spriteBatch.Draw(SingleHpPoint, hp, Color.LawnGreen);
        
    }

    private void ResetGame()
    {
        PlayerObjectList.Clear();
        AllPlayers.Clear();
        TextBox.Clear();
        Player.NumberOfPlayers = 0;
        ElapsedTime = 0;
        colCount = 1;
        rowCount = 1;
        WriteToDataBase = true;
    }

    private void SetNameBoxCoords(Player p)
    {
        int YResPerRow = (DeviceResY + (int)ResolutionCorrection.TwoKRes) / Variables.MaxNameBoxRows;
        int firstBox;
        if(PlayerObjectList.Count >= 5)
        {
            firstBox = DeviceResX / 6;
        }
        else
        {
            firstBox = DeviceResX / (PlayerObjectList.Count + 1);
        }
       
        p.NameBoxCenterX = firstBox * colCount;

        switch(rowCount)
        {
            case 1:
                p.NameBoxCenterY = YResPerRow;
                break;
            case 2:
                p.NameBoxCenterY = YResPerRow * 2;
                break;
            case 3:
                p.NameBoxCenterY = YResPerRow * 3;
                break;
            case 4:
                p.NameBoxCenterY = YResPerRow * 4;
                break;
            case 5:
                p.NameBoxCenterY = YResPerRow * 5;
                break;
            case 6:
                p.NameBoxCenterY = YResPerRow * 6;
                break;
            default:
                break;
        }

        colCount++;

        if(colCount >= 6)
        {
            colCount = 1;
            rowCount++;
        }
    }

    private void DrawPlayerBoxes()
    {
        foreach(var p in PlayerObjectList)
        {
            DrawNameBox(p);
            DrawHpBox(p.NameBoxCenterX, p.NameBoxCenterY, p.PlayerHP);
        }
    }

    private void TestRows(int rows)
    {
        colCount = 1;
        rowCount = 1;
        for(int i = 1; i < (rows * 5) + 1; i++)
        {
            PlayerObjectList.Add(new Player(i.ToString(), Color.Black, PlayingMode.Standard));
        }
        foreach(var p in PlayerObjectList)
        {
            SetNameBoxCoords(p);
        }
    }
}
