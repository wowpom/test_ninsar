namespace Game.Domain
{
    public interface ISymbolGrid
    {
        int Width { get; }

        int Height { get; }

        Symbol this[GridCoord coord] { get; }

        GridCoord Wrap(GridCoord coord);
    }
}
