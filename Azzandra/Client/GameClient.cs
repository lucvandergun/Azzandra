using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public class GameClient : IScene
    {
        public readonly Engine Engine;
        private readonly string SaveFile;

        // Input:
        public InputHandler InputHandler { get; private set; }
        public enum Focus { General, TextInput, Interface }
        public Focus KeyboardFocus { get; set; } = Focus.General;
        public string LastCommand;

        // Graphics:
        public DisplayHandler DisplayHandler { get; private set; }
        public const int GRID_SIZE = 16;

        // Game server:
        public Server Server { get; private set; }
        public bool IsCheatMode { get; private set; } = false;
        public bool IsLighted { get; private set; } = false;
        public bool IsDebug => Engine.Settings.IsDebugMode;
        public bool IsDevMode => Engine.IsDevMode;
        public Log Log { get; private set; }

        public GameClient(Engine engine, Point screenSize, string saveFile)
        {
            Engine = engine;
            SaveFile = saveFile;

            // Setup interfaces
            Log = new Log(this);
            Log.Add("<spring>Welcome to the Caverns of Azzandra.");

            DisplayHandler = new DisplayHandler(this, screenSize);
            InputHandler = new InputHandler(this);
        }

        /// <summary>
        /// This method will try to load the game from a pre-specified save file. Will generate a new one with a random seed if save file not found.
        /// </summary>
        public void LoadGame()
        {
            if (!TryLoadGame())
                CreateNewGame("Unnamed", 0, null, 1337);
        }

        /// <summary>
        /// This method will generate a whole new server instance with a new world.
        /// (Is used to generate a new world by commands.)
        /// </summary>
        /// <param name="seed">The seed the world is based on. Can be left blank to have it create a random one.</param>
        public void CreateNewGame(int? seed = null)
        {
            Server = new Server(this);
            Server.GenerateWorld(seed);
            if (seed != null) Log.Add("<spring>Successfully created a new world with seed: " + seed + ".");
        }

        /// <summary>
        /// This method is called to create a new game world.
        /// First the user is assigned its values, then the world is generated.
        /// </summary>
        /// <param name="playerName">The player's character name.</param>
        /// <param name="classNr">The player's class number.</param>
        /// <param name="stats">The player's stats (length = 7).</param>
        /// <param name="seed">The game seed to be used in generation.</param>
        public void CreateNewGame(string playerName, int classNr, int[] stats, int? seed = null)
        {
            Server = new Server(this);
            Server.CreateNewGame(playerName, classNr, stats, seed);
            if (seed != null) Log.Add("<spring>Successfully created a new world with seed: " + seed + ".");
        }


        private bool TryLoadGame()
        {
            Server = new Server(this);
            var loader = new GameLoader(Server);
            if (loader.TryLoadGame(Engine.SAVE_DIRECTORY, SaveFile))
            {
                Log.Add("Successfully loaded game.");
                return true;
            }
            else
            {
                Log.Add("<red>Failed to load the game!");
                return false;
            }
        }

        public bool SaveGame(bool showMessage = true)
        {
            if (Server == null) return false;
            
            var saver = new GameSaver(Server);
            var s = saver.TrySaveGame(Engine.SAVE_DIRECTORY, SaveFile);
            
            if (showMessage)
            {
                Log.Add(s ? "<spring>Successfully saved game." : "<red>Failed to save game!");
            }

            return s;
        }

        public void DeleteSaveFile()
        {
            if (!Directory.Exists(Engine.SAVE_DIRECTORY))
                Directory.CreateDirectory(Engine.SAVE_DIRECTORY);

            var path = Engine.SAVE_DIRECTORY + SaveFile;
            File.Delete(path);
        }


        public void Update(GameTime gameTime)
        {
            if (KeyboardFocus == Focus.General)
            {
                //Engine.CloseAndSaveGame();
                if (DisplayHandler.Interface == null && DisplayHandler.MouseInterface == null)
                {
                    // Open pause-game interface:
                    if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Input.IsKeyPressed[Keys.Escape])
                        DisplayHandler.Interface = new PausedInterface(this);

                    // Open pause-game interface:
                    if (Input.IsKeyPressed[Keys.OemQuestion] && (Input.IsKeyDown[Keys.LeftShift] || Input.IsKeyDown[Keys.RightShift]))
                    {
                        DisplayHandler.Interface = new ControlsInterface(this);
                    }
                }

                // Toggle debug mode:
                if (Input.IsKeyPressed[Keys.Y])
                    Engine.Settings.IsDebugMode = !IsDebug;

                // Developer commands:
                if (IsDevMode)
                {
                    if (Input.IsKeyPressed[Keys.V])
                    {
                        Server.User.ShowMessage(Util.CastRay(new Vector(0, 0), new Vector(5, 3), true, true).Stringify());
                    }


                    // Open command input interface
                    if (DisplayHandler.ChatInterface == null)
                        if (Input.IsKeyPressed[Keys.OemQuestion] && !Input.IsKeyDown[Keys.LeftShift] && !Input.IsKeyDown[Keys.RightShift])
                            DisplayHandler.ChatInterface = new CommandInput(this);

                    // Toggles:
                    if (Input.IsKeyPressed[Keys.C])
                        IsCheatMode = !IsCheatMode;
                    if (Input.IsKeyPressed[Keys.L])
                    {
                        IsLighted = !IsLighted;
                        DisplayHandler.ViewHandler.OnNewFloor();
                    }

                    // Server settings:
                    if (Server != null)
                    {
                        // Move depth
                        bool prev = Input.IsKeyPressed[Keys.F11], next = Input.IsKeyPressed[Keys.F12];
                        if (prev || next)
                        {
                            int depth = Server.LevelManager.Depth;
                            if (next) depth += 1;
                            else if (prev) depth -= 1;
                            Server.GoToLevel(depth);
                            if (Server.LevelManager.Depth != depth)
                                Server.User.ShowMessage("<medblue>You enter a new floor.");
                        }

                        // Gen DijkstraMap
                        if (Input.IsKeyPressed[Keys.G])
                        {
                            var player = Server.User.Player;
                            if (player != null)
                            {
                                var map = new DijkstraMap(player, new List<Instance>() { player.Level.ActiveInstances.FirstOrDefault(i => i is StairsDown) });
                                map.CreateMap();
                                map.MultiplyWith(-1.2f);
                                map.IterateOverMap();
                                Debug.WriteLine(map.Matrix.Stringify(i => i < 0 ? i.ToString("00.0") : i == 0 ? "    0" : "    x"));
                            }
                        }
                    }
                }

                // Turn handling:
                InputHandler.Update();
            }

            Server?.Update();

            DisplayHandler.Update();
        }

        public void OnResize(Point screenSize)
        {
            DisplayHandler.OnResize(screenSize);
        }

        public RenderTarget2D Render(GameTime gameTime, GraphicsDevice gd, SpriteBatch sb)
        {
            DisplayHandler.RenderDisplay(gameTime);
            return DisplayHandler.MainSurface.Display;
        }

        public void Exit()
        {
            if (Server?.GameState == Server.State.Running)
                SaveGame();
        }
    }
}
