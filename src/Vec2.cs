namespace Core;

/// <summary>
/// Integer Vector 2.
/// </summary>
public class Vec2 
{
    public int x { get; set; }
    public int y { get; set; }

    public Vec2(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public static Vec2 operator +(Vec2 a, Vec2 b)
    {
        return new Vec2(a.x + b.x, a.y + b.y);
    }
}


