using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Core;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Game.Infrastructure
{
    public sealed class AddressablesAssetProvider : IAssetProvider
    {
        private readonly List<AsyncOperationHandle> _handles = new List<AsyncOperationHandle>();

        private bool _disposed;

        public async UniTask<TAsset> LoadAsync<TAsset>(string address, CancellationToken cancellationToken)
            where TAsset : UnityEngine.Object
        {
            if (string.IsNullOrWhiteSpace(address))
            {
                throw new ArgumentException("Адрес ассета пустой.", nameof(address));
            }

            if (_disposed)
            {
                throw new ObjectDisposedException(
                    nameof(AddressablesAssetProvider),
                    "Провайдер уже освобождён.");
            }

            var handle = Addressables.LoadAssetAsync<TAsset>(address);
            TAsset asset;

            try
            {
                asset = await handle.ToUniTask(cancellationToken: cancellationToken);
            }
            catch (OperationCanceledException)
            {
                ReleaseIfValid(handle);
                throw;
            }
            catch (Exception exception)
            {
                ReleaseIfValid(handle);
                throw new InvalidOperationException(
                    $"Не загрузился ассет «{address}».",
                    exception);
            }

            if (_disposed)
            {
                ReleaseIfValid(handle);
                throw new ObjectDisposedException(
                    nameof(AddressablesAssetProvider),
                    $"Провайдер освободили, пока грузился «{address}».");
            }

            _handles.Add(handle);

            return asset;
        }

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;

            foreach (var handle in _handles)
            {
                ReleaseIfValid(handle);
            }

            _handles.Clear();
        }

        private static void ReleaseIfValid(AsyncOperationHandle handle)
        {
            if (handle.IsValid())
            {
                Addressables.Release(handle);
            }
        }
    }
}
