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
        public const int TURN_SPEED = 16;                   // ### Not used ###
        public const int TICK_POTENTIAL_ADDITION = 1;       // Amt of ActionPotential added to each entity every game update
        public const int TICK_SPEED = 2;                    // Amt of updates it takes to get perform a tick.

        public int TickDelay { get; private set; }
        public int AmtUpdates = 0;
        public int AmtTurns { get; private set; }

        /// <summary>
        /// ~1 = at the start of the turn, i.e. fully away from the end.
        /// 0 = at the end of the turn, i.e. waiting for the next one.
        /// </summary>
        /// <param name="momentOfLastTurn">The server update at which the instance performed its last turn.</param>
        /// <param name="initiative">The amount of ticks (is in ratio to amt of server updates) it takes for this instance to get a turn.</param>
        /// <returns></returns>
        public float GetTickFraction(int momentOfLastTurn, int initiative)
        {
            return Math.Max(0, 1f - ((float)AmtUpdates - momentOfLastTurn) * TICK_SPEED * TICK_POTENTIAL_ADDITION / initiative);
        }

        public int TurnDelay { get; private set; }
        public int EnemyTurnDelay { get; private set; }
        
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
            AmtTurns = turns;
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
            AmtTurns++;
        }



        public void Update()
        {
            var player = User.Player;
            if (player == null) return;

            AmtUpdates++;

            // Perform the tick 
            //if (TickDelay == 0)
            //{
                
            //    TickDelay = -1;
            //}
            //LevelManager.CurrentLevel.TurnEnd();

            // If player can take a turn, allow it to do so first:
            if (player.ActionPotential >= player.Initiative)
            {              
                CheckForPlayerInput();
            }

            LevelManager.CurrentLevel.TurnEnd();

            User.UpdateVisibilityMap();

            // If the player can't take a turn anymore, increase the action potential of all instances:
            if (player.ActionPotential < player.Initiative)
            {
                if (TickDelay <= 0)
                {
                    LevelManager.CurrentLevel.ReQueueInstances(User.Player);
                    LevelManager.CurrentLevel.Turn();
                    TickDelay = TICK_SPEED;
                }
                if (TickDelay > 0) TickDelay--;
            }
        }

        private void CheckForPlayerInput()
        {
            // Attempt to perform 'Action', if null, try 'NextAction':
            if (User.Player.Action == null)
            {
                if (!User.Player.PutNextAction())
                    return;
            }

            // Perform the player's turn if an action was present and it could be performed:
            if (User.Player.Action.Perform())
            {
                User.Player.ActionPotential -= User.Player.Initiative;
                User.Player.Turn();
                User.Player.TimeSinceLastTurn = 0;
                User.Player.MomentOfLastTurn = AmtUpdates;
                AmtTurns++;
                //User.ShowMessage("tick");
                //User.UpdateVisibilityMap();
            }
            else
            {
                User.Player.Action = null;
            }
        }



        // === Error/Debug message handlers === \\
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
