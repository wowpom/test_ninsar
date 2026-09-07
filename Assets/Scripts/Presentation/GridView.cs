using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Core;
using Game.Domain;
using UnityEngine;
using VContainer;

namespace Game.Presentation
{
    public sealed class GridView : MonoBehaviour, IGridView
    {
        private const string CubePrefabAddress = "Cube";
        private const string CubePaletteAddress = "CubePalette";
        private const float CellSpacing = 1.25f;

        private IAssetProvider _assetProvider;
        private CubePaletteLoader _paletteLoader;
        private CubePalette _palette;
        private CubeView[] _cubes;
        private int _windowSize;

        [Inject]
        public void Construct(IAssetProvider assetProvider, CubePaletteLoader paletteLoader)
        {
            _assetProvider = assetProvider;
            _paletteLoader = paletteLoader;
        }

        public async UniTask BuildAsync(int windowSize, CancellationToken cancellationToken)
        {
            if (windowSize <= 0 || windowSize % 2 == 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(windowSize), windowSize, "Нужен положительный нечётный размер окна.");
            }

            if (_assetProvider == null || _paletteLoader == null)
            {
                throw new InvalidOperationException(
                    $"{nameof(IAssetProvider)} и {nameof(CubePaletteLoader)} не пришли. {nameof(GridView)} нет в контейнере.");
            }

            if (_cubes != null)
            {
                throw new InvalidOperationException("Сетка уже собрана, второй раз BuildAsync вызывать нельзя.");
            }

            var (prefab, palette) = await UniTask.WhenAll(
                _assetProvider.LoadAsync<GameObject>(CubePrefabAddress, cancellationToken),
                _paletteLoader.LoadAsync(CubePaletteAddress, cancellationToken));

            cancellationToken.ThrowIfCancellationRequested();

            if (prefab.GetComponent<CubeView>() == null)
            {
                throw new InvalidOperationException(
                    $"На префабе «{CubePrefabAddress}» нет {nameof(CubeView)}.");
            }

            var parent = transform;
            var cubes = new CubeView[windowSize * windowSize];
            var radius = windowSize / 2;

            for (var y = 0; y < windowSize; y++)
            {
                for (var x = 0; x < windowSize; x++)
                {
                    var instance = Instantiate(prefab, parent);

                    instance.name = $"Cube_{x}_{y}";
                    instance.transform.localPosition = new Vector3(
                        (x - radius) * CellSpacing,
                        -(y - radius) * CellSpacing,
                        0f);

                    cubes[y * windowSize + x] = instance.GetComponent<CubeView>();
                }
            }

            _palette = palette;
            _cubes = cubes;
            _windowSize = windowSize;
        }

        public void Render(GridWindow window)
        {
            if (_cubes == null)
            {
                throw new InvalidOperationException($"Сначала вызовите {nameof(BuildAsync)}, сетка ещё не собрана.");
            }

            if (window.Size != _windowSize)
            {
                throw new ArgumentException(
                    $"Окно {window.Size}×{window.Size}, а кубы собраны под {_windowSize}×{_windowSize}.",
                    nameof(window));
            }

            for (var y = 0; y < _windowSize; y++)
            {
                for (var x = 0; x < _windowSize; x++)
                {
                    var symbol = window[x, y];

                    if (!_palette.TryGetMaterial(symbol, out var material))
                    {
                        throw new InvalidOperationException(
                            $"Для символа {(byte)symbol} в палитре нет материала.");
                    }

                    _cubes[y * _windowSize + x].Apply(material);
                }
            }
        }
    }
}
