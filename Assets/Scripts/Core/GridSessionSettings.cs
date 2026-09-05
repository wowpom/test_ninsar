using System;
using Game.Domain;

namespace Game.Core
{
    public sealed class GridSessionSettings
    {
        public GridSessionSettings(int windowSize)
        {
            if (windowSize <= 0 || windowSize % 2 == 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(windowSize), windowSize, "Нужен положительный нечётный размер окна.");
            }

            WindowSize = windowSize;
        }

        public GridSessionSettings()
            : this(GridNavigator.DefaultWindowSize)
        {
        }

        public int WindowSize { get; }
    }
}
