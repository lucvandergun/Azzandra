using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public class DustCloud : Entity
    {
        public override EntityType EntityType => EntityType.NonPhysical;
        public override int GetMovementSpeed() => 1;
        public override Symbol GetSymbol() => new Symbol('@', Color.Tan.ChangeBrightness(-0.5f));
        public override bool IsSolid() => false;
        public override bool IsInstanceSolidToThis(Instance inst) => false;
        public override MoveType GetMovementType() => MoveType.Fly;

        public int Time = 30;
        public Vector Angle = new Vector(0, -1);

        public DustCloud(int x, int y, Vector angle) : base(x, y)
        {
            Angle = angle;
        }

        public DustCloud(int x, int y) : base(x, y)
        { }

        public override void TurnStart()
        {
            base.TurnStart();

            Time--;
            if (Time <= 0)
                Destroy();
        }

        public override void Turn()
        {
            var pos = Position;
            Move(Angle, false);
            if (pos == Position)
                Destroy();
            
            base.Turn();
        }

        public override void OnCollisionWithInstance(Instance collider)
        {
            if (collider is DustElemental || collider is DustCloud)
                return;

            if (collider.IsAttackable())
                Affect(collider, new DirectDamage(Level.Server, Style.Ranged, 15, true, null));
        }

        public override bool IsImmuneToStatusEffect(int statusID, string name) => true;
        public override bool IsAttackable() => true;


        /// Saving & Loading:
        public override void Load(byte[] bytes, ref int pos)
        {
            // dir
            int x, y;
            x = BitConverter.ToInt32(bytes, pos);
            pos += 4;
            y = BitConverter.ToInt32(bytes, pos);
            pos += 4;
            Angle = new Vector(x, y);
            

            base.Load(bytes, ref pos);
        }

        public override byte[] ToBytes()
        {
            // dir
            var bytes = new byte[8];
            bytes.Insert(0, BitConverter.GetBytes(Angle.X));
            bytes.Insert(4, BitConverter.GetBytes(Angle.Y));

            return bytes.Concat(base.ToBytes()).ToArray();
        }
    }
}
