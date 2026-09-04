using System;

namespace Game.Domain
{
    public sealed class GridWindow
    {
        private readonly Symbol[] _cells;

        internal GridWindow(int size)
        {
            _cells = new Symbol[size * size];
            Size = size;
        }

        public int Size { get; }

        public GridCoord Center { get; private set; }

        public int CellCount => _cells.Length;

        public Symbol this[int x, int y]
        {
            get
            {
                if (x < 0 || x >= Size)
                {
                    throw new ArgumentOutOfRangeException(nameof(x), x, null);
                }

                if (y < 0 || y >= Size)
                {
                    throw new ArgumentOutOfRangeException(nameof(y), y, null);
                }

                return _cells[y * Size + x];
            }
        }

        internal void Fill(ISymbolGrid grid, GridCoord center)
        {
            var radius = Size / 2;
            Center = center;

            for (var y = 0; y < Size; y++)
            {
                for (var x = 0; x < Size; x++)
                {
                    _cells[y * Size + x] = grid[center.Offset(x - radius, y - radius)];
                }
            }
        }
    }
}
