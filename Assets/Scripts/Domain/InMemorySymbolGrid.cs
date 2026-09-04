using System;

namespace Game.Domain
{
    public sealed class InMemorySymbolGrid : ISymbolGrid
    {
        private readonly Symbol[] _cells;

        public InMemorySymbolGrid(int width, int height, Symbol[] cells)
        {
            if (width <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(width), width, "Ширина должна быть больше нуля.");
            }

            if (height <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(height), height, "Высота должна быть больше нуля.");
            }

            if (cells == null)
            {
                throw new ArgumentNullException(nameof(cells));
            }

            if (cells.LongLength != (long)width * height)
            {
                throw new ArgumentException(
                    $"Для таблицы {width}×{height} нужно {(long)width * height} ячеек, пришло {cells.LongLength}.",
                    nameof(cells));
            }

            Width = width;
            Height = height;
            _cells = cells;
        }

        public int Width { get; }

        public int Height { get; }

        public Symbol this[GridCoord coord]
        {
            get
            {
                var wrapped = Wrap(coord);
                return _cells[wrapped.Y * Width + wrapped.X];
            }
        }

        public GridCoord Wrap(GridCoord coord)
        {
            return GridWrapping.Wrap(coord, Width, Height);
        }
    }
}
