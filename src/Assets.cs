using Raylib_cs;

namespace Core;

/// <summary>
/// Shared assets.
/// </summary>
static class Assets 
{
    public static Dictionary<string, Tile> tiles { set; get; } = new Dictionary<string, Tile>();

    static Assets()
    {
        tiles.Add("void", new Tile(false, Color.Red));
        tiles.Add("wall", new Tile(true, Color.Red));
        tiles.Add("empty", new Tile(false, Color.Green));
        tiles.Add("door", new Tile(false, Color.Blue));
    }
}
