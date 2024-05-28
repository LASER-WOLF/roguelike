namespace Core;

/// <summary>
/// A room.
/// </summary>
public class Room
{
    // Fields
    private static int minRoomSize = 5;
    public readonly int x;
    public readonly int y;
    public readonly int width;
    public readonly int height;
    
    // Parent node
    public readonly BspNode node;

    // Room data
    public bool?[,] area { get; private set; }
    public List<Vec2> lights { get; private set; } = new List<Vec2>();

    // Constructor
    public Room(BspNode node)
    {
        Logger.Log("Making room in node (" + node.id + ")");

        // Set padding
        int paddingVertical = Rand.random.Next(2, Math.Clamp(node.height - minRoomSize, 2, 10));
        int paddingHorizontal = Rand.random.Next(2, Math.Clamp(node.width - minRoomSize, 2, 10)); 
        int paddingTop = Rand.random.Next((int)(paddingVertical * 0.2), (int)(paddingVertical * 0.8));
        int paddingLeft = Rand.random.Next((int)(paddingHorizontal * 0.2), (int)(paddingHorizontal * 0.8));

        int width = node.width - paddingHorizontal;
        int height = node.height - paddingVertical;

        // Restrict room width/height ratio
        if (height > width) { height = Math.Min((int)(width * 8), width); }
        else { width = Math.Min((int)(height * 8), height); }

        // Set position and size
        this.node = node;
        this.x = node.x + paddingLeft;
        this.y = node.y + paddingTop;
        this.width = width;
        this.height = height;
        this.area = new bool?[width, height];

        // Generate room
        Generate();
    }
    
    // Generate room
    private void Generate() {
        for (int y = 0; y < height; y++) 
        {
            for (int x = 0; x < width; x++) 
            {
                if (x == 0 || y == 0 || x == width - 1 || y == height - 1) { area[x,y] = false; }
                else { area[x,y] = true; }
            }
        }
        
        // Add light at random position
        Vec2 lightPos = new Vec2(Rand.random.Next(1, width - 2), Rand.random.Next(1, height - 2));
        if (area[lightPos.x, lightPos.y] == true) { lights.Add(lightPos); }
    }
}
