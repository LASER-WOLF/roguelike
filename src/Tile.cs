using Raylib_cs;

namespace Core;

/// <summary>
/// A map tile.
/// </summary>
public class Tile
{
    public bool blocksVision { get; private set; }
    public Color color { get; private set; }

    public Tile(bool blocksVision, Color color)
    {
        this.blocksVision = blocksVision;
        this.color = color;
    }
}
