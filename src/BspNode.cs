namespace Core;

/// <summary>
/// Binary space partitioning (BSP) node for map generation.
/// </summary>
public class BspNode
{
    // Fields
    private static int minSize = 12;
    public readonly int id; 
    public readonly int x;
    public readonly int y;
    public readonly int width;
    public readonly int height;

    // Properties
    public static int count { get; private set; } = 0;

    // Parent tree
    public readonly BspTree tree;

    // Parent node
    public BspNode parent { get; private set;}

    // Children
    public BspNode[] children = { null, null };
    
    // Node data
    public Room room { get; private set; } = null;
    public Corridor corridor { get; set; } = null;
    
    // Private

    // Constructor
    public BspNode(BspTree tree, int width, int height, int x = 0, int y = 0, BspNode parent = null)
    {
        this.id = count;
        count++;
        Logger.Log("Making new node (" + id.ToString() + ")");
        this.tree = tree;
        this.parent = parent;
        this.x = x;
        this.y = y;
        this.width = width;
        this.height = height;

        // Recursivly try to split the node into smaller nodes
        TrySplit();
    }

    // Return node depth
    public int GetLevel() 
    {
        int level = 0;
        BspNode parentCheck = parent;
        while (parentCheck != null)
        {
            parentCheck = parentCheck.parent;
            level++;
        }
        return level;
    }
    
    // Return true if this node has a parent
    public bool HasParent()
    {
        if (parent != null) { return true; }
        return false;
    }
    
    // Return true if this node is the second child
    public bool IsSecondChild()
    {
        if (HasParent()) { if (parent.children[1] == this) { return true; } }
        return false;
    }
   
    // Return the sibling node if this node has a sibling, else returns null
    public BspNode GetSibling()
    {
        if (HasParent())
        {
            if (IsSecondChild()) { return parent.children[0]; }
            else { return parent.children[1]; }
        }
        return null;
    }
    
    // Return true if this node has a corridor
    public bool HasCorridor()
    {
        if (corridor != null) { return true; }
        return false;
    }

    // Return true if this node has a room
    public bool HasRoom()
    {
        if (room != null) { return true; }
        return false;
    }

    // Return true if this node is a leaf
    public bool IsLeaf()
    {
        if (children[0] == null && children[1] == null) { return true; }
        return false;
    }

    // Try to split the node horizontally
    private bool TrySplitHorizontal() 
    {
        int splitX = x + Random.Shared.Next((int)(width * 0.25), (int)(width * 0.75));
        int widthChildLeft = splitX - x;
        int widthChildRight = (x + width) - splitX;
        if (widthChildLeft > minSize && widthChildRight > minSize)
        {
            Logger.Log("Splitting node (" + id.ToString() + ") horizontally");
            children[0] = new BspNode(tree, widthChildLeft, height, x, y, parent: this);
            children[1] = new BspNode(tree, widthChildRight, height, splitX, y, parent: this);
            return true;
        }
        return false;
    }

    // Try to split the node vertically
    private bool TrySplitVertical()
    {
        int splitY = y + Random.Shared.Next((int)(height * 0.25), (int)(height * 0.75));
        int heightChildLeft = splitY - y;
        int heightChildRight = (y + height) - splitY;
        if (heightChildLeft > minSize && heightChildRight > minSize)
        {
            Logger.Log("Splitting node (" + id.ToString() + ") vertically");
            children[0] = new BspNode(tree, width, heightChildLeft, x, y, parent: this);
            children[1] = new BspNode(tree, width, heightChildRight, x, splitY, parent: this);
            return true;
        }
        return false;
    }

    // Try to split the node
    private void TrySplit() 
    {
        // Set to true if node was split
        bool validSplit = false;

        // Randomly pick horizontal or vertical split
        bool dirHor = Random.Shared.Next(99) < 30;
        
        // Try to split the node horizontally, if not possible then try vertically
        if (dirHor)
        { 
            validSplit = TrySplitHorizontal(); 
            if (!validSplit) { validSplit = TrySplitVertical(); }
        }
        
        // Try to split the node vertically, if not possible then try horizontally
        else
        {
            validSplit = TrySplitVertical(); 
            if (!validSplit) { validSplit = TrySplitHorizontal(); }
        }

        // Was not able to split the node, turn the node into a room
        if (!validSplit) { MakeRoom(); }
    }

    // Make a room in this node
    private void MakeRoom()
    {
        room = new Room(this);
    }
}
