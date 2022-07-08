using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public class User
    {
        public readonly Server Server;

        public string Name { get; private set; } = "Nameless";
        public PlayerClass Class { get; private set; }

        public bool IsCheatMode => Server.GameClient.IsCheatMode;

        public Log Log { get; private set; }
        public UserInventory Inventory { get; private set; }
        public Equipment Equipment { get; private set; }
        public Stats Stats { get; private set; }

        public void LearnSpell(string id)
        {
            if (!LearnedSpells.Any(s => s.ID == id))
                LearnedSpells.Add(new LearnedSpell(id));
        }
        public List<LearnedSpell> LearnedSpells { get; private set; } = new List<LearnedSpell>();
        public int GetLearnedSpcBoost(string id) => LearnedSpells.FirstOrDefault(s => s.ID == id)?.SpcBoost ?? 0;


        public DrinkData[] DrinkEffects { get; set; }


        private Player _player;
        public Player Player
        {
            get => _player;
            set
            {
                if (Player != null)
                    ThrowError("Tried to replace current player object.");
                else if (value == null)
                    ThrowError("Tried to set current player object to null.");
                else
                    _player = value;
            }
        }
        public string Action { get; set; }


        public Instance Target
        {
            get => Player?.Target?.Instance;
            set
            {
                if (Player != null)
                {
                    if (value == null) Player.Target = null;
                    else Player.Target = new InstRef(value);
                }
            }
        }
        public void SetTarget(Instance inst) => Target = inst;


        //public Item ThrowItem;
        //public Spell SelectedSpell;

        // Visibility:
        public VisibilityHandler VisibilityHandler { get; private set; }


        public User(Server server)
        {
            Server = server;
            Item.User = this;   // TODO: in item, replace with: Container.User ?
            Log = server.GameClient.Log;
            Inventory = new UserInventory(this);
            Equipment = new Equipment(this);
            Stats = new Stats(this);
            VisibilityHandler = new VisibilityHandler(Server);

            Name = "Nameless";
            Class = PlayerClass.GetClass(0);
        }

        public void Init(string name, int classNr, int[] stats)
        {
            Name = name ?? "Nameless";
            Class = PlayerClass.GetClass(classNr);
            foreach (var item in Class.StartingItems)
            {
                // Add item to equipment if possible, else if filled add to inventory:
                if (Equipment.CanAddItem(item))
                {
                    if (item is Items.Equipment eqItem)
                    {
                        if (Equipment.GetItemByIndex(eqItem.Slot) == null)
                        {
                            Equipment.AddItem(item);
                            continue;
                        }
                        else if (eqItem is Items.Weapon wepItem && wepItem.CanOffHand && (!(Equipment.GetItemByIndex(eqItem.Slot) is Items.Weapon main) || !main.IsTwoHander))
                        {
                            Equipment.SetIndex(1, wepItem);
                            continue;
                        }
                    }
                }
                Inventory.AddItem(item);
            }

            foreach (var spell in Class.StartingSpells)
                LearnSpell(spell);


            if (stats == null) stats = new int[0];
            for (int i = 0; i < Math.Min(stats.Length, Stats.AMT_OF_SKILLS); i++)
                Stats.SetLevel(i, stats[i]);
        }


        public void Clear()
        {
            Inventory.Clear();
            Equipment.Clear();
            Stats = new Stats(this);
            //Log.Clear();
            Player = null;
            Action = null;
            Target = null;
        }

        public void Victory()
        {
            Server.GameClient.DisplayHandler.Interface = new VictoryInterface(Server.GameClient);
            Server.GameClient.DeleteSaveFile();
            Server.GameState = Server.State.Victory;
        }

        public void GameOver()
        {
            Server.GameClient.DisplayHandler.Interface = new GameOverInterface(Server.GameClient);
            Server.GameClient.DeleteSaveFile();
            Server.GameState = Server.State.GameOver;
        }


        public void Respawn(Level level)
        {
            // This is double when called from player destroy method, but just to be sure.
            level.ActiveInstances.Remove(Player);

            // Create new player object
            var pos = level.StartPosition;
            _player = null;
            SpawnPlayer(level, pos);

            // Save the game!
            Server.GameClient.SaveGame(false);
            UpdateVisibilityMap();

            ShowMessage("<spring>An additional life has been granted to you! Use it well.");
        }

        public void SpawnPlayer(Level world, Vector pos)
        {
            Player = new Player(pos.X, pos.Y, this);
            world.CreateInstance(Player);
        }


        public void UpdateVisibilityMap()
        {
            Vector origin = Player?.Position ?? Vector.Zero;
            VisibilityHandler.Update(origin, Player.GetVisionRange());
        }

        

        // === Saving & Loading === \\
        public byte[] ToBytes()
        {
            var bytes = new byte[20 + 4 + 4];
            int pos = 0;
            bytes.Insert(pos, GameSaver.GetBytes(Name));
            pos += 20;
            bytes.Insert(pos, BitConverter.GetBytes(PlayerClass.GetID(Class)));
            pos += 4;

            // Unlike the other instances, the type of player is not saved - it is always "player" after all.
            var playerBytes = Player.ToBytes();
            bytes.Insert(pos, BitConverter.GetBytes(playerBytes.Length));
            pos += 4;
            bytes = bytes.Concat(playerBytes).ToArray();

            // LearnedSpells 
            var spellBytes = BitConverter.GetBytes(LearnedSpells.Count);
            foreach (var spell in LearnedSpells)
                spellBytes = spellBytes.Concat(spell.ToBytes()).ToArray();
            bytes = bytes.Concat(spellBytes).ToArray();

            // Drinks 
            var drinkBytes = BitConverter.GetBytes(DrinkEffects.Length);
            foreach (var drink in DrinkEffects)
                drinkBytes = drinkBytes.Concat(drink.ToBytes()).ToArray();
            bytes = bytes.Concat(drinkBytes).ToArray();

            return bytes;
        }
        public void Load(byte[] bytes)
        {
            int pos = 0;

            // Load name, etc
            Name = GameSaver.ToString(bytes, pos);
            pos += 20;
            Class = PlayerClass.GetClass(BitConverter.ToInt32(bytes, pos));
            pos += 4;

            // Load player
            int playerBytesAmt = BitConverter.ToInt32(bytes, pos);
            pos += 4;
            var playerBytes = new byte[playerBytesAmt];
            Array.Copy(bytes, pos, playerBytes, 0, playerBytesAmt);
            int p = 0;
            pos += playerBytesAmt;

            // Create and load player object:
            Player = new Player(0, 0, this);         
            Player.Load(playerBytes, ref p);

            // LearnedSpells
            var amt = BitConverter.ToInt32(bytes, pos);
            pos += 4;
            for (int i = 0; i < amt; i++)
                LearnedSpells.Add(new LearnedSpell(bytes, ref pos));

            // Drinks
            amt = BitConverter.ToInt32(bytes, pos);
            pos += 4;
            DrinkEffects = new DrinkData[amt];
            for (int i = 0; i < amt; i++)
                DrinkEffects[i] = new DrinkData(bytes, ref pos);
        }

        public void LoadPlayerToLevel(Level level)
        {
            // Add player to the given level
            level.AddInstance(Player);
            VisibilityHandler.SetupVisibilityMap(level);             // Must this happen here???
            UpdateVisibilityMap();
        }



        // == Messages == \\
        public void ShowMessage(string msg, bool filter = false)
        {
            Server.GameClient.Log.Add(msg, filter);
        }
        public void ThrowDebug(string msg)
        {
            // Don't add message if not on debug mode
            if (!Server.GameClient.IsDebug || !Server.GameClient.IsDevMode)
                return;

            Server.GameClient.Log.Add("<beige>[Debug] " + msg);
        }
        public void ThrowError(string msg)
        {
            Server.GameClient.Log.Add("<rose>[Error] " + msg);
        }
    }
}
