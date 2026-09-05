using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Domain;

namespace Game.Core
{
    public sealed class GridSessionController : IDisposable
    {
        private readonly IMoveInput _input;
        private readonly IGridView _view;
        private readonly GridSessionSettings _settings;

        private GridNavigator _navigator;
        private bool _disposed;

        public GridSessionController(IMoveInput input, IGridView view, GridSessionSettings settings)
        {
            _input = input;
            _view = view;
            _settings = settings;
        }

        public async UniTask StartAsync(ISymbolGrid grid, GridCoord start, CancellationToken cancellationToken)
        {
            if (_navigator != null)
            {
                throw new InvalidOperationException("Сессия уже идёт.");
            }

            await _view.BuildAsync(_settings.WindowSize, cancellationToken);

            _navigator = new GridNavigator(grid, start, _settings.WindowSize);
            _view.Render(_navigator.Window);

            _input.Moved += OnMoved;
            _input.Enable();
        }

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;

            _input.Moved -= OnMoved;
            _input.Disable();
        }

        private void OnMoved(MoveDirection direction)
        {
            if (_disposed || _navigator == null)
            {
                return;
            }

            _navigator.Move(direction);
            _view.Render(_navigator.Window);
        }
    }
}
