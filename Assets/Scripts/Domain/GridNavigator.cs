using System;

namespace Game.Domain
{
    public sealed class GridNavigator
    {
        public const int DefaultWindowSize = 3;

        private readonly ISymbolGrid _grid;
        private readonly GridWindow _window;

        public GridNavigator(ISymbolGrid grid, GridCoord start, int windowSize = DefaultWindowSize)
        {
            if (grid == null)
            {
                throw new ArgumentNullException(nameof(grid));
            }

            if (windowSize <= 0 || windowSize % 2 == 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(windowSize), windowSize, "Нужен положительный нечётный размер окна.");
            }

            _grid = grid;
            _window = new GridWindow(windowSize);

            Center = grid.Wrap(start);
            _window.Fill(grid, Center);
        }

        public GridCoord Center { get; private set; }

        public GridWindow Window => _window;

        public void Move(MoveDirection direction)
        {
            Center = _grid.Wrap(direction.Apply(Center));
            _window.Fill(_grid, Center);
        }
    }
}
