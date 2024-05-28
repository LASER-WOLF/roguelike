namespace Core;

/// <summary>
/// Binary space partitioning (BSP) tree for map generation.
/// </summary>
public class BspTree
{
    // Fields
    public readonly int id;
    public readonly int x;
    public readonly int y;
    public readonly int width;
    public readonly int height;

    // Properties
    public static int count { get; private set; } = 0;

    // Parent
    public readonly Map map;

    // Root node
    public readonly BspNode root;

    // Constructor
    public BspTree(Map map, int width, int height, int x = 0, int y = 0)
    {
        this.id = count;
        count++;
        Logger.Log("Making new BSP tree (" + id.ToString() + ")");
        this.x = x;
        this.y = y;
        this.width = width;
        this.height = height;
        this.map = map;
        
        // Generate all nodes and rooms
        this.root = new BspNode(this, width, height);
        
        // Generate pathfinding graph for all the rooms
        AddAllRoomsToPathGraph();

        // Generate corridors
        VisitAllNodes(GenerateCorridor);

        // Print info for all nodes
        VisitAllNodes(NodeInfo);
    }

    // Add all the rooms to the pathfinding graph
    public void AddAllRoomsToPathGraph()
    {
        BspNode[] nodes = NodeArray();
        foreach (BspNode node in nodes) { if (node.HasRoom()) { map.pathGraph.AddRoom(node.room); }}
    }
    
    // Print info for a given node
    private void NodeInfo(BspNode node)
    {
        Logger.Log("Node (" + node.id.ToString() + "), parent: " + (node.parent != null ? node.parent.id : "null") + ", sibling: " + (node.GetSibling() != null ? node.GetSibling().id : "null") + ", children[0]: " + (node.children[0] != null ? node.children[0].id : "null") + ", children[1]: " + (node.children[1] != null ? node.children[1].id : "null"));
    }
    
    // Return an array containing all nodes
    public BspNode[] NodeArray()
    {
        BspNode[] nodeArray = new BspNode[BspNode.count];
        NodeArrayAdd(root, ref nodeArray);
        return nodeArray;
    }
    
    // Traverse all child nodes and add to array
    private void NodeArrayAdd(BspNode node, ref BspNode[] nodeArray)
    {
        nodeArray[node.id] = node;
        if (node.children[0] != null) { NodeArrayAdd(node.children[0], ref nodeArray); }
        if (node.children[1] != null) { NodeArrayAdd(node.children[1], ref nodeArray); }
    }

    // Visit all nodes and call callback when visiting
    public void VisitAllNodes(Action<BspNode> callback)
    {
        VisitNodes(root, callback);
    }

    // Visit current node and all child nodes and call callback method when visiting
    private void VisitNodes(BspNode node, Action<BspNode> callback)
    {
        if (node.children[0] != null) { VisitNodes(node.children[0], callback); }
        if (node.children[1] != null) { VisitNodes(node.children[1], callback); }
        callback(node);
    }

    // Visit all leaves and call callback when visiting
    public void VisitAllLeaves(Action<BspNode> callback)
    {
        VisitLeaves(root, callback);
    }

    // Traverse current node and all child nodes and call callback method when reaching the tree leaves
    private void VisitLeaves(BspNode node, Action<BspNode> callback)
    {
        if (node.children[0] != null) { VisitLeaves(node.children[0], callback); }
        if (node.children[1] != null) { VisitLeaves(node.children[1], callback); }
        if (node.children[0] == null && node.children[1] == null) { callback(node); }
    }

    // Visit left child recursively until leaf found
    public BspNode FindLeftLeaf(BspNode node)
    {
        if (node.children[0] != null) { return FindLeftLeaf(node.children[0]); }
        else{ return node; }
    }
    
    // Visit right child recursively until leaf found
    public BspNode FindRightLeaf(BspNode node)
    {
        if (node.children[1] != null) { return FindRightLeaf(node.children[1]); }
        else{ return node; }
    }

    // Find next leaf to the right of current leaf, return null if at rightmost leaf
    public BspNode LeafToLeafRight(BspNode node)
    {
        while (true)
        {
            bool moveRight = !node.IsSecondChild();
            if (node.parent == null) { return null; }
            else { node = node.parent; }
            if (node.children[1] != null && moveRight)
            {  
                node = node.children[1];
                if (node.children[0] != null) { return FindLeftLeaf(node.children[0]); }
                return node;
            }
        }

    }

// Generate a corridor for the current node, between the rightmost leaf of the left child, and the leftmost leaf of the right child
private void GenerateCorridor(BspNode node)
{
    if (node.children[0] != null && node.children[1] != null)
    {
        // Set start and endpoints for the corridor
        Room roomFirst = FindRightLeaf(node.children[0]).room;
        Room roomSecond = FindLeftLeaf(node.children[1]).room;
        int x0 = roomFirst.x + Math.Min(roomFirst.width - 1, Rand.random.Next(2, roomFirst.width - 3));
        int y0 = roomFirst.y + Math.Min(roomFirst.height - 1, Rand.random.Next(2, roomFirst.height - 3));
        int x1 = roomSecond.x + Math.Min(roomSecond.width - 1, Rand.random.Next(2, roomSecond.width - 3));
        int y1 = roomSecond.y + Math.Min(roomSecond.height - 1, Rand.random.Next(2, roomSecond.height - 3));

        // Check if path already exists between startpoint and endpoint
        if (map.pathGraph.BfsCheck(map.MapCoord(x0, y0), map.MapCoord(x1, y1)))
        { Logger.Log("NOT making corridor in node (" + node.id.ToString() + "), path already exists!"); }

        // If path doesn't exist make new corridor
        else
        {
            node.corridor = new Corridor(node, x0, y0, x1, y1 );
            map.pathGraph.AddCorridor(node.corridor, modifyNeighbors: true);
        }
    }
}

// Check all nodes if a given point is inside a room
public bool? CheckCollisionAll(int x, int y, bool room = true, bool corridor = true)
{
    // Check if within map bounds
    if ((room || corridor) && (x > 0 || x < map.width || y > 0 || y < map.height))
    {
        // Perform collision check
        return CheckCollision(root, x, y, room, corridor);
    }

    // Return null if out of bounds
    return null;
}

// Check if given point is inside a room in a given node, if no collision found recursivly check all child nodes
public bool? CheckCollision(BspNode node, int x, int y, bool room = true, bool corridor = true)
    {
        bool? collisionFound = null;

        // Check for collision in room
        if (collisionFound == null && node.room != null && room == true)
        {
            Room r = node.room;
            if ((x >= r.x) && (x < r.x + r.width) && (y >= r.y) && (y < r.y + r.height)) 
            {
                collisionFound = r.area[x-r.x, y-r.y];
                if (collisionFound == true) { collisionFound = false; }
                else if (collisionFound == false) { collisionFound = true; }
            }
        }

        // Check for collision in corridor
        if (collisionFound == null && node.corridor != null && corridor == true)
        {
            Corridor c = node.corridor;
            if ((x >= c.x) && (x < c.x + c.width) && (y >= c.y) && (y < c.y + c.height)) 
            {
                collisionFound = c.area[x-c.x, y-c.y];
                if (collisionFound == true) { collisionFound = false; }
                else if (collisionFound == false) { collisionFound = true; }
            }
        }

        // Check for collision in children nodes recursively
        if (collisionFound == null && node.children[0] != null) { collisionFound = CheckCollision(node.children[0], x, y, room, corridor); }
        if (collisionFound == null && node.children[1] != null) { collisionFound = CheckCollision(node.children[1], x, y, room, corridor); }

        // Return collision
        return collisionFound;
    }
}
