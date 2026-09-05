using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Game.Core
{
    public interface IAssetProvider : IDisposable
    {
        UniTask<TAsset> LoadAsync<TAsset>(string address, CancellationToken cancellationToken)
            where TAsset : UnityEngine.Object;
    }
}
