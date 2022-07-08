using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public class CommandInput : TextInput
    {
        private string CurrentCommand;

        public CommandInput(GameClient gameClient) : base(gameClient, "Enter a command:")
        {

        }

        public override void Update()
        {
            base.Update();

            //swap between current & last command
            if (Input.IsKeyPressed[Keys.Up])
            {
                if (GameClient.LastCommand != null)
                {
                    CurrentCommand = Builder.ToString();
                    Builder = new StringBuilder(GameClient.LastCommand);
                }
            }
            else if (Input.IsKeyPressed[Keys.Down])
            {
                if (CurrentCommand != null)
                    Builder = new StringBuilder(CurrentCommand);
                else
                    Builder = new StringBuilder();
            }

            if (CursorTimer > 0) CursorTimer--;
            else CursorTimer = CursorSpeed;
        }

        protected override void Send()
        {
            // Create command from stringbuilder
            var str = Builder.ToString();
            var command = TranslateToCommand(str);

            // Set new previous command
            GameClient.LastCommand = str;
        }

        /// <summary>
        /// Checks whether the command sequence has a minimal additional argument count.
        /// Also displays a message in the log when it returns false.
        /// </summary>
        /// <returns>Whether the minimal additional argument count is met</returns>
        private bool HasAdditionalArguments(String[] c, int minAdditional)
        {
            if (c.Length - 1 < minAdditional)
            {
                //Engine.ThrowError("The \"" + c[0] + "\" command needs at least " + minAdditional + " total additional arguments.");
                return false;
            }

            return true;
        }

        private bool TranslateToCommand(string str)
        {
            var c = Util.SplitWords(str.ToLower());

            if (c.Length > 0)
            {
                switch (c[0])
                {
                    default:
                        ThrowError("unknown command (" + c[0] + ").");
                        break;

                    case "give":
                        if (HasAdditionalArguments(c, 1))
                        {
                            string id = c[1];
                            int qty = (c.Length > 2) ? (int)GetItemQuantity(c[2]) : 1;

                            if (Data.CheckItemExists(id))
                            {
                                var item = Item.Create(id, qty);
                                GameClient.Server.User.Inventory.AddItem(item);
                                GameClient.Server.User.Log.Add("You have been given <aqua>" + item.ToString2() + "<r>.");
                            }
                            else ThrowError("there is no such item as \"" + id + "\".");
                        }
                        else ThrowError("specify an item id.");
                        break;

                    case "equip":
                        if (HasAdditionalArguments(c, 1))
                        {
                            // Item kits
                            if (c[1] == "iron" || c[1] == "steel" || c[1] == "samarite")
                            {
                                string[] items = new string[] { "great_helm", "chestplate", "platelegs", "broadsword", "shield", "sabatons", "gauntlets" };
                                foreach (var item in items)
                                    GameClient.Server.User.Equipment.AddItem(Item.Create(c[1] + "_" + item));
                                GameClient.Server.User.Log.Add("You have been equipped a full set of " + c[1] + " equipment.");
                                break;
                            }
                            
                            // Single specified item
                            string id = c[1];
                            int qty = (c.Length > 2) ? (int)GetItemQuantity(c[2]) : 1;

                            if (Data.CheckItemExists(id))
                            {
                                var item = Item.Create(id, qty);
                                GameClient.Server.User.Equipment.AddItem(item);
                                GameClient.Server.User.Log.Add("You have been equipped <aqua>" + item.ToString2() + "<r>.");
                            }
                            else ThrowError("there is no such item as \"" + id + "\".");
                        }
                        else ThrowError("specify an item id.");
                        break;

                    case "clear":
                        if (HasAdditionalArguments(c, 1))
                        {
                            switch (c[1])
                            {
                                case "inv":
                                case "inventory":
                                    GameClient.Server.User.Inventory.Clear();
                                    break;

                                case "gear":
                                case "eq":
                                case "equip":
                                case "equipment":
                                case "worn":
                                    GameClient.Server.User.Equipment.Clear();
                                    break;

                                case "effects":
                                    GameClient.Server.User.Player.StatusEffects.Clear();
                                    break;

                                case "spells":
                                    GameClient.Server.User.LearnedSpells.Clear();
                                    break;
                            }
                        }
                        else
                            ThrowError("specify an object to clear.");
                        break;

                    case "tp":
                    case "teleport":
                        if (HasAdditionalArguments(c, 2))
                        {
                            if (int.TryParse(c[1], out var x))
                            {
                                if (int.TryParse(c[2], out var y))
                                {
                                    GameClient.Server.User.Player.Position = new Vector(x, y);
                                }
                                else ThrowError("specify an integer y value.");
                            }
                            else ThrowError("specify an integer x value.");
                        }
                        else ThrowError("specify an x and a y value.");
                        break;

                    case "spawn":
                        if (HasAdditionalArguments(c, 1))
                        {
                            var type = InstanceID.GetType2(c[1]);
                            if (type == null)
                            {
                                ThrowError("there is no such instance as \"" + c[1] + "\".");
                                break;
                            }

                            int x = 0, y = 0;
                            if (HasAdditionalArguments(c, 2))
                            {
                                if (int.TryParse(c[2], out var xx))
                                {
                                    x = xx;
                                    if (HasAdditionalArguments(c, 3))
                                    {
                                        if (int.TryParse(c[3], out var yy))
                                            y = yy;
                                        else ThrowError("specify an integer y value.");
                                    }
                                }
                                else ThrowError("specify an integer x value.");
                            }

                            var pos = GameClient.Server.User.Player.Position;
                            var inst = (Instance)Activator.CreateInstance(type, pos.X + x, pos.Y + y);
                            GameClient.Server.LevelManager.CurrentLevel.CreateInstance(inst);
                            GameClient.Server.User.UpdateVisibilityMap();
                        }
                        else ThrowError("specify what entity to spawn.");
                        break;

                    case "effect":
                    case "apply":
                        if (HasAdditionalArguments(c, 1))
                        {
                            var type = Type.GetType("Azzandra.StatusEffects." + c[1].ToCamelCase());
                            if (type == null)
                            {
                                ThrowError("there is no such status effect as \"" + c[1] + "\".");
                                break;
                            }

                            int level = 1, time = 10;
                            if (HasAdditionalArguments(c, 2))
                            {
                                if (int.TryParse(c[2], out var xx))
                                {
                                    level = xx;
                                    if (HasAdditionalArguments(c, 3))
                                    {
                                        if (int.TryParse(c[3], out var yy))
                                            time = yy;
                                        else ThrowError("specify an integer time value.");
                                    }
                                }
                                else ThrowError("specify an integer level value.");
                            }

                            var effect = (StatusEffect)Activator.CreateInstance(type, level, time, null);
                            GameClient.Server.User.Player.AddStatusEffect(
                                effect, true
                                );
                            GameClient.Server.User.Log.Add("<lavender>Applied " + effect.TypeName + effect.GetRomanLevel());
                        }
                        else ThrowError("specify what status effect to add.");
                        break;

                    case "learn":
                        if (HasAdditionalArguments(c, 1))
                        {
                            if (c[1] == "all")
                            {
                                foreach (var spell in Data.GetAllSpells())
                                    GameClient.Server.User.LearnSpell(spell);
                                GameClient.Server.User.ShowMessage("<lavender>You know all spells now!");
                                break;
                            }
                            
                            var data = Data.GetSpellData(c[1]);
                            if (data != null && data != SpellData.Default)
                            {
                                if (!GameClient.Server.User.LearnedSpells.Any(s => s.ID == c[1]))
                                {
                                    GameClient.Server.User.LearnSpell(c[1]);
                                    GameClient.Server.User.ShowMessage("<lavender>You have learned the <aqua>" + data.Name + "<lavender> spell.");
                                }
                                else
                                {
                                    GameClient.Server.User.ShowMessage("<lavender>You now have a better grasp on the " + data.Name + " spell.");
                                }
                            }
                            else ThrowError("there is no such a spell as \"" + c[1] + "\".");
                        }
                        else ThrowError("specify what spell to learn.");
                        break;

                    case "heal":
                        if (HasAdditionalArguments(c, 1))
                        {
                            if (int.TryParse(c[1], out var amt))
                            {
                                int realAmt = GameClient.Server.User.Player.Heal(amt);
                                GameClient.Server.User.Log.Add("<lime>You have been healed for " + realAmt + " health!");
                            }
                            else ThrowError("specify an integer amount to heal.");
                        }
                        else
                        {
                            GameClient.Server.User.Player.Heal(GameClient.Server.User.Player.GetFullHp());
                            GameClient.Server.User.Log.Add("<lime>You have been healed to full health!");
                        }
                        break;

                    case "restore_sp":
                        if (HasAdditionalArguments(c, 1))
                        {
                            if (int.TryParse(c[1], out var amt))
                            {
                                GameClient.Server.User.Player.Sp += amt;
                                GameClient.Server.User.Log.Add("<yellow>You have restored " + amt + " sp!");
                            }
                            else ThrowError("specify an integer amount to restore.");
                        }
                        else
                        {
                            GameClient.Server.User.Player.Sp = GameClient.Server.User.Player.GetFullSp();
                            GameClient.Server.User.Log.Add("<lime>You have restored all of your spellpoints!");
                        }
                        break;

                    case "kill":
                        GameClient.Server.User.Player.Hp = 0;
                        GameClient.Log.Add("<red>You have been slain by the server!");
                        //Engine.Server.PlayerTick()
                        break;

                    case "goto":
                    case "depth":
                        if (HasAdditionalArguments(c, 1))
                        {
                            if (int.TryParse(c[1], out var depth))
                            {
                                if (depth < 0 || depth > 35)
                                {
                                    ThrowError("The depth provided must be larger or equal to 1 and smaller or equal to 35.");
                                    break;
                                }

                                GameClient.Log.Add("<medblue>You have been moved to depth " + depth + ".");
                                GameClient.Server.LevelManager.GoToLevel(depth);
                            }
                            else ThrowError("specify an integer depth.");
                        }
                        break;

                    case "gen":
                    case "generate":
                        int? seed = null;
                        if (HasAdditionalArguments(c, 1))
                        {
                            if (int.TryParse(c[1], out var s))
                                seed = s;
                            else ThrowError("specify an integer depth or leave blank.");
                        }

                        var seedStr = seed == null ? "a random seed" : "seed " + seed;
                        GameClient.Log.Add("<yellow>Generating new world with " + seedStr + "...");
                        GameClient.CreateNewGame(seed);
                        break;

                    case "load":
                        GameClient.LoadGame();
                        break;

                    case "save":
                        GameClient.SaveGame();
                        break;

                    case "set":
                        if (HasAdditionalArguments(c, 1))
                        {
                            if (HasAdditionalArguments(c, 2))
                            {
                                switch (c[1])
                                {
                                    case "stats":
                                    case "skills":
                                    case "all":
                                        if (int.TryParse(c[2], out var lvl))
                                        {
                                            foreach (var stat in SkillID.Names)
                                                GameClient.Server.User.Stats.SetLevel(SkillID.FromString(stat), lvl);
                                            
                                            GameClient.Log.Add("<spring>You have set all your stats to level " + GameClient.Server.User.Stats.GetLevel(0) + "!");
                                            // Heal player if hp is altered
                                            GameClient.Server.User.Player.Heal(GameClient.Server.User.Player.GetFullHp());
                                        }
                                        else ThrowError("specify an integer as value.");
                                        break;

                                    case "hunger":
                                        if (int.TryParse(c[2], out var hunger))
                                        {
                                            GameClient.Server.User.Player.Hunger = hunger;
                                            GameClient.Log.Add("<spring>You have set your hunger to " + GameClient.Server.User.Player.Hunger + "!");
                                        }
                                        else ThrowError("specify an integer as value.");
                                        break;

                                    default:
                                        // Try to read a skill id from word
                                        var id = SkillID.FromString(c[1]);
                                        if (id != -1)
                                        {
                                            if (int.TryParse(c[2], out var lvl2))
                                            {
                                                GameClient.Server.User.Stats.SetLevel(id, lvl2);
                                                GameClient.Log.Add("<spring>You have set your " + SkillID.GetName(id) + " to level " + GameClient.Server.User.Stats.GetLevel(id) + "!");

                                                // Heal player if hp is altered
                                                GameClient.Server.User.Player.Heal(GameClient.Server.User.Player.GetFullHp());
                                            }
                                            else ThrowError("specify an integer as value.");
                                        }
                                        else
                                        {
                                            ThrowError("\'" + c[1] + "\' is not a known skill.");
                                        }
                                        break;
                                }
                            }
                            else ThrowError("specify what level to set to.");
                        }
                        else ThrowError("specify what skill to set.");
                        break;

                        //case "enable":
                        //    if (c.Length > 1)
                        //    {
                        //        switch (c[1])
                        //        {
                        //            case "cheat":
                        //            case "cheats":
                        //            case "cheatmode":
                        //                return new Server.TextCommand.SetCheatMode(1);
                        //        }
                        //    }
                        //    else ThrowError("specify what to enable.");
                        //    break;

                        //case "disable":
                        //    if (c.Length > 1)
                        //    {
                        //        switch (c[1])
                        //        {
                        //            case "cheat":
                        //            case "cheats":
                        //            case "cheatmode":
                        //                return new Server.TextCommand.SetCheatMode(-1);
                        //        }
                        //    }
                        //    else ThrowError("specify what to enable.");
                        //    break;

                        //case "cheat":
                        //case "cheats":
                        //case "cheatmode":
                        //    if (c.Length > 1)
                        //    {
                        //        if (c[1] == "off") return new Server.TextCommand.SetCheatMode(-1);
                        //        else if (c[1] == "on") return new Server.TextCommand.SetCheatMode(1);
                        //        else ThrowError("unknown meaning (" + c[1] + ").");
                        //    }
                        //    else return new Server.TextCommand.SetCheatMode(0);
                        //    break;

                        //case "wait":
                        //case "waitmode":
                        //    if (c.Length > 1)
                        //    {
                        //        if (c[1] == "off") return new Server.Command.SetTickMode(1);
                        //        else if (c[1] == "on") return new Server.Command.SetTickMode(-1);
                        //        else ThrowError("unknown meaning (" + c[1] + ").");
                        //    }
                        //    else return new Server.Command.SetTickMode(-1);
                        //    break;

                        //case "loop":
                        //case "loopmode":
                        //    if (c.Length > 1)
                        //    {
                        //        if (c[1] == "off") return new Server.Command.SetTickMode(-1);
                        //        else if (c[1] == "on") return new Server.Command.SetTickMode(1);
                        //        else ThrowError("unknown meaning (" + c[1] + ").");
                        //    }
                        //    else return new Server.Command.SetTickMode(1);
                        //    break;
                }
            }

            return true;
        }


        private void ThrowError(string str)
        {
            if (str != null)
            {
                GameClient.Log.Add("Command error: <rose>" + str);
            }
        }
    }
}
