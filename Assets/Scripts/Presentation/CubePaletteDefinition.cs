using System;

namespace Game.Presentation
{
    [Serializable]
    public sealed class CubePaletteDefinition
    {
        public CubePaletteEntryDefinition[] entries;
    }

    [Serializable]
    public sealed class CubePaletteEntryDefinition
    {
        public int symbol;
        public string materialAddress;
    }
}
