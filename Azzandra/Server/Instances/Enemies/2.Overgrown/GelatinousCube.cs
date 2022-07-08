using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public class GelatinousCube : Enemy
    {
        public override EntityType EntityType => EntityType.Acid;
        public override int GetW() => 3;
        public override int GetH() => 3;
        public override int GetMoveDelay() => 2;
        public Inventory Inventory { get; set; } = new Inventory();

        public GelatinousCube(int x, int y) : base(x, y) { }

        public override bool IsInstanceSolidToThis(Instance inst)
        {
            return inst is GelatinousCube;
        }

        public override void OnOtherInstanceCollision(Instance inst)
        {
            base.OnOtherInstanceCollision(inst);
            //if (inst is Entity entity)
            //{
            //    entity.GetHit(Style.Other, entity.Hp);
            //}
            if (inst is GroundItem grit)
            {
                Inventory.AddItem(grit.Item);
                inst.Destroy();
            }
            else if (inst.CanBeDestroyed())
            {
                if (inst is MultipleItemContainer mic)
                    Inventory.AddItems(mic.Inventory.Items);
                else if (inst is SingleItemContainer sic)
                    Inventory.AddItem(sic.Item);

                inst.Destroy();
                if (inst is Player player)
                    player.User.ShowMessage("<red>" + ToStringAdress().CapFirst() + " sucks you in!");
            }
        }

        protected override void ApplyDeathEffects()
        {
            base.ApplyDeathEffects();

            if (!(this is GelatinousCubeSmall))
            {
                var cube = new GelatinousCubeSmall(X + Util.Random.Next(1), Y + Util.Random.Next(1));
                cube.Inventory.AddItems(Inventory.Items);
                Level.CreateInstance(cube);
            }
        }

        //protected override void DropItemsOnDeath()
        //{
        //    foreach (var item in Inventory.Items)
        //        DropItem(item);
        //}

        public override EntityAction DetermineAggressiveAction()
        {
            // Just to make sure: check whether target actually exists
            var target = Target.Combatant;
            if (target == null)
            {
                Target = null;
                return null;
            }

            return new ActionMoveTo(this, target);
        }


        public override Symbol GetSymbol() => new Symbol("G", Color.Aqua);


        // === Saving & Loading === \\
        public override void Load(byte[] bytes, ref int pos)
        {
            int invBytesAmt = BitConverter.ToInt32(bytes, pos);
            pos += 4;
            var invBytes = new byte[invBytesAmt];
            Array.Copy(bytes, pos, invBytes, 0, invBytesAmt);
            Inventory.Load(invBytes);
            pos += invBytesAmt;

            base.Load(bytes, ref pos);
        }

        public override byte[] ToBytes()
        {
            var bytes = new byte[4];
            int pos = 0;

            var invBytes = Inventory.ToBytes();
            int invBytesAmt = invBytes.Length;
            bytes.Insert(pos, BitConverter.GetBytes(invBytesAmt));
            pos += 4;

            return bytes.Concat(invBytes).Concat(base.ToBytes()).ToArray();
        }
    }
}
