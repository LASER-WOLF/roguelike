using System.Numerics;
namespace Core;

/// <summary>
/// Integer Vector 2.
/// </summary>
public class Vec2 
{
    public int x;
    public int y;

    public Vec2(int value) : this(value, value)
    {
    }

    public Vec2(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public static Vec2 Zero
        {
            get => default;
    }

    public static Vec2 One
    {
        get => new Vec2(1);
    }

    public static Vec2 UnitX
    {
        get => new Vec2(1, 0);
    }

    public static Vec2 UnitY
    {
        get => new Vec2(0, 1);
    }

    public static Vec2 operator +(Vec2 a, Vec2 b)
    {
        return new Vec2(a.x + b.x, a.y + b.y);
    }

    public static Vec2 operator /(Vec2 a, Vec2 b)
    {
        return new Vec2(a.x / b.x, a.y / b.y);
    }

    public static Vec2 operator /(Vec2 a, int b)
    {
        return a / new Vec2(b);
    }

    public static Vec2 operator *(Vec2 a, Vec2 b)
    {
        return new Vec2(a.x * b.x, a.y * b.y);
    }

    public static Vec2 operator *(Vec2 a, int b)
    {
        return a * new Vec2(b);
    }

    public static Vec2 operator *(int a, Vec2 b)
    {
        return b * a;
    }

    public static Vec2 operator -(Vec2 a, Vec2 b)
    {
        return new Vec2(a.x - b.x, a.y - b.y);
    }

    public static Vec2 operator -(Vec2 value)
    {
        return Zero - value;
    }

    public static int Dot(Vec2 value1, Vec2 value2)
    {
        return (value1.x * value2.x) + (value1.y * value2.y);
    }

    public static int DistanceSquared(Vec2 value1, Vec2 value2)
    {
        Vec2 difference = value1 - value2;
        return Dot(difference, difference);
    }

    public static float Distance(Vec2 value1, Vec2 value2)
    {
        return (float)Math.Sqrt((float)DistanceSquared(value1, value2));
    }

    public static Vec2 FromVector2(Vector2 a)
    {
        return new Vec2((int)a.X, (int)a.Y);
    }
}


