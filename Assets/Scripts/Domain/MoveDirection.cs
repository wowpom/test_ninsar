using System;

namespace Game.Domain
{
    public enum MoveDirection
    {
        Up,
        Down,
        Left,
        Right,
    }

    public static class MoveDirectionExtensions
    {
        public static GridCoord Apply(this MoveDirection direction, GridCoord coord)
        {
            switch (direction)
            {
                case MoveDirection.Up:
                    return coord.Offset(0, -1);
                case MoveDirection.Down:
                    return coord.Offset(0, 1);
                case MoveDirection.Left:
                    return coord.Offset(-1, 0);
                case MoveDirection.Right:
                    return coord.Offset(1, 0);
                default:
                    throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
            }
        }
    }
}
