using System;

namespace Game.Domain
{
    public interface IStartCoordProvider
    {
        GridCoord Provide(ISymbolGrid grid);
    }

    public sealed class RandomStartCoordProvider : IStartCoordProvider
    {
        private readonly Random _random;

        public RandomStartCoordProvider(int? seed = null)
        {
            _random = seed.HasValue ? new Random(seed.Value) : new Random();
        }

        public GridCoord Provide(ISymbolGrid grid)
        {
            if (grid == null)
            {
                throw new ArgumentNullException(nameof(grid));
            }

            return new GridCoord(_random.Next(grid.Width), _random.Next(grid.Height));
        }
    }
}
