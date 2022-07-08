using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public class Server
    {
        // === Core Managers === \\
        public GameClient GameClient { get; private set; }
        public User User { get; private set; }
        public LevelManager LevelManager { get; private set; }
        public enum State { Running, GameOver, Victory };
        public State GameState = State.Running;

        // === Turn Handling === \\
        public const int TURN_SPEED = 12;
        public int TurnDelay { get; private set; }
        public int EnemyTurnDelay { get; private set; }
        public int Turns { get; private set; }
        public Turn CurrentTurn { get; private set; }

        // === Acting === \\
        public bool ReQueueing => GameClient.Engine.Settings.ReQueueing;
        public void SetPlayerAction(EntityAction action) => User.Player.Action = action;


        public Server(GameClient gameClient)
        {
            GameClient = gameClient;
            LevelManager = new LevelManager(this);
            User = new User(this);

            //Initialize();
        }

        /// <summary>
        /// This method is called to initialize the gameloop at a certain point.
        /// (Used upon new creation and when loading the game.
        /// </summary>
        /// <param name="turns">The current amount of turns.</param>
        public void Initialize(int turns = 0)
        {
            // Set all current game's values
            CurrentTurn = Turn.Player;
            Turns = turns;
            TurnDelay = -1;
            EnemyTurnDelay = -1;
        }

        public void GenerateWorld(int? seed = null)
        {
            Initialize();
            LevelManager.GenerateNew(seed);
            //User.VisibilityHandler.SetupVisibilityMap(LevelManager.CurrentWorld);
            //User.UpdateVisibilityMap();
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
            User.Init(playerName, classNr, stats);
            GenerateWorld(seed);
        }

        public void GoToLevel(int depth)
        {
            int d = Math.Sign(depth - LevelManager.Depth);
            if (d > 0)
                LevelManager.GoToNextLevel();
            else if (d < 0)
                LevelManager.GoToPreviousLevel();
            User.UpdateVisibilityMap();
            Turns++;
        }

        public void Update()
        {
            // Player tick start:
            if (TurnDelay == 1)
            {
                LevelManager.CurrentLevel.TickEnd(true);
                User.UpdateVisibilityMap();

            }
            else if (TurnDelay == 0)
            {
                LevelManager.CurrentLevel.TickStart(true);
                User.UpdateVisibilityMap();

                TurnDelay = -1;
            }

            // Player's turn: Wait for input first
            if (TurnDelay <= 0)
            {
                CheckForPlayerInput();
            }

            // Enemy tick end
            if (EnemyTurnDelay == 1)
            {
                LevelManager.CurrentLevel.TickEnd(false);
                User.UpdateVisibilityMap();
            }
            // Enemy's turn - Perform as soon as player's turn delay hits half way point
            else if (EnemyTurnDelay <= 0 && TurnDelay == TURN_SPEED / 2)
            {
                LevelManager.CurrentLevel.TickStart(false);
                EnemyTurnDelay = -1;
                EnemyTick();
                User.UpdateVisibilityMap();
                Turns++;
            }

            // Decrease turntimers:
            if (TurnDelay > 0)
                TurnDelay--;
            if (EnemyTurnDelay > 0)
                EnemyTurnDelay--;
        }

        private void CheckForPlayerInput()
        {
            if (User.Player == null) return;
            
            // Attempt to perform Action, if null, NextAction:
            if (User.Player.Action == null)
            {
                if (!User.Player.PutNextAction())
                    return;
            }

            // Start tick cycle if action was present and could be performed.
            if (User.Player.Action.Perform())
            {
                PlayerTick();
                User.UpdateVisibilityMap();
            }
            else
            {
                User.Player.Action = null;
            }
        }

        private void PlayerTick()
        {
            LevelManager.CurrentLevel.Tick(true);

            TurnDelay = TURN_SPEED;
        }

        public bool IsPlayerTurn => CurrentTurn == Turn.Player && TurnDelay <= 0;

        private void EnemyTick()
        {
            LevelManager.CurrentLevel.Tick(false);

            EnemyTurnDelay = TURN_SPEED;

            LevelManager.CurrentLevel.ReQueueInstances(User.Player);
        }

        // Send messages to client log
        public void ThrowError(string msg)
        {
            User.ThrowError(msg); 
        }
        public void ThrowDebug(string msg)
        {
            User.ThrowDebug(msg);
        }
    }
}
