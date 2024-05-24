namespace Main;

/// <summary>
/// Player controlled character.
/// </summary>
public class Player
{
    private Map map;
    public readonly char symbol = '@';
    public int x { get; private set; }
    public int y { get; private set; }
    private int visionRange = 8;

    public Player(Map map)
    {
        this.map = map;
        Room room = map.tree.FindLeftLeaf(map.tree.root).room;
        this.x = room.x + 1;
        this.y = room.y + 1;
        Fov();
    }

    private void Fov()
    {
        for (int y = 0; y < map.height; y++)
        {
            for (int x = 0; x < map.width; x++)
            {
                map.mapVisible[x, y] = false;
            }
        }

        Shadowcast.Run(map, new Vec2(x, y));
        //ShadowcastAlt.Run(map, new Vec2(x, y));
    }

    public bool MoveUp()
    {
        if (map.pathGraph.HasLocation(map.MapCoord(x, y-1))) { y--; Fov(); return true; }
        return false;
    }

    public bool MoveDown()
    {
        if (map.pathGraph.HasLocation(map.MapCoord(x, y+1))) { y++; Fov(); return true; }
        return false;
    }
    
    public bool MoveLeft()
    {
        if (map.pathGraph.HasLocation(map.MapCoord(x-1, y))) { x--; Fov(); return true; }
        return false;
    }
    
    public bool MoveRight()
    {
        if (map.pathGraph.HasLocation(map.MapCoord(x+1, y))) { x++; Fov(); return true; }
        return false;
    }
}
