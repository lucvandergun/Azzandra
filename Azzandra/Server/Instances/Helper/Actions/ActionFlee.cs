using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public class ActionFlee : EntityAction
    {
        public readonly List<Instance> Threats;
        public List<Vector> LastLocations;
        public DijkstraMap FleeMap;

        public ActionFlee(Entity caller, Instance threat) : base(caller)
        {
            Threats = new List<Instance>() { threat };
            Setup();
            // Add Flee distance-threshold (when fleeing stops/is satisfied). Or when certain combined value is reached, or when close enough to other enemies.
        }
        public ActionFlee(Entity caller, List<Instance> threats) : base(caller)
        {
            Threats = threats;
            Setup();
        }

        protected void Setup()
        {
            LastLocations = Threats.Select(t => t.Position).ToList();
            FleeMap = new DijkstraMap(Caller, Threats);
            FleeMap.CreateMap();
            FleeMap.MultiplyWith(-1.5f);
            FleeMap.IterateOverMap();
        }

        protected override bool PerformAction()
        {
            // Compute the dist to move:
            Vector dist;
            if (Threats.All(t => Caller.TileDistanceTo(t) >= DijkstraMap.RANGE))
            {
                dist = (Caller.Position - Threats[0].Position);
                dist = Vector.Smallest(dist, dist.Sign() * Caller.GetMovementSpeed());

                if (dist == Vector.Zero)
                    dist = Dir.Random.ToVector();
            }
            else
            {
                // Update if necessary
                if (CheckTargetLocChange())
                    Setup();

                // Retrieve the next step from the dijkstra map:
                var step = FleeMap.GetStep();
                if (step == null)
                    return false;

                dist = step; //.Value
            }

            // Try to move dist, or open door if its in the way:
            if (!Caller.CanMoveUnobstructed(dist.X, dist.Y))
            {
                if (Caller.CanOpenDoors())
                {
                    var door = Caller.Level.ActiveInstances.FirstOrDefault(i => i.Position == dist + Caller.Position && i is Door d && d.CanBeOpened());
                    if (door != null)
                    {
                        new ActionInteract(Caller, door).Perform();
                        Caller.NextAction = this;
                        return true;
                    }
                }
                Caller.NextAction = this;
                return false;
            }
            else
            {
                Caller.NextAction = this;
                return new ActionMove(Caller, dist, false).Perform();

                // Stop condition - if value on dijkstra map is of certain magnitude?
                //if (FleeMap.GetValue(Caller.Position) <= DijkstraMap.RANGE)
                //    Caller.NextAction = this;
            }
        }

        private bool CheckTargetLocChange()
        {
            for (int i = 0; i < Threats.Count(); i++)
            {
                if (Threats[i].Position != LastLocations[i])
                    return true;
            }
            return false;
        }

        public override string ToString()
        {
            return "Flee from " + Util.Stringify2(FleeMap.Targets);
        }
    }
}
