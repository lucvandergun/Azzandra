using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

namespace Azzandra
{
    public enum Turn { Player, Pet, Enemy }


    public class Engine : Game
    {
        // Save file path:
        public static string SAVE_DIRECTORY = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\.azzandra\\";
        public static string CLIENT_SETTINGS_FILE_NAME = "client_settings.json";
        public static string GAME_VERSION = "Beta 1.0.7";

        public readonly bool IsDevMode = false;

        // Graphics:
        public GraphicsDeviceManager Graphics { get; private set; }
        public SpriteBatch SpriteBatch { get; private set; }
        public const int MIN_SCREEN_WIDTH = 57 * 16, MIN_SCREEN_HEIGHT = 29 * 16;
        public const int DEFAULT_SCREEN_WIDTH = 60 * 16, DEFAULT_SCREEN_HEIGHT = 31 * 16;
        public static int Scale = 1;
        public int
            TrueScreenWidth = DEFAULT_SCREEN_HEIGHT,
            TrueScreenHeight = DEFAULT_SCREEN_HEIGHT;
        public const int FPS = 60;
        public Point ScreenSize => new Point(TrueScreenWidth, TrueScreenHeight);

        public Settings Settings { get; private set; }

        // Scenes:
        public IScene CurrentScene { get; private set; }
        public void SetScene(IScene scene)
        {
            CurrentScene.Exit();
            CurrentScene = scene;
        }


        public Engine()
        {
            Graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            Graphics.PreferredBackBufferWidth = DEFAULT_SCREEN_WIDTH;
            Graphics.PreferredBackBufferHeight = DEFAULT_SCREEN_HEIGHT;

            // Allow resizing:
            Window.AllowUserResizing = true;
            Window.ClientSizeChanged += OnResize;
            
            // Load ClientSettings
            LoadClientSettings();
        }

        public void OnResize(object sender, EventArgs e)
        {
            var bounds = Window.ClientBounds;
            CheckWindowBounds(bounds.Width, bounds.Height);

            // Update settings with possible new startup size
            if (Settings.RememberClientSize)
            {
                Settings.DefaultWindowWidth = bounds.Width;
                Settings.DefaultWindowHeight = bounds.Height;
            }
        }

        public void CheckWindowBounds(int w, int h)
        {
            // Bound by minimum window size:
            if (w < MIN_SCREEN_WIDTH)
            {
                Graphics.PreferredBackBufferWidth = MIN_SCREEN_WIDTH;
                Graphics.ApplyChanges();
            }
            if (h < MIN_SCREEN_HEIGHT)
            {
                Graphics.PreferredBackBufferHeight = MIN_SCREEN_HEIGHT;
                Graphics.ApplyChanges();
            }

            // Attempt to scale the display:
            Scale = ComputeScale(w, h, Settings.AutoScaling);

            // These maximizers are needed as Graphics.ApplyChanges() is only applied later...
            TrueScreenWidth = Math.Max(MIN_SCREEN_WIDTH, w / Scale);
            TrueScreenHeight = Math.Max(MIN_SCREEN_HEIGHT, h / Scale);

            CurrentScene?.OnResize(new Point(TrueScreenWidth, TrueScreenHeight));

            Debug.WriteLine(TrueScreenWidth + " " + TrueScreenHeight + ", scale: " + Scale);
        }

        /// <summary>
        /// Used to update the display after settings have been applied.
        /// </summary>
        public void UpdateDisplay()
        {
            CheckWindowBounds(Window.ClientBounds.Width, Window.ClientBounds.Height);
        }

        public static int ComputeScale(int screenW, int screenH, bool allowScaling)
        {
            int xScale = screenW / MIN_SCREEN_WIDTH;
            int yScale = screenH / MIN_SCREEN_HEIGHT;
            return allowScaling ? Math.Max(1, Math.Min(xScale, yScale)) : 1;
        }

        private void LoadClientSettings()
        {
            // Try to load existing "client_settings" file, else create a new one:
            if (Settings.TryLoad(SAVE_DIRECTORY, CLIENT_SETTINGS_FILE_NAME, out var set))
                Settings = set;
            else
                Settings = new Settings();

            // Update settings start-up window size if desired:
            if (Settings.RememberClientSize)
            {
                Graphics.PreferredBackBufferWidth = Settings.DefaultWindowWidth;
                Graphics.PreferredBackBufferHeight = Settings.DefaultWindowHeight;
                Graphics.ApplyChanges();
            }

            CheckWindowBounds(Settings.DefaultWindowWidth, Settings.DefaultWindowHeight);
        }

        public void SaveClientSettings()
        {
            Settings.Save(SAVE_DIRECTORY, CLIENT_SETTINGS_FILE_NAME);
        }

        public void CloseAndSaveGame()
        {
            SaveClientSettings();
            CurrentScene?.Exit();
            Exit();
        }

        protected override void Initialize()
        {
            base.Initialize();
            CurrentScene = new MenuScene(this, ScreenSize);
        }

        
        protected override void LoadContent()
        {
            SpriteBatch = new SpriteBatch(GraphicsDevice);
            Assets.LoadAssets(Content, GraphicsDevice);
            Data.LoadData();
            BlockID.LoadData();
            Util.NewRandom(1337);
            //Generation.SpawnData.SetupDictionary();
        }

        protected override void UnloadContent() { }


        protected override void Update(GameTime gameTime)
        {
            // Update current mouse and keyboard state:
            Input.Update();

            CurrentScene.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            // Ask current scene to render a texture
            var display = CurrentScene.Render(gameTime, GraphicsDevice, SpriteBatch);

            // Display this texture
            GraphicsDevice.Clear(Color.Black);
            SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, null);

            //Display.DrawSurface(display);
            Display.DrawTexture(Vector2.Zero, display, Scale);

            SpriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
