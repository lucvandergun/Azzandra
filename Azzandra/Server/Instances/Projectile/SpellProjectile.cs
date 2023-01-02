using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public class SpellProjectile : TargetProjectileMoving
    {
        public SpellProjectile(Instance origin, Instance target, Color color, string asset = "spell") : base(origin, target, color, asset)
        {
            
        }

        public static Color GetColor(IEnumerable<AttackProperty> props)
        {
            if (props == null) return Color.White;

            foreach (var p in props)
            {
                switch (p.GetID())
                {
                    case AttackPropertyID.Fire:
                        return Color.MonoGameOrange;
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

        //public override void Draw(SpriteBatch sb, Vector2 pos, float lightness)
        //{
        //    base.Draw(sb, pos, lightness);
        //    var a = AnimationManager.Animation.Texture.Name;
        //    //Display.DrawSprite(pos, texture, Color, 1, Angle);
        //}
    }
}
