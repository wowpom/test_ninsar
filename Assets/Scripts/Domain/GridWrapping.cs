namespace Game.Domain
{
    public static class GridWrapping
    {
        public static GridCoord Wrap(GridCoord coord, int width, int height)
        {
            return new GridCoord(PositiveRemainder(coord.X, width), PositiveRemainder(coord.Y, height));
        }

        public static int PositiveRemainder(int value, int size)
        {
            var remainder = value % size;
            return remainder < 0 ? remainder + size : remainder;
        }
    }
}
