using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public class ActionThrow : ActionVector
    {
        public Item Item;

        public ActionThrow(Entity caller, Vector target, Item item) : base(caller, target)
        {
            Item = item;
        }

        protected override bool PerformAction()
        {
            var player = Caller as Player;
            
            if (Item == null)
            {
                player?.User.ThrowError("the throw-item does not exist!");
                return false;
            }

            // Remove 1 x item from its container:
            Item.RemoveAmount(1);
            var item = Item.Create(Item.ID, 1);

            // Show message
            var msg = (player == null ? Caller.ToStringAdress().CapFirst() + " throws" : "You throw") + " the " + item.ToString() + ".";
            Caller.Level.Server.User.ShowMessage(msg);

            // Create a grit:
            var grit = new GroundItem(Caller.X, Caller.Y, item);
            Caller.Level.CreateInstance(grit);

            // Move it towards the spot:
            //var oldPos = grit.Position;
            //grit.Position = Target;
            var dist = Target - grit.Position;
            grit.Move(new List<Vector>() { dist }, true, false, true);


            // Try to affect any instance it lands on:
            var pos = grit.Position;
            var instances = Caller.Level.ActiveInstances.Where(i => i.GetTiles().Contains(pos)).ToList();
            for (int i = 0; i < instances.Count(); i++)
            {
                if (item.OnThrowOnInstance(Caller.Level, grit, instances[i]))
                    return true;
            }

            // Otherwise try to affect the tile it lands on:
            item.OnThrowOnTile(Caller.Level, grit, pos);

            return true;
        }
    }
}
