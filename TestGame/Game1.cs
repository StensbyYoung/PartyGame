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
    private Texture2D pixel;
    private StringBuilder TextBox = new ();
    private List<string> AllPlayers = new ();
    private GameStates State = GameStates.Init;
    private int DeviceResY;
    private int DeviceResX;
    private List<Player> playerObjectList = new ();
    private Player[] playerObjectArray = new Player[20];

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

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        pixel = new Texture2D(GraphicsDevice, 1, 1);
        pixel.SetData(new[] { Color.White} );
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
                    State = GameStates.EnteringPlayer;
                }
                if(Keyboard.GetState().IsKeyDown(Keys.P))
                {                    
                    for(int i = 0; i < AllPlayers.Count; i++)
                    {
                        playerObjectArray[i] = new Player(AllPlayers[i], Color.Crimson, PlayingMode.Standard);
                    }

                    State = GameStates.Playing;
                }
                break;
            case GameStates.EnteringPlayer:
                if(Keyboard.GetState().IsKeyDown(Keys.Enter))
                {
                    if(TextBox.ToString() != "")
                    {
                        AddPlayer(TextBox.ToString());
                        playerObjectList.Add(new Player(TextBox.ToString(), Color.Black, PlayingMode.Standard));
                    }
                    State = GameStates.PreGame;
                }
                if(Keyboard.GetState().IsKeyDown(Keys.Multiply))
                {
                    ;
                }
                break;
            case GameStates.Playing:
                break;
            case GameStates.EndGame:
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
                foreach(var p in playerObjectList)
                {
                    _spriteBatch.DrawString(StandardBoldFont, p.PlayerName, new Vector2(20, 160 + 30 * p.PlayerID), p.PlayerColor);
                }
                break;
            case GameStates.EnteringPlayer:
                _spriteBatch.DrawString(StandardBoldFont, "Players: ", new Vector2(20, 100), Color.Black);
                _spriteBatch.DrawString(StandardBoldFont, "Enter player name: ", new Vector2((DeviceResX / 2) - (15 * 15 + 10 * 3), (DeviceResY / 2)), Color.Black);
                _spriteBatch.DrawString(StandardBoldFont, TextBox, new Vector2((DeviceResX / 2), (DeviceResY / 2)), Color.Black);
                _spriteBatch.DrawString(StandardBoldFont, TextBox.Length.ToString(), new Vector2((DeviceResX / 2), (DeviceResY / 2) + 40), Color.Black);
                break;
            case GameStates.Playing:                
                foreach(var p in playerObjectList)
                {
                    DrawNameBox(DeviceResX/2, DeviceResY/2, Color.Fuchsia, p);
                }
                break;
            case GameStates.EndGame:
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

    private void DrawNameBox(int boxCenterX, int boxCenterY, Color edgeClr, Player p)
    {
        var len = StandardBoldFont.MeasureString(p.PlayerName);
        var xCoordString = boxCenterX - (len.Length() / 2);
        var yCoordString = boxCenterY - ShapeParameters.SmallTextBoxHeightFg / 4;

        DrawSmallTextBox(boxCenterX, boxCenterY, edgeClr);
        _spriteBatch.DrawString(StandardBoldFont, p.PlayerName, new Vector2(xCoordString, yCoordString), Color.Black);
    }
}
