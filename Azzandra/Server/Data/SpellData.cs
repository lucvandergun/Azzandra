using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public class SpellData
    {
        public string ID, Name, Desc, Type;
        public int SpellPoints, Tier = -1, Weight;

        public SpellData()
        {

        }

        public static readonly SpellData Default = new SpellData()
        {
            ID = "undefined",
            Name = "Undefined",
            Desc = "This is an undefined spell.",
            Type = "Undefined",
            SpellPoints = 1
        };

        public ISpellEffect GetSpellEffect()
        {
            var typeName = "Azzandra.SpellEffects." + ID.ToCamelCase();
            var type = System.Type.GetType(typeName);
            return type != null ? (ISpellEffect)Activator.CreateInstance(type) : null;
        }

        public Color GetStringColor()
        {
            switch (Type)
            {
                default: return Color.White;
                case "defensive": return Color.Lime;
                case "offensive": return Color.LightSeaGreen;
                case "utility": return Color.LightSkyBlue;
            }
        }

    }
}
