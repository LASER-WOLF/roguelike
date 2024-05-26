using Raylib_cs;

namespace Main;

/// <summary>
/// World map.
/// </summary>
public class Map
{
    // Fields
    public readonly int width;
    public readonly int height;

    // Player
    public Player player { get; private set; }

    // Map data
    public Tree tree { get; private set; }
    private Tile[,] map;
    public bool[,] mapVisible { set; get; }
    
    // Lighting system
    private List<int> lights = new List<int>();
    private Dictionary<int, int> lightMap = new Dictionary<int, int>();
    private Dictionary<int, int> blocksLight = new Dictionary<int, int>();

    // Pathfinding
    public readonly PathGraph pathGraph;

    // Constructor
    public Map(int width, int height)
    {
        this.width = width;
        this.height = height;
        this.map = new Tile[width, height];
        this.mapVisible = new bool[width, height];
        this.pathGraph = new PathGraph(this);
        this.tree = new Tree(this, width, height);
        BuildMap();

        // Spawn the player (TODO: Move player from Map.cs -> Game.cs)
        this.player = new Player(this);
       
        //TEST: Pathfinding test
        //if (pathGraph.AstarPath(MapCoord(5, 5), MapCoord(25, 35), debugMode: true) != null) { Logger.Err("PATH FOUND!"); }
    }

    public bool InBounds(Vec2 location)
    {
        if (location.x >= 0 && location.x < width && location.y >= 0 && location.y < height) { return true; }
        return false;
    }

    public bool GetBlocking(Vec2 location)
    {
        return InBounds(location) ? !pathGraph.HasLocation(MapCoord(location.x, location.y)) : true;
    }

    public void SetVisible(Vec2 location, bool visible = true)
    {
        if (InBounds(location)) { mapVisible[location.x, location.y] = visible; }
    }

    // Convert x and y coord to single number
    public int MapCoord(int x, int y)
    {
        return (width * y) + x;
    }

    public Vec2 MapCoordReverse(int coord)
    {
        return new Vec2(coord % width, coord / width);
    }
    
    public int MapCoordReverseX(int coord)
    {
        return coord % width;
    }
    
    public int MapCoordReverseY(int coord)
    {
        return coord / width;
    }

    // Build the map
    private void BuildMap() {
        
        // Fill map with tile
        FillMap(Assets.tiles["void"]);

        // Build all rooms
        tree.VisitAllNodes(BuildRoom);

        // Build all corridors
        tree.VisitAllNodes(BuildCorridor);

        // Make lightmap
        lightMap = pathGraph.DijkstraMap(lights, blocksLight, 24);
    }

    public void AddDoor(int x, int y)
    {
        int coord = MapCoord(x, y);
        if (map[x, y] == Assets.tiles["empty"])
        { 
            map[x, y] = Assets.tiles["door"];
            blocksLight[coord] = 12;
        }
    }
    
    public void AddLight(int x, int y)
    {
        int coord = MapCoord(x, y);
        if (!lights.Contains(coord) && map[x, y] == Assets.tiles["empty"])
        { 
            lights.Add(coord); 
        }
    }

    private int GetLightIntensity(int x, int y)
    {
        if (lightMap != null)
        {
            int coord = MapCoord(x, y);
            if (lightMap.ContainsKey(coord))
            {
                return lightMap[coord];
            }
        }
        return 0;
    }

    // Fill the map with a tile
    private void FillMap(Tile tile)
    {
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                map[x, y] = tile;
            }
        }
    }
    
    // Build room from a node
    private void BuildRoom(Node node)
    {
        if (node.HasRoom())
        {
            Room room = node.room;
            BuildSpace(room.x, room.y, room.width, room.height, room.area);
            
            // Add lights
            foreach (Vec2 light in room.lights)
            {
                AddLight(room.x + light.x, room.y + light.y);
            }
        }
    }

    // Build corridor from a node
    private void BuildCorridor(Node node)
    {
        // Build corridor
        if (node.HasCorridor())
        {
            Corridor corridor = node.corridor;
            BuildSpace(corridor.x, corridor.y, corridor.width, corridor.height, corridor.area);
            
            // Add doors
            foreach (Vec2 door in corridor.doors)
            {
                AddDoor(corridor.x + door.x, corridor.y + door.y);
            }
        }
    }

    // Transfer location to world space and carve out area
    private void BuildSpace(int worldX, int worldY, int width, int height, bool?[,] area)
    {
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                bool? isSpace = area[x, y];
                if (isSpace == true) { map[worldX + x, worldY + y] = Assets.tiles["empty"]; }
                else if (isSpace == false) { map[worldX + x, worldY + y] = Assets.tiles["wall"]; }
            }
        }
    }

    // Render minimap
    public void Render() {
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (map[x, y] != null)
                {
                    Tile tile = map[x, y];
                    if (tile != Assets.tiles["void"] && tile != Assets.tiles["wall"])
                    {
                        Raylib.DrawRectangleLines(x * 6, y * 6, 6, 6, Color.Green);
                    }
                }
                    
                // Set empty tile char
                //char? tileChar = ' ';

                // if (mapVisible[x, y])
                // {
                //     // Visualize light intensity
                //     int lightIntensity = GetLightIntensity(x, y);
                //     if (lightIntensity > 16) { Console.BackgroundColor = ConsoleColor.Gray; }
                //     else if (lightIntensity > 0) { Console.BackgroundColor = ConsoleColor.DarkGray; }
                //     else { Console.BackgroundColor = ConsoleColor.Black; }
                    
                //     // Set tile char to char from tile map
                //     if (map[x, y] != null)
                //     {
                //         Tile tile = map[x, y];
                //         Console.ForegroundColor = tile.color;
                //         tileChar = tile.symbol;
                //     }

                //     // Render player
                //     if (player != null && player.x == x && player.y == y)
                //     {
                //         Console.BackgroundColor = ConsoleColor.Black;
                //         Console.ForegroundColor = ConsoleColor.Red;
                //         tileChar = player.symbol;
                //     }
               
                // }

            }
        }
    }
}
