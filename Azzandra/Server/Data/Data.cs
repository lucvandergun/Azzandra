using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public static class Data
    {
        private static Dictionary<string, Item> ItemList;
        private static Dictionary<string, Droptable> Droptables;
        private static Dictionary<string, EnemyData> EnemyDataReferences;
        private static Dictionary<string, SpellData> SpellList;

        //(System.IO.File.ReadAllText("Content/data/enemy_data.json"));
        public static void LoadData()
        {
            ItemList = JsonConvert.DeserializeObject<Dictionary<string, Item>>
                (ReadEmbedded("item_data.json"));

            Droptables = JsonConvert.DeserializeObject<Dictionary<string, Droptable>>
                (ReadEmbedded("droptable_data.json"));

            EnemyDataReferences = JsonConvert.DeserializeObject<Dictionary<string, EnemyData>>
                (ReadEmbedded("enemy_data.json"));

            SpellList = JsonConvert.DeserializeObject<Dictionary<string, SpellData>>
                (ReadEmbedded("spell_data.json"));
            foreach (var spell in SpellList)
                spell.Value.ID = spell.Key;

            Debug.WriteLine("Data loaded:");
            Debug.WriteLine("- Amt items: " + ItemList.Count);
            Debug.WriteLine("- Amt spells: " + SpellList.Count);
            Debug.WriteLine("- Amt enemies: " + EnemyDataReferences.Count);
        }

        private static string ReadEmbedded(string file)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = assembly.GetManifestResourceNames().Single(str => str.EndsWith(file));

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            using (StreamReader reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }


        // === Item data handlers === \\
        public static bool CheckItemExists(string itemID)
        {
            return ItemList.ContainsKey(itemID);
        }
        public static Item GetItemData(string itemID)
        {
            // Try to get item class based on item id
            if (itemID != null && ItemList.TryGetValue(itemID, out var item))
                return item;

            // Return new undefined item
            return Item.Create(itemID);
        }


        // === Droptable data handlers === \\
        public static bool CheckDroptableExists(string droptableID)
        {
            return Droptables.ContainsKey(droptableID);
        }

        /// <summary>
        /// Retrieves the corresponding droptable and rolls a random value with the specified random.
        /// Can return multiple drops, but is usually only one.
        /// </summary>
        /// <param name="droptableID">The droptable to roll from</param>
        /// <param name="random">The random to use</param>
        /// <param name="tier">The tier to limit the rolls to (-1 for no filtering)</param>
        /// <returns>Item array of drops</returns>
        public static Item[] GetDroptableDrop(string droptableID, Random random, int tier = -1)
        {
            if (droptableID != null && Droptables.TryGetValue(droptableID, out var table))
            {
                return table.GetDrop(random, tier);
            }

            return new Item[] { GetItemData("nonexistent_droptable") };
        }


        // === Enemy data handlers === \\
        public static EnemyData GetEnemyData(string enemyID)
        {
            // Try to get enemy data class based on item id
            if (enemyID != null && EnemyDataReferences.TryGetValue(enemyID, out var enemyData))
                return enemyData;

            // Return the default EnemyData if none found
            return EnemyData.Default;
        }


        // === Spell data handlers === \\
        public static SpellData GetSpellData(string spellID)
        {
            // Try to get spell data class based on spell id
            if (spellID != null && SpellList.TryGetValue(spellID, out var spell))
                return spell;

            // Return the default undefined spell if none found
            return SpellData.Default;
        }
        public static SpellData GetSpellData(ISpellEffect effect)
        {
            return GetSpellData(effect == null ? null : Util.ToUnderscore(effect.GetType().Name));
        }

        public static SpellData GetSpellDataRoll(int tier = -1, Random random = null)
        {
            // Compile potentials:
            var potentials = SpellList.Values.ToList().Where(s => s.Tier == tier || tier == -1 || s.Tier == -1);
            return Util.PickItemByWeight(potentials.Select(s => Tuple.Create(s.Weight, s)), random);
        }

        public static string[] GetAllSpells()
        {
            return SpellList.Select(s => s.Value.ID).ToArray();
        }
    }
}
