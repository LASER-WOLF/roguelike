using Raylib_cs;

namespace Core;

public class Tile
{
    // TODO: Add offset and mesh
    public bool blocksVision { get; private set; }
    public Color color { get; private set; }

    public Tile(bool blocksVision, Color color)
    {
        this.blocksVision = blocksVision;
        this.color = color;
    }
}
