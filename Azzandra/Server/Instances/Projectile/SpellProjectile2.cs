using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public class SpellProjectile2 : TargetProjectileMoving
    {
        protected Symbol Symbol;

        public static Color GetColor(IEnumerable<AttackProperty> props)
        {
            if (props == null) return Color.White;

            foreach (var p in props)
            {
                switch (p.GetID())
                {
                    case AttackPropertyID.Fire:
                        return Color.OrangeRed;
                    case AttackPropertyID.Frost:
                        return Color.LightBlue;
                    case AttackPropertyID.Shadow:
                        return Color.Purple;
                    case AttackPropertyID.Poison:
                        return Color.Green;
                    default:
                    case AttackPropertyID.Knockback:
                        return Color.White;
                }
            }

            return Color.White;
        }

        public SpellProjectile2(Instance origin, Instance target, Symbol symbol) : base(origin, target, Color.White, "spell")
        {
            Symbol = symbol;
        }

        public override void Draw(SpriteBatch sb, Vector2 pos, float lightness)
        {
            Display.DrawInstanceString(pos, Symbol.Char, Assets.Gridfont, Symbol.Color, 1, 0f, true);
        }
    }
}
