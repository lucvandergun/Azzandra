using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public class TileDisplay
    {
        public virtual Color BGColor { get; }
        public virtual Symbol Symbol { get; }

        public TileDisplay(Color bgcolor, Symbol symbol = null)
        {
            BGColor = bgcolor;
            Symbol = symbol;
        }

        public TileDisplay()
        {

        }

        public static TileDisplay Get(int id)
        {
            if (TileDisplays.TryGetValue(id, out var display))
                return display;
            else
                return new TileDisplay(Color.MediumOrchid, new Symbol('?', Color.White));
        }


        private static Dictionary<int, TileDisplay> TileDisplays = new Dictionary<int, TileDisplay>()
        {
            { BlockID.Void, new TileDisplay(Color.Black, null) },
            { BlockID.Wall, new TileDisplay(new Color(75, 75, 75), new Symbol('#', new Color(150, 150, 150))) },
            { BlockID.Pillar, new TileDisplay(new Color(200, 200, 200), new Symbol('#', new Color(255, 255, 255))) },
            { BlockID.Bricks, new TileDisplay(new Color(125, 125, 125), new Symbol("[ ]", new Color(200, 200, 200))) },
            { BlockID.Floor, new TileDisplay(Color.Black, new Symbol('.', new Color(63, 63, 63))) },
            { BlockID.Chasm, new TileDisplay(Color.Black, null) },
            { BlockID.Water, new TileDisplay(Color.Navy, new Symbol('~', Color.Blue)) },
            { BlockID.Lava, new TileDisplay(Color.MonoGameOrange * 0.9f, new Symbol('~', Color.Orange)) }, // MonoGameOrange & Orange
            { BlockID.Sulphur, new TileDisplay(Color.Yellow, new Symbol('~', Color.Lime)) },
            { BlockID.Ice, new TileDisplay(new Color(114, 128, 160), new Symbol('.', Color.White)) },
            { BlockID.Poison, new TileDisplay(Color.Green, new Symbol('~', Color.LightGreen)) },
            { BlockID.Dock, new TileDisplay(new Color(64, 32, 0), new Symbol('.', new Color(128, 64, 0))) },
            { BlockID.Rock, new TileDisplay(Color.Black, new Symbol('^', new Color(150, 150, 150))) },
            { BlockID.Mud, new TileDisplay(new Color(17, 13, 10), new Symbol('.', new Color(49, 34, 27))) },//new TileDisplay(new Color(34, 26, 21), new Symbol('.', new Color(98, 68, 54))) },
            { BlockID.Mushroom, new TileDisplay(Color.Black, new Symbol('m', Color.SaddleBrown)) },
            { BlockID.LightMushroom, new TileDisplay(Color.Black, new Symbol('m', Color.DarkCyan.ChangeBrightness(0.75f))) },
            { BlockID.Cobweb, new TileDisplay(Color.Black, new Symbol('#', new Color(200, 200, 200))) },
            { BlockID.Crystal, new TileDisplay(Color.Black, new Symbol('^', Color.Fuchsia)) },
            { BlockID.Torch, new TileDisplay(Color.Black, new Symbol('I', Color.Gold)) },
            { BlockID.Icicle, new TileDisplay(Color.Black, new Symbol('^', Color.SkyBlue)) },
            { BlockID.Plant, new TileDisplay(Color.Black, new Symbol('"', Color.ForestGreen)) },
            { BlockID.Root, new TileDisplay(Color.Black, new Symbol('~', new Color(64, 32, 0))) },
            { BlockID.Vine, new TileDisplay(Color.Black, new Symbol('&', Color.SeaGreen)) },
            { BlockID.Bench, new TileDisplay(Color.Black, new Symbol('n', new Color(89, 60, 31))) }, // π
            { BlockID.Table, new TileDisplay(Color.Black, new Symbol('T', new Color(89, 60, 31))) }, // π
            { BlockID.Acid, new TileDisplay(Color.Black, new Symbol('_', Color.YellowGreen)) }, // π
        };
        //new Color(51, 39, 31)
        private static Color Wall = new Color(75, 75, 75);
        private static Color Brown = new Color(89, 60, 31);

        public class Lava : TileDisplay
        {
            public Lava() { }

            public override Color BGColor => ViewHandler.GetLavaColor;
            public override Symbol Symbol => new Symbol('~', ViewHandler.GetLavaColor);
        }
    }
}
