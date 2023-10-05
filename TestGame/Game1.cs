using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Runtime.InteropServices;
using System.Linq;
using MonoGame.Extended;

namespace TestGame;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    private SpriteFont StandardFont;
    private SpriteFont StandardBoldFont;
    private Texture2D RoundedRectangleFg;
    private Texture2D RoundedRectangleBg;
    private Texture2D Pixel;
    private StringBuilder TextBox = new ();
    private List<string> AllPlayers = new ();
    private GameStates State = GameStates.Init;
    private int DeviceResY;
    private int DeviceResX;
    private List<Player> PlayerObjectList = new ();

    private int loserID, hitPlayer;
    private bool GhostingLock_space, GhostingLock_down, GameIsPaused;
    private int ElapsedTime;

    private const string SDL = "SDL2.dll";
    [DllImport(SDL, CallingConvention = CallingConvention.Cdecl)]
    public static extern void SDL_MaximizeWindow(IntPtr window);

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
        SDL_MaximizeWindow(Window.Handle);
        _graphics.ApplyChanges();

        GhostingLock_space = GhostingLock_down = false;
        GameIsPaused = true;
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
                if(Keyboard.GetState().IsKeyDown(Keys.Space))
                {
                    State = GameStates.PreGame;
                }
                break;
            case GameStates.PreGame:
                if(Keyboard.GetState().IsKeyDown(Keys.C))
                {
                    TextBox.Clear();
                }

                if(Keyboard.GetState().IsKeyDown(Keys.Add))
                {
                    TextBox.Clear();
                    State = GameStates.EnterPlayer;
                }

                if(Keyboard.GetState().IsKeyDown(Keys.P))
                {
                    TextBox.Clear();
                    State = GameStates.Pause;
                }

                if(Keyboard.GetState().IsKeyDown(Keys.R))
                {
                    ResetGame();
                }

                if(Keyboard.GetState().IsKeyDown(Keys.Subtract))
                {
                    TextBox.Clear();
                    State = GameStates.RemovePlayer;
                }
                break;
            case GameStates.RemovePlayer:
                if(Keyboard.GetState().IsKeyDown(Keys.Enter))
                {
                    if(int.TryParse(TextBox.ToString(), out int RemoveIdx))
                    {
                        if(RemoveIdx > PlayerObjectList.Count)
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
                if(Keyboard.GetState().IsKeyDown(Keys.Enter))
                {
                    if(TextBox.ToString() != "")
                    {
                        AddPlayer(TextBox.ToString());
                        PlayerObjectList.Add(new Player(TextBox.ToString(), Color.Black, PlayingMode.Standard));
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
                if(Keyboard.GetState().IsKeyDown(Keys.Home))
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
                DrawSmallTextBox(DeviceResX/4, DeviceResY/4, Color.Fuchsia);
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
                foreach(var p in PlayerObjectList)
                {
                    DrawNameBox(PositionCoords.NameBoxXCoord, PositionCoords.FirstNameBoxYCoord, p);
                }
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

    private void DrawNameBox(int centerAnchorX, int centerAnchorY, Player p)
    {
        var len = StandardBoldFont.MeasureString(p.PlayerName);
        int boxCenterX = centerAnchorX + PositionCoords.FrameOffset;
        int boxCenterY = centerAnchorY + p.PlayerID * 200;
        int xCoordHP = boxCenterX + ShapeParameters.SmallTextBoxWidthOffset + ShapeParameters.SmallTextBoxWidthBg / 2;
        var xCoordString = boxCenterX - (len.Length() / 2);
        var yCoordString = boxCenterY - ShapeParameters.SmallTextBoxHeightFg / 4;

        DrawSmallTextBox(boxCenterX, boxCenterY, Color.Black);
        _spriteBatch.DrawString(StandardBoldFont, p.PlayerName, new Vector2(xCoordString, yCoordString), Color.Black);
        _spriteBatch.DrawString(StandardBoldFont, p.PlayerHP.ToString(), new Vector2(xCoordHP, yCoordString), Color.Black);
    }

    private void ResetGame()
    {
        PlayerObjectList.Clear();
        AllPlayers.Clear();
        TextBox.Clear();
        Player.NumberOfPlayers = 0;
        ElapsedTime = 0;
    }
}
