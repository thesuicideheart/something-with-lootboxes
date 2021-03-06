﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

using MonoGame.Extended;
using System.Collections.Generic;
using MysteryBox.Core;
using System.IO;

namespace MysteryBox
{

    public static class Option
    {
        public const int Width = 800;
        public const int Height = 600;
        public const int FPS = 60;
        public const string SaveFolderName = "MysteryBoxSimulator";
        public const string SaveFileName = "Save.xml";
    }

    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        public GraphicsDeviceManager graphics;
        public SpriteBatch spriteBatch;

        public SpriteFont font, TimerFont;

        public InputManager input;

        public static Game1 Instance;

        public State CurrentState;

        public Dictionary<string, State> states = new Dictionary<string, State>();

        public Player player;

        public Core.MessageBox messageBox;

        public InventoryState InventoryState;
        public MainState MainState;
        public OpenCaseState OpenCaseState;
        public TestState TestState;
        public StoreState StoreState;
        public CharSelectState CharSelectState;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            Instance = this;
            messageBox = new Core.MessageBox();
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.xd 
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            if (Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + $"\\{Option.SaveFolderName}\\") == false)
            {
                Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + $"\\{Option.SaveFolderName}\\");
            }
            RPC.Initialize();
            Sprites.Load(Content);
            graphics.PreferredBackBufferWidth = Option.Width;
            graphics.PreferredBackBufferHeight = Option.Height;
            graphics.ApplyChanges();
            this.IsMouseVisible = true;
            this.IsFixedTimeStep = true;
            this.MaxElapsedTime = TimeSpan.FromSeconds(1f / Option.FPS);
            input = new InputManager(this);
            this.Components.Add(input);
            base.Initialize();
            GameData.Init();

            if (File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + $"\\{Option.SaveFolderName}\\{Option.SaveFileName}"))
            {
                player = Player.Load(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + $"\\{Option.SaveFolderName}\\{Option.SaveFileName}");
            }
            else
            {
                player = new Player();
            }


            InventoryState = new InventoryState(player);
            MainState = new MainState(player);
            OpenCaseState = new OpenCaseState(player);
            TestState = new TestState(player);
            StoreState = new StoreState();
            CharSelectState = new CharSelectState(player);

            AddState(InventoryState);
            AddState(MainState);
            AddState(OpenCaseState);
            AddState(TestState);
            AddState(StoreState);
            AddState(CharSelectState);

            SwitchState(GameData.MainState);

        }

        public static void ShowMessageBox(string msg)
        {
            Instance.messageBox.Show(msg);
        }

        protected override void OnExiting(object sender, EventArgs args)
        {
            RPC.Dispose();
            base.OnExiting(sender, args);
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            font = Content.Load<SpriteFont>("font");
            TimerFont = Content.Load<SpriteFont>("TimerFont");

        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
        }

        int timer;
        protected override void Update(GameTime gameTime)
        {

            CurrentState.Update();
            messageBox.Update();

            timer++;

            if (timer % 60 == 0)
            {
                player.Save();
            }

            if (input.IsHeld(Keys.LeftControl) && input.JustPressed(Keys.F))
            {
                this.graphics.ToggleFullScreen();
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);


            spriteBatch.Begin(samplerState: SamplerState.PointClamp);

            spriteBatch.Draw(Sprites.GetTexture("base_background"), new Rectangle(0, 0, Window.ClientBounds.Width, Window.ClientBounds.Height), Color.White);

            CurrentState.Draw(spriteBatch);

            messageBox.Draw(spriteBatch);

            spriteBatch.End();

            base.Draw(gameTime);
        }

        public void SwitchState(string id)
        {
            if (states.ContainsKey(id))
            {
                Console.WriteLine("switched to " + id);
                CurrentState = states[id];
            }
        }

        public void AddState(State state)
        {
            if (!states.ContainsKey(state.ID))
            {
                states.Add(state.ID, state);
            }
        }


        public void draw(Texture2D texture, Rectangle rect)
        {
            spriteBatch.Draw(texture, rect, Color.White);
        }

        public void drawString(string text, int x, int y)
        {
            spriteBatch.DrawString(font, text, new Vector2(x, y), Color.White);
        }
    }
}
