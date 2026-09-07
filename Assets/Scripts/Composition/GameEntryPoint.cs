using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Core;
using Game.Domain;
using VContainer.Unity;

namespace Game.Composition
{
    public sealed class GameEntryPoint : IAsyncStartable
    {
        private readonly IGridSource _gridSource;
        private readonly IStartCoordProvider _startCoordProvider;
        private readonly GridSessionController _sessionController;
        private readonly IErrorPresenter _errorPresenter;

        public GameEntryPoint(
            IGridSource gridSource,
            IStartCoordProvider startCoordProvider,
            GridSessionController sessionController,
            IErrorPresenter errorPresenter)
        {
            _gridSource = gridSource;
            _startCoordProvider = startCoordProvider;
            _sessionController = sessionController;
            _errorPresenter = errorPresenter;
        }

        public async UniTask StartAsync(CancellationToken cancellation)
        {
            try
            {
                var result = await _gridSource.LoadAsync(cancellation);

                if (!result.Success)
                {
                    await _errorPresenter.ShowAsync(result.Error, cancellation);
                    return;
                }

                var start = _startCoordProvider.Provide(result.Value);

                await _sessionController.StartAsync(result.Value, start, cancellation);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception exception)
            {
                await _errorPresenter.ShowAsync(
                    $"Не получилось запустить: {exception.Message}",
                    cancellation);
            }
        }
    }
}
