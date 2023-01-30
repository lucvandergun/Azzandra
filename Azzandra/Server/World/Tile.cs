using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public struct Tile : IEquatable<Tile>
    {
        public Block Ground;
        public Block Object;

        public int Marker;

        public Tile(int groundID, int objectID = -1)
        {
            Ground = new Block(groundID);
            Object = new Block(objectID);
            Marker = 0;
        }

        public Tile(int groundID, int groundValue, int objectID, int objectValue)
        {
            Ground = new Block(groundID, groundValue);
            Object = new Block(objectID, objectValue);
            Marker = 0;
        }

        public Tile(Block ground, Block obj)
        {
            Ground = ground;
            Object = obj;
            Marker = 0;
        }


        // == Property Getters == \\
        public bool IsWalkable()
        {
            return Ground.Data.IsWalkable && Object.Data.IsWalkable || Object.ID == BlockID.Dock;
        }

        public bool IsAimable()
        {
            return Ground.Data.IsAimable && Object.Data.IsAimable;
        }

        public bool IsFlyable()
        {
            return Ground.Data.IsFlyable && Object.Data.IsFlyable;
        }

        public bool IsCornerable()
        {
            return Ground.Data.IsCornerable && Object.Data.IsCornerable;
        }

        public int GetLightEmittance()
        {
            return Math.Max(Ground.Data.LightEmittance, Object.Data.LightEmittance);
        }


        // == Event Handlers (Pass-through to sub-tiles) == \\
        
        /// <summary>
        /// Perform on-step actions for certain tiles here. This method should only contain actions that affect the tile.
        /// </summary>
        /// <param name="level"></param>
        /// <param name="pos"></param>
        /// <param name="entity"></param>
        public void OnInstanceWalking(Level level, Vector pos, Instance inst)
        {
            Ground.Data.OnInstanceStep(level, new BlockPos(pos, true), inst);
            Object.Data.OnInstanceStep(level, new BlockPos(pos, false), inst);
        }

        public void OnInstanceStanding(Level level, Vector pos, Instance inst)
        {
            Ground.Data.OnInstanceStanding(level, new BlockPos(pos, true), inst);
            Object.Data.OnInstanceStanding(level, new BlockPos(pos, false), inst);
        }

        public void OnInteraction(Level level, Vector pos, Player player)
        {
            Ground.Data.OnInteraction(level, new BlockPos(pos, true), player);
            Object.Data.OnInteraction(level, new BlockPos(pos, false), player);
        }


        public override bool Equals(object obj)
        {
            if (obj is Tile tile)
                return this.Equals(tile);

            return false;
        }
        public bool Equals(Tile tile)
        {
            return tile.Ground == Ground && tile.Object == Object && tile.Marker == Marker;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hash = Ground.GetHashCode();
                hash = (hash * 47) ^ Object.GetHashCode();
                return hash;
            }
        }

        public static bool operator ==(Tile a, Tile b)
        {
            return a.Ground.ID == b.Ground.ID && a.Object.ID == b.Object.ID;
        }
        public static bool operator !=(Tile a, Tile b)
        {
            return a.Ground.ID != b.Ground.ID || a.Object.ID != b.Object.ID;
        }

        public override string ToString()
        {
            return "Tile(" + Ground + ", " + Object +")";
        }
    }
}
