using System;

namespace Game.Domain
{
    public readonly struct GridCoord : IEquatable<GridCoord>
    {
        public readonly int X;
        public readonly int Y;

        public GridCoord(int x, int y)
        {
            X = x;
            Y = y;
        }

        public GridCoord Offset(int deltaX, int deltaY)
        {
            return new GridCoord(X + deltaX, Y + deltaY);
        }

        public bool Equals(GridCoord other)
        {
            return X == other.X && Y == other.Y;
        }

        public override bool Equals(object obj)
        {
            return obj is GridCoord other && Equals(other);
        }

        public override int GetHashCode()
        {
            return unchecked((X * 397) ^ Y);
        }

        public override string ToString()
        {
            return $"({X}, {Y})";
        }

        public static bool operator ==(GridCoord left, GridCoord right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(GridCoord left, GridCoord right)
        {
            return !left.Equals(right);
        }
    }
}
