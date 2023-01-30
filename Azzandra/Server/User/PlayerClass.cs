using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    //public enum PlayerClass
    //{
    //    Knight = 0,
    //    Rogue = 1,
    //    Wizard = 2,
    //    Barbarian = 3,
    //    Priest = 4
    //}

    public class PlayerClass
    {
        // Class properties:
        public readonly string Name = "Classless";
        public float
            MeleeAccuracyMod = 1,
            MeleeDamageMod = 1,
            RangedAccuracyMod = 1,
            RangedDamageMod = 1,
            MagicAccuracyMod = 1,
            MagicDamageMod = 1,
            ParryMod = 1,
            BlockMod = 1,
            EvadeMod = 1,

            HealthMod = 1,
            SpellcastMod = 1,
            ResistanceMod = 1;
        public Item[] StartingItems;
        public string[] StartingSpells;

        private PlayerClass(string name, Item[] startingItems, string[] startingSpells = null)
        {
            Name = name;
            StartingItems = startingItems;
            StartingSpells = startingSpells;
        }

        public override string ToString() => Name;

        public static readonly PlayerClass
            Knight = new PlayerClass("Knight",
                new Item[] {
                    Item.Create("iron_broadsword"),
                    Item.Create("iron_shield"),
                    Item.Create("iron_hauberk"),
                    Item.Create("heraldic_cape")
                },
                new string[] {
                    "wind_blast",
                    "weaken"
                })
            { MeleeAccuracyMod = 1.2f, ParryMod = 1.2f, BlockMod = 1.2f },

            Rogue = new PlayerClass("Rogue",
                new Item[] {
                    Item.Create("pine_recurve_bow"),
                    Item.Create("iron_arrow", 75),
                    Item.Create("leather_body"),
                    Item.Create("leather_boots"),
                    Item.Create("iron_dirk")
                },
                new string[] {
                    "dash",
                    "disorient"
                })
            { RangedAccuracyMod = 1.2f, EvadeMod = 1.2f },

            Wizard = new PlayerClass("Wizard",
                new Item[] {
                    Item.Create("wand"),
                    Item.Create("iron_dirk"),
                    Item.Create("wizard_hat"),
                    Item.Create("wizard_socks")
                },
                new string[] {
                    "freeze",
                    "lightning"
                })
            { MagicAccuracyMod = 1.2f, MagicDamageMod = 1.2f, SpellcastMod = 1.2f, ResistanceMod = 1.2f },

            Barbarian = new PlayerClass("Barbarian",
                new Item[] {
                    Item.Create("iron_war_axe"),
                    Item.Create("clothing_straps"),
                    Item.Create("leather_boots")
                },
                new string[] {
                    "charge",
                    "whirlwind"
                })
            { MeleeDamageMod = 1.2f, RangedDamageMod = 1.2f, HealthMod = 1.25f },

            Priest = new PlayerClass("Priest",
                new Item[] {
                    Item.Create("iron_mace"),
                    Item.Create("iron_buckler"),
                    Item.Create("iron_chestplate")
                },
                new string[] {
                    "cure",
                    "deflect"
                })
            { ResistanceMod = 1.4f };

        public static int GetID(PlayerClass c)
        {
            if (c == Rogue) return 1;
            else if (c == Wizard) return 2;
            else if (c == Barbarian) return 3;
            else if (c == Priest) return 4;
            else return 0;
        }

        public static PlayerClass GetClass(int c)
        {
            if (c == 1) return Rogue;
            else if (c == 2) return Wizard;
            else if (c == 3) return Barbarian;
            else if (c == 4) return Priest;
            else return Knight;
        }
    }
}
