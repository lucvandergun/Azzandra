using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public class TileDisplay
    {
        public virtual Color? BGColor { get; }
        public virtual Texture2D Texture { get; }

        public virtual bool RenderFire => false;

        public TileDisplay(Color? bgcolor, string texture = null)
        {
            BGColor = bgcolor;
            Texture = texture == null ? null : Assets.GetSprite(texture);
        }

        public TileDisplay()
        {

        }

        public static TileDisplay Get(int id)
        {
            if (TileDisplays.TryGetValue(id, out var display))
                return display;
            else
                return new TileDisplay(Color.MediumOrchid, null);
        }


        private static Dictionary<int, TileDisplay> TileDisplays = new Dictionary<int, TileDisplay>()
        {
            { BlockID.Void, new TileDisplay(Color.Black, null) },
            { BlockID.Wall, new TileDisplay(null, "wall") },
            { BlockID.Pillar, new TileDisplay(null, "pillar") },
            { BlockID.Bricks, new TileDisplay(null, "bricks") },
            { BlockID.Floor, new TileDisplay(null, "floor") },
            { BlockID.Chasm, new TileDisplay(Color.Black, null) },
            { BlockID.Water, new TileDisplay(Color.Navy, "water") },
            { BlockID.Lava, new Lava() }, // new TileDisplay(Color.MonoGameOrange * 0.9f, "lava") }, // MonoGameOrange & Orange
            { BlockID.Sulphur, new TileDisplay(new Color(201, 194, 0), "sulphur") },
            { BlockID.Ice, new TileDisplay(new Color(66, 105, 122), "ice") },
            { BlockID.Poison, new TileDisplay(Color.Green, "water") },
            { BlockID.Dock, new TileDisplay(null, "dock") },
            { BlockID.Rock, new TileDisplay(null, "rock") },
            { BlockID.Mud, new TileDisplay(new Color(17, 13, 10), "floor") },//new TileDisplay(new Color(34, 26, 21), new Symbol('.', new Color(98, 68, 54))) },
            { BlockID.Mushroom, new TileDisplay(null, "mushroom") },
            { BlockID.LightMushroom, new TileDisplay(null, "mushroom_light") },
            { BlockID.Cobweb, new TileDisplay(null, "cobweb") },
            { BlockID.Crystal, new TileDisplay(null, "crystal") },
            { BlockID.Torch, new TileDisplay(null, "torch") },
            { BlockID.Icicle, new TileDisplay(null, "icicle") },
            { BlockID.Plant, new TileDisplay(null, "plant") },
            { BlockID.Root, new TileDisplay(null, "root") },
            { BlockID.Vine, new TileDisplay(null, "vine") },
            { BlockID.Stool, new TileDisplay(null, "stool") },
            { BlockID.Table, new TileDisplay(null, "table") },
            { BlockID.Acid, new TileDisplay(null, "acid") },
        };
        //new Color(51, 39, 31)
        private static Color Wall = new Color(75, 75, 75);
        private static Color Brown = new Color(89, 60, 31);

        public static Color Glacial = new Color(1, 8, 17);
        public static Color Freezing = new Color(3, 20, 28);
        public static Color Cold = new Color(1, 30, 23);
        public static Color Lukewarm = new Color(17, 13, 10);
        public static Color Warm = new Color(1, 22, 5); //new Color(14, 30, 2);
        public static Color Hot = new Color(35, 16, 1);
        public static Color Scorching = new Color(40, 8, 3);

        public static Color GetTempColor(Temp temp)
        {
            switch (temp)
            {
                case Temp.Glacial:
                    return Glacial;
                case Temp.Freezing:
                    return Freezing;
                case Temp.Cold:
                    return Cold;
                default: case Temp.Lukewarm:
                    return Lukewarm;
                case Temp.Warm:
                    return Warm;
                case Temp.Hot:
                    return Hot;
                case Temp.Scorching:
                    return Scorching;
            }
        }

        public class Lava : TileDisplay
        {
            public Lava() { }

            public override Color? BGColor => Color.MonoGameOrange * 0.9f;// ViewHandler.GetLavaColor;
            public override Texture2D Texture => Assets.GetSprite("lava");

            public override bool RenderFire => true;
        }
    }
}
