using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Core;
using UnityEngine;

namespace Game.Presentation
{
    public sealed class ErrorOverlayPresenter : IErrorPresenter, IDisposable
    {
        private const string OverlayAddress = "ErrorOverlay";
        private const string EmptyMessageFallback = "Ошибка без текста.";

        private readonly IAssetProvider _assetProvider;

        private ErrorOverlayView _view;
        private UniTask<ErrorOverlayView> _spawnTask;
        private bool _spawnStarted;
        private bool _disposed;

        public ErrorOverlayPresenter(IAssetProvider assetProvider)
        {
            _assetProvider = assetProvider;
        }

        public async UniTask ShowAsync(string message, CancellationToken cancellationToken)
        {
            var text = string.IsNullOrWhiteSpace(message) ? EmptyMessageFallback : message;

            Debug.LogError(text);

            if (_disposed)
            {
                return;
            }

            var view = await SpawnAsync(cancellationToken);

            if (_disposed)
            {
                return;
            }

            view.Apply(text);
        }

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;

            if (_view != null)
            {
                UnityEngine.Object.Destroy(_view.gameObject);
                _view = null;
            }
        }

        private UniTask<ErrorOverlayView> SpawnAsync(CancellationToken cancellationToken)
        {
            if (_view != null)
            {
                return UniTask.FromResult(_view);
            }

            if (!_spawnStarted)
            {
                _spawnStarted = true;
                _spawnTask = SpawnOnceAsync(cancellationToken).Preserve();
            }

            return _spawnTask;
        }

        private async UniTask<ErrorOverlayView> SpawnOnceAsync(CancellationToken cancellationToken)
        {
            var prefab = await _assetProvider.LoadAsync<GameObject>(OverlayAddress, cancellationToken);

            cancellationToken.ThrowIfCancellationRequested();

            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(ErrorOverlayPresenter));
            }

            if (prefab.GetComponent<ErrorOverlayView>() == null)
            {
                throw new InvalidOperationException(
                    $"На префабе «{OverlayAddress}» нет {nameof(ErrorOverlayView)}.");
            }

            var instance = UnityEngine.Object.Instantiate(prefab);
            instance.name = OverlayAddress;

            if (_disposed)
            {
                UnityEngine.Object.Destroy(instance);
                throw new ObjectDisposedException(nameof(ErrorOverlayPresenter));
            }

            _view = instance.GetComponent<ErrorOverlayView>();
            return _view;
        }
    }
}
