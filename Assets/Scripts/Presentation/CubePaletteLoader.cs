using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Core;
using Game.Domain;
using UnityEngine;

namespace Game.Presentation
{
    public sealed class CubePaletteLoader
    {
        private readonly IAssetProvider _assetProvider;

        public CubePaletteLoader(IAssetProvider assetProvider)
        {
            _assetProvider = assetProvider;
        }

        public async UniTask<CubePalette> LoadAsync(string paletteAddress, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(paletteAddress))
            {
                throw new ArgumentException("Адрес палитры пустой.", nameof(paletteAddress));
            }

            var paletteAsset = await _assetProvider.LoadAsync<TextAsset>(paletteAddress, cancellationToken);

            cancellationToken.ThrowIfCancellationRequested();

            if (paletteAsset == null)
            {
                throw new InvalidOperationException($"По адресу «{paletteAddress}» нет текстового ассета.");
            }

            var parsed = CubePaletteDefinitionParser.Parse(paletteAsset.text);

            if (!parsed.Success)
            {
                throw new InvalidOperationException(
                    $"Палитра «{paletteAddress}»: {parsed.Error}",
                    parsed.Cause);
            }

            var entries = parsed.Value.entries;
            var requests = new UniTask<Material>[entries.Length];

            for (var index = 0; index < entries.Length; index++)
            {
                requests[index] = _assetProvider.LoadAsync<Material>(entries[index].materialAddress, cancellationToken);
            }

            var materials = await UniTask.WhenAll(requests);

            cancellationToken.ThrowIfCancellationRequested();

            var lookup = new Dictionary<Symbol, Material>(entries.Length);

            for (var index = 0; index < entries.Length; index++)
            {
                lookup[(Symbol)entries[index].symbol] = materials[index];
            }

            return new CubePalette(lookup);
        }
    }
}
