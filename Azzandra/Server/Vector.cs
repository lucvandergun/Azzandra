using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Azzandra
{
    public struct Vector : IEquatable<Vector>
    {
        // Properties and Constructors:
        public int X, Y;
        public Vector(int x, int y) { X = x; Y = y; }
        public Vector(int xy) { X = xy; Y = xy; }

        // Operator Overloads:
        public static Vector operator +(Vector a, Vector b) => new Vector(a.X + b.X, a.Y + b.Y);
        public static Vector operator -(Vector a, Vector b) => new Vector(a.X - b.X, a.Y - b.Y);
        public static Vector operator *(Vector a, Vector b) => new Vector(a.X * b.X, a.Y * b.Y);

        public static Vector operator *(Vector a, int scale) => new Vector(a.X * scale, a.Y * scale);
        public static Vector operator *(int scale, Vector a) => new Vector(a.X * scale, a.Y * scale);
        public static Vector operator /(Vector a, int scale) => new Vector(a.X / scale, a.Y / scale);

        public static bool operator ==(Vector a, Vector b) => a.X == b.X && a.Y == b.Y;
        public static bool operator !=(Vector a, Vector b) => a.X != b.X || a.Y != b.Y;

        public static bool operator >(Vector a, Vector b)=> a.X > b.X && a.Y > b.Y;
        public static bool operator <(Vector a, Vector b) => a.X < b.X && a.Y < b.Y;

        public static bool operator >=(Vector a, Vector b) => a.X >= b.X && a.Y >= b.Y;
        public static bool operator <=(Vector a, Vector b) => a.X <= b.X && a.Y <= b.Y;




        public bool Equals(Vector vector)
        {
            return this == vector;
        }
        public override bool Equals(object obj)
        {
            if (obj is Vector vector)
                return this.Equals(vector);

            return false;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hash = X.GetHashCode();
                hash = (hash * 397) ^ Y.GetHashCode();
                return hash;
            }
        }



        // Checkers:
        public bool IsNull() => IsValue(0, 0);

        public bool IsValue(int x, int y) => X == x && Y == y;
        public bool IsPerfectDiagonal() => X == Y && !IsNull();

        /// <summary>
        /// 
        /// </summary>
        /// <returns>Whether X == 0 or Y == 0. Returns false when both are equal to 0!</returns>
        public bool IsOrthogonal() => !IsNull() && (X == 0 || Y == 0);

        /// <summary>
        /// Returns true if specified pos is in rectangle area
        /// </summary>
        public bool IsInRegion(Region reg) => this >= reg.Position && this < reg.BottomRight;


        // Converters:

        /// <summary>
        /// Creates a new vector with both the X and Y value signed.
        /// </summary>
        /// <returns></returns>
        public Vector Sign()
        {
            return new Vector(Math.Sign(X), Math.Sign(Y));
        }
        public Vector2 ToFloat()
        {
            return new Vector2(X, Y);
        }
        public Dir ToDir()
        {
            return new Dir(X, Y);
        }
        public override string ToString()
        {
            //return "(" + X + ", " + Y + ")";
            return "(X:" + X + ", Y:" + Y + ")";
        }
        public byte[] ToBytes()
        {
            return BitConverter.GetBytes(X).Concat(BitConverter.GetBytes(Y)).ToArray();
        }

        /// <summary>
        /// Creates a new vector object with loaded attributes.
        /// Auto-adjusts pos by +8.
        /// </summary>
        /// <param name="bytes">The byte array to load from.</param>
        /// <param name="pos">The position to load from in the byte array.</param>
        /// <returns>A new vector object.</returns>
        public static Vector Load(byte[] bytes, ref int pos)
        {
            pos += 8;
            return new Vector(BitConverter.ToInt32(bytes, pos - 8), BitConverter.ToInt32(bytes, pos - 4));
        }

        /// <summary>
        /// Returns an absolute version of the vector.
        /// </summary>
        /// <returns>A new Vector without negative values.</returns>
        public Vector Absolute() => new Vector(Math.Abs(X), Math.Abs(Y));
        
        /// <summary>
        /// Returns the absolute length of X and Y combined.
        /// </summary>
        public int OrthogonalLength() => Math.Abs(X) + Math.Abs(Y);

        /// <summary>
        /// Returns a new vector with only the largest component (either X or Y, the other is set to 0).
        /// </summary>
        public Vector Orthogonalize() => Math.Abs(X) >= Math.Abs(Y) ? new Vector(X, 0) : new Vector(0, Y);

        /// <summary>
        /// Calculates the absolute diagonal length of the vector (lower integer bound),
        /// </summary>
        /// <returns>The length as an integer.</returns>
        public int EuclidianLength()
        {
            var v = Absolute();
            return (int)Math.Sqrt(v.X * v.X + v.Y + v.Y);
        }

        public float EuclidianLengthFloat()
        {
            var v = Absolute();
            return (float)Math.Sqrt(v.X * v.X + v.Y + v.Y);
        }


        public static Vector Min(Vector v1, Vector v2)
        {
            return new Vector(Math.Min(v1.X, v2.X), Math.Min(v1.Y, v2.Y));
        }
        public static Vector Max(Vector v1, Vector v2)
        {
            return new Vector(Math.Max(v1.X, v2.X), Math.Max(v1.Y, v2.Y));
        }

        /// <summary>
        /// Creates a new vector that takes the X and Y values closest to zero.
        /// </summary>
        public static Vector Smallest(Vector v1, Vector v2)
        {
            return new Vector(
                Util.ClosestToZero(v1.X, v2.X),
                Util.ClosestToZero(v1.Y, v2.Y));
        }

        /// <summary>
        /// Returns the (absolute) smallest value of either x or y.
        /// </summary>
        /// <returns>Either 0 or a positive integer.</returns>
        public int SmallestValue()
        {
            return Math.Min(Math.Abs(X), Math.Abs(Y));
        }
        /// <summary>
        /// Returns the Chebyshev distance: or largest absolute value of either x or y.
        /// </summary>
        /// <returns>Either 0 or a positive integer.</returns>
        public int ChebyshevLength()
        {
            return Math.Max(Math.Abs(X), Math.Abs(Y));
        }


        public static IEnumerable<Vector> SumTilesInRange(Vector start, Vector size)
        {
            for (int i, j = start.Y; j < start.Y + size.Y; j++)
            {
                for (i = start.X; i < start.X + size.X; i++) 
                {
                    yield return new Vector(i, j);
                }
            }
            yield break;
        }


        // Static Vector Values:
        public static readonly Vector Zero = new Vector(0, 0);
        public static readonly Vector One = new Vector(1, 1);
        public static readonly List<Vector> Dirs4 = new List<Vector>() { new Vector(1, 0), new Vector(0, -1), new Vector(-1, 0), new Vector(0, 1) };
        public static readonly List<Vector> Dirs4Diagonal = new List<Vector>() { new Vector(-1, -1), new Vector(1, -1), new Vector(-1, 1), new Vector(1, 1) };
        public static readonly List<Vector> Dirs5 = new List<Vector>() { new Vector(0, 0), new Vector(1, 0), new Vector(0, -1), new Vector(-1, 0), new Vector(0, 1) };
        public static readonly List<Vector> Dirs8 = new List<Vector>()
            { new Vector(1, 0), new Vector(0, -1), new Vector(-1, 0), new Vector(0, 1), new Vector(1, 1), new Vector(1, -1), new Vector(-1, 1), new Vector(-1, -1)};
        public static readonly List<Vector> Dirs9 = new List<Vector>()
            { new Vector(0, 0), new Vector(1, 0), new Vector(0, -1), new Vector(-1, 0), new Vector(0, 1), new Vector(1, 1), new Vector(1, -1), new Vector(-1, 1), new Vector(-1, -1)};
    }
}
