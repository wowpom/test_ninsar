using System.Collections.Generic;
using Game.Domain;
using UnityEngine;

namespace Game.Presentation
{
    public sealed class CubePalette
    {
        private readonly Dictionary<Symbol, Material> _materials;

        public CubePalette(Dictionary<Symbol, Material> materials)
        {
            _materials = materials;
        }

        public bool TryGetMaterial(Symbol symbol, out Material material)
        {
            return _materials.TryGetValue(symbol, out material);
        }
    }
}
