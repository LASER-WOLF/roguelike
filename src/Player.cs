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
        FieldOfView();
    }

    private void FieldOfView()
    {
        Shadowcast.FieldOfView(map, new Vec2(x, y));
    }

    public bool MoveUp()
    {
        if (map.pathGraph.HasLocation(map.MapCoord(x, y-1))) { y--; FieldOfView(); return true; }
        return false;
    }

    public bool MoveDown()
    {
        if (map.pathGraph.HasLocation(map.MapCoord(x, y+1))) { y++; FieldOfView(); return true; }
        return false;
    }
    
    public bool MoveLeft()
    {
        if (map.pathGraph.HasLocation(map.MapCoord(x-1, y))) { x--; FieldOfView(); return true; }
        return false;
    }
    
    public bool MoveRight()
    {
        if (map.pathGraph.HasLocation(map.MapCoord(x+1, y))) { x++; FieldOfView(); return true; }
        return false;
    }
}
