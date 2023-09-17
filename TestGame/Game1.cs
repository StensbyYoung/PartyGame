using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

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
    private GameStates m_State = GameStates.PreGame;
    private string[] testString = {"Marcus", "Mona"};
    private int m_LoopLength;
    private int m_DeviceResY;
    private int m_DeviceResX;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        // TODO: Add your initialization logic here
        Window.AllowUserResizing = true;
        Window.TextInput += TextInputHandler;
        Window.Position = new Point(0,0);
        //SDL_Maximize
        m_DeviceResX = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
        m_DeviceResY = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
        //_graphics.HardwareModeSwitch = false;
        //_graphics.IsFullScreen = true;
        //_graphics.PreferredBackBufferWidth = m_DeviceResX;
        //_graphics.PreferredBackBufferHeight = m_DeviceResY;
        _graphics.ApplyChanges();

        m_LoopLength = -1;

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        whiteRectangle = new Texture2D(GraphicsDevice, 1, 1);
        whiteRectangle.SetData(new[] { Color.White} );
        m_StandardFont = Content.Load<SpriteFont>("standardFont");
        m_StandardBoldFont = Content.Load<SpriteFont>("standardBoldFont");

        // TODO: use this.Content to load your game content here
    }

    protected override void Update(GameTime gameTime)
    {
        if(GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
        {
            Exit();
        }

        switch(m_State)
        {
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
                break;
            case GameStates.Playing:
                break;
            case GameStates.EndGame:
                break;
            default:
                break;
        }

        // TODO: Add your update logic here

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        _spriteBatch.Begin();

        switch(m_State)
        {
            case GameStates.PreGame:
                _spriteBatch.DrawString(m_StandardBoldFont, "X res: " + m_DeviceResX.ToString(), new Vector2(500, 300), Color.Black);
                _spriteBatch.DrawString(m_StandardBoldFont, "Y res: " + m_DeviceResY.ToString(), new Vector2(500, 500), Color.Black);
                break;
            case GameStates.EnteringPlayer:
                _spriteBatch.DrawString(m_StandardBoldFont, m_TextBox, new Vector2(100, 500), Color.Black);
                break;
            case GameStates.Playing:
                break;
            case GameStates.EndGame:
                break;
            default:
                break;
        }

        _spriteBatch.DrawString(m_StandardBoldFont, "State: " + m_State, new Vector2(100, 100), Color.Black);
        _spriteBatch.DrawString(m_StandardBoldFont, "Players: ", new Vector2(100, 300), Color.Black);
        if(m_Players.Count > 0)
        {
            _spriteBatch.DrawString(m_StandardBoldFont, m_Players[m_LoopLength], new Vector2(230, 300), Color.Black);
        }

         //_spriteBatch.DrawString(m_StandardBoldFont, "Players:", new Vector2(100, 300), Color.Black);

        // Example: Draw a rectangle
        //_spriteBatch.Draw(whiteRectangle, new Rectangle(500, 20, 80, 30), Color.Black);
        
        _spriteBatch.End();

        // TODO: Add your drawing code here

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

        m_TextBox.Append(character);
    }
}
