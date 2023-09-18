using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Runtime.InteropServices;
using System.Linq;

namespace TestGame;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    StringBuilder m_TextBox = new StringBuilder();
    Texture2D whiteRectangle;

    private SpriteFont m_StandardFont;
    private SpriteFont m_StandardBoldFont;
    private List<string> m_Players = new List<string>();
    private bool m_EnterPressed;
    private bool m_AddPressed;
    private GameStates m_State = GameStates.Init;
    private string[] testString = {"Marcus", "Mona"};
    private int m_LoopLength;
    private int m_DeviceResY;
    private int m_DeviceResX;

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
        m_DeviceResX = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
        m_DeviceResY = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
        _graphics.PreferredBackBufferWidth = m_DeviceResX;
        _graphics.PreferredBackBufferHeight = m_DeviceResY;
        Window.Position = new Point(0,0);
        SDL_MaximizeWindow(Window.Handle);
        _graphics.ApplyChanges();

        // TODO - not needed for now
        //m_LoopLength = -1;

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        whiteRectangle = new Texture2D(GraphicsDevice, 1, 1);
        whiteRectangle.SetData(new[] { Color.White} );
        m_StandardFont = Content.Load<SpriteFont>("standardFont");
        m_StandardBoldFont = Content.Load<SpriteFont>("standardBoldFont");
    }

    protected override void Update(GameTime gameTime)
    {
        if(GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
        {
            Exit();
        }

        switch(m_State)
        {
            case GameStates.Init:
                if(Keyboard.GetState().IsKeyDown(Keys.Space))
                {
                    m_State = GameStates.PreGame;
                }
                break;
            case GameStates.PreGame:
                if(Keyboard.GetState().IsKeyDown(Keys.C))
                {
                    m_TextBox.Clear();
                }
                if(Keyboard.GetState().IsKeyDown(Keys.Add))
                {
                    m_TextBox.Clear();
                    m_AddPressed = true;
                    m_State = GameStates.EnteringPlayer;
                }
                if(Keyboard.GetState().IsKeyDown(Keys.P))
                {
                    m_State = GameStates.Playing;
                }
                break;
            case GameStates.EnteringPlayer:
                if(Keyboard.GetState().IsKeyDown(Keys.Enter))
                {
                    if(m_TextBox.ToString() != "")
                    {
                        m_LoopLength++;
                        AddPlayer(m_TextBox.ToString());
                    }
                    m_EnterPressed = true;
                    m_State = GameStates.PreGame;
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

        _spriteBatch.DrawString(m_StandardBoldFont, "State: " + m_State, new Vector2(20, 10), Color.Black);
        _spriteBatch.DrawString(m_StandardBoldFont, "X res: " + m_DeviceResX.ToString(), new Vector2(20, 40), Color.Black);
        _spriteBatch.DrawString(m_StandardBoldFont, "Y res: " + m_DeviceResY.ToString(), new Vector2(20, 70), Color.Black);
        _spriteBatch.DrawString(m_StandardBoldFont, "Players: ", new Vector2(20, 100), Color.Black);

        switch(m_State)
        {
            case GameStates.Init:
                break;
            case GameStates.PreGame:
                break;
            case GameStates.EnteringPlayer:
                _spriteBatch.DrawString(m_StandardBoldFont, "Enter player name: ", new Vector2((m_DeviceResX / 2) - (15 * 15 + 10 * 3), (m_DeviceResY / 2)), Color.Black);
                _spriteBatch.DrawString(m_StandardBoldFont, m_TextBox, new Vector2((m_DeviceResX / 2), (m_DeviceResY / 2)), Color.Black);
                _spriteBatch.DrawString(m_StandardBoldFont, m_TextBox.Length.ToString(), new Vector2((m_DeviceResX / 2), (m_DeviceResY / 2) + 40), Color.Black);
                break;
            case GameStates.Playing:
                _spriteBatch.Draw(whiteRectangle, new Rectangle(500, 20, 80, 30), Color.Black);
                break;
            case GameStates.EndGame:
                break;
            default:
                break;
        }

        if(m_Players.Count > 0)
        {
            _spriteBatch.DrawString(m_StandardBoldFont, string.Join(", ", m_Players), new Vector2(140, 100), Color.Black);
        }

        // Example: Draw a rectangle
        //_spriteBatch.Draw(whiteRectangle, new Rectangle(500, 20, 80, 30), Color.Black);
        
        _spriteBatch.End();

        base.Draw(gameTime);
    }

    private void AddPlayer(string player)
    {
        m_Players.Add(player);       
    }

    private void TextInputHandler(object sender, TextInputEventArgs args)
    {
        var pressedKey = args.Key;
        var character = args.Character;


        if(character == (char)Keys.Back)
        {
            var index = m_TextBox.Length - 1;
            if(index >= 0)
            {
                m_TextBox.Remove(index, 1);
            }
        }
        else
        {
            m_TextBox.Append(character);
        }
    }

    private void MaximizeWindow()
    {
        SDL_MaximizeWindow(Window.Handle);
    }
}
