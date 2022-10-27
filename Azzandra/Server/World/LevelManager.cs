using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public enum Temp { Glacial, Freezing, Cold, Lukewarm, Warm, Hot, Scorching }
    
    public class LevelManager
    {
        public Server Server;
        
        public const int MAX_DEPTH = 30;

        public readonly int[] AVERAGE_TEMP = new int[MAX_DEPTH]
        { 
            // Freezing floors:
            2, 2, 2, 1, 1, 0, 0, 0, 1, 1,
            
            // Overgrown floors:
            2, 2, 4, 3, 3, 5, 4, 4, 4, 4,

            // Fiery floors:
            4, 5, 4, 5, 5, 5, 6, 6, 6, 6
        };

        private static int GenerateGameSeed() => Util.Random.Next(-9999, 10000);
        private static int GenerateLevelSeed(Random random) => random.Next(-9999, 10000);

        public int GameSeed { get; private set; }
        public Random Random { get; private set; }
        public int Depth { get; private set; }
        public int InstanceIDCounter { get; private set; } = 0;
        public int GetUniqueInstanceID() => InstanceIDCounter++;

        public int[] LevelSeeds { get; private set; }
        public Temp[] LevelTemperatures { get; private set; }
        public Level[] Levels { get; private set; }

        public Level CurrentLevel => Depth >= 1 && Depth <= MAX_DEPTH ? Levels[Depth - 1] : null;



        // Benefit value - used to determine how many good items can be spawned.
        public int BenefitValue { get; private set; }
        public const int LEVEL_BENEFIT = 15;
        /// <summary>
        /// Removes a certain amount from the remaining benefit value.
        /// </summary>
        /// <param name="amt">The amount to remove (positive).</param>
        /// <returns>The amount removed.</returns>
        public int RemoveBenefit(int amt)
        {
            int amtRemoved = Math.Min(BenefitValue, Math.Max(0, amt));
            BenefitValue -= amtRemoved;
            return amtRemoved;
        }


        public LevelManager(Server server)
        {
            Server = server;

            // Create arrays
            Levels = new Level[MAX_DEPTH];
            LevelSeeds = new int[MAX_DEPTH];
            LevelTemperatures = new Temp[MAX_DEPTH];
        }

        public void GenerateNew(int? seed = null)
        {
            GameSeed = seed != null ? seed.Value : GenerateGameSeed();
            Random = new Random(GameSeed);

            // Generate main level data
            CreateListOfSeeds();
            CreateListOfTemperatures();

            // Assign Drink Effects
            Server.User.DrinkEffects = DrinkData.AssignDrinkEffects(Random);

            // Go to first level
            CreateFirstLevel();
        }

        private void CreateListOfSeeds()
        {
            for (int i = 0; i < MAX_DEPTH; i++)
            {
                LevelSeeds[i] = GenerateLevelSeed(Random);
            }
        }

        private void CreateListOfTemperatures()
        {
            // Decide upon depth biome change
            //int cold, overgrown, fiery;
            //cold = 1;
            //overgrown = Random.Next(9, 12);
            //fiery = Random.Next(19, 22);

            var levelTemperatures = new int[MAX_DEPTH];

            // Set initial temperature:
            levelTemperatures[0] = 3;

            // Loop through depths and assign temperatures:
            for (int i = 1; i < MAX_DEPTH; i++)
            {
                var newTemp = ComputeTemperature(levelTemperatures[i - 1], AVERAGE_TEMP[i]);
                levelTemperatures[i] = Math.Min(6, Math.Max(0, newTemp));
                LevelTemperatures[i] = (Temp)newTemp;
            }

            // Assign enum temp value from int
            for (int i = 0; i < MAX_DEPTH; i++)
            {
                LevelTemperatures[i] = (Temp)levelTemperatures[i];
            }
        }

        private int ComputeTemperature(int current, int nextAverage)
        {
            int dTemp = nextAverage - current;
            int randomValue = Random.Next(12);

            if (Math.Abs(dTemp) > 1)
            {
                return current + Math.Sign(dTemp);
            }
            else if (Math.Abs(dTemp) == 1)
            {
                return randomValue <= 6 ? current + Math.Sign(dTemp) : current;
            }
            else
            {
                return randomValue <= 4 ? current + Math.Sign(dTemp) : randomValue <= 8 ? current - Math.Sign(dTemp) : current;
            }
        }

        private void CreateFirstLevel()
        {
            // Create new level/world:
            Depth = 1;
            var level = CreateLevel(1);

            // Spawn player:
            Server.User.SpawnPlayer(level, level.StartPosition);

            // Setup user's visibility map of level
            Server.User.VisibilityHandler.SetupVisibilityMap(CurrentLevel);
            Server.User.UpdateVisibilityMap();
        }

        private Level CreateLevel(int depth)
        {
            if (depth < 1 || depth > MAX_DEPTH)
                return null;

            //BenefitValue += LEVEL_BENEFIT;
            //var level = new Level(Server, depth, LevelTemperatures[depth - 1], LevelSeeds[depth - 1]);
            //level.Generate();
            //Levels[depth - 1] = level;

            // Generation time-out system:
            Level level = null;
            int benefit = BenefitValue + LEVEL_BENEFIT;
            while (true)
            {
                BenefitValue = benefit;
                level = new Level(Server, depth, LevelTemperatures[depth - 1], LevelSeeds[depth - 1]);

                var task = Task.Run(() => level.Generate());
                if (task.Wait(TimeSpan.FromSeconds(10)))
                    break;
                else
                {
                    LevelSeeds[depth - 1]++;
                    Server.User.ShowMessage("gen timed out.. re-attempting..");
                    Debug.WriteLine("ree-attempting: " + level + "");
                }
            }

            Debug.WriteLine("level: " + level + "");
            Levels[depth - 1] = level;
            return level;
        }

        public bool GoToLevel(int depth)
        {
            if (depth == Depth)
                return false;

            // Set bounds to accessible depths - create level if not yet present
            int newDepth = Math.Min(MAX_DEPTH, Math.Max(1, depth));
            if (Levels[newDepth - 1] == null)
            {
                CreateLevel(newDepth);
            }

            // Move player to new world
            var player = Server.User.Player;
            CurrentLevel?.ActiveInstances.Remove(player);

            bool moveDown = newDepth >= Depth;
            Depth = newDepth;

            // Move player to the corresponding stairs:
            player.Position = moveDown ? CurrentLevel.StartPosition : CurrentLevel.EndPosition;
            CurrentLevel.AddInstance(player);    // this sets player.Level to new world
            
            Server.User.VisibilityHandler.SetupVisibilityMap(CurrentLevel);
            Server.User.UpdateVisibilityMap();

            // Remove all previous tile displays
            Server.GameClient.DisplayHandler.ViewHandler.OnNewFloor();

            // Save the current game!
            Server.GameClient.SaveGame(false);

            // Return true if was able to move to requested level:
            //  newDepth is changed if it wasn't possible
            return newDepth == depth;
        }

        public bool GoToNextLevel() => GoToLevel(Depth + 1);
        public bool GoToPreviousLevel() => GoToLevel(Depth - 1);

        public byte[] ToBytes()
        {
            // Save general data
            var bytes = new byte[16];
            bytes.Insert(0, BitConverter.GetBytes(GameSeed));
            bytes.Insert(4, BitConverter.GetBytes(Depth));
            bytes.Insert(8, BitConverter.GetBytes(InstanceIDCounter));
            bytes.Insert(12, BitConverter.GetBytes(BenefitValue));

            int amt = 0;

            // Save levels
            for (int i = 0; i < MAX_DEPTH; i++)
            {
                var lvlBytes = Levels[i]?.ToBytes();

                // General level data
                var subBytes = new byte[12];
                subBytes.Insert(0, BitConverter.GetBytes(LevelSeeds[i]));
                subBytes.Insert(4, BitConverter.GetBytes((int)LevelTemperatures[i]));

                if (lvlBytes != null)
                {
                    var lvlBytesAmt = lvlBytes != null ? lvlBytes.Length : 0;
                    subBytes.Insert(8, BitConverter.GetBytes(lvlBytesAmt));
                    //Debug.WriteLine(" - Level " + (i + 1) + ": " + lvlBytesAmt);

                    subBytes = subBytes.Concat(lvlBytes).ToArray();
                    amt++;
                }
                
                bytes = bytes.Concat(subBytes).ToArray();
            }

            Debug.WriteLine(" - Total levels saved: " + amt);
            return bytes;
        }

        public void Load(byte[] bytes)
        {
            int pos = 0;

            // Load general data
            GameSeed = BitConverter.ToInt32(bytes, pos);
            pos += 4;
            Depth = BitConverter.ToInt32(bytes, pos);
            pos += 4;
            InstanceIDCounter = BitConverter.ToInt32(bytes, pos);
            pos += 4;
            BenefitValue = BitConverter.ToInt32(bytes, pos);
            pos += 4;

            int amt = 0;

            for (int i = 0; i < MAX_DEPTH; i++)
            {
                LevelSeeds[i] = BitConverter.ToInt32(bytes, pos);
                pos += 4;
                LevelTemperatures[i] = (Temp)BitConverter.ToInt32(bytes, pos);
                pos += 4;
                var lvlBytesAmt = BitConverter.ToInt32(bytes, pos);
                pos += 4;

                // Level itself - add if exists
                if (lvlBytesAmt > 0)
                {
                    // Copy level byte section
                    var lvlBytes = new byte[lvlBytesAmt];
                    Array.Copy(bytes, pos, lvlBytes, 0, lvlBytesAmt);

                    // Create level
                    var level = new Level(Server, i + 1, LevelTemperatures[i], LevelSeeds[i]);
                    level.Load(lvlBytes);
                    Levels[i] = level;

                    // Load player into level:
                    if (level == CurrentLevel)
                        Server.User.LoadPlayerToLevel(level);

                    level.LoadInstRefs();
                    
                    pos += lvlBytesAmt;

                    amt++;
                    //Debug.WriteLine(" - Level " + (i+1) + ": " + lvlBytesAmt);

                }
            }

            Debug.WriteLine(" - Total levels loaded: " + amt);
            Debug.WriteLine(" - Entering depth: " + Depth);
            Server.GameClient.DisplayHandler.ViewHandler.OnNewFloor();
        }
    }
}
