using System.Numerics;

public static class ClassExtensions
{
    public static Vector3 XXX(this Vector3 vec)
    {
        return new Vector3(vec.X, vec.X, vec.X);
    }
    public static Vector3 XXY(this Vector3 vec)
    {
        return new Vector3(vec.X, vec.X, vec.Y);
    }
    public static Vector3 XXZ(this Vector3 vec)
    {
        return new Vector3(vec.X, vec.X, vec.Z);
    }
    public static Vector3 XYX(this Vector3 vec)
    {
        return new Vector3(vec.X, vec.Y, vec.X);
    }
    public static Vector3 XYY(this Vector3 vec)
    {
        return new Vector3(vec.X, vec.Y, vec.Y);
    }
    public static Vector3 XZX(this Vector3 vec)
    {
        return new Vector3(vec.X, vec.Z, vec.X);
    }
    public static Vector3 XZY(this Vector3 vec)
    {
        return new Vector3(vec.X, vec.Z, vec.Y);
    }

    public static Vector3 XZZ(this Vector3 vec)
    {
        return new Vector3(vec.X, vec.Z, vec.Z);
    }

    public static Vector3 YXX(this Vector3 vec)
    {
        return new Vector3(vec.Y, vec.X, vec.X);
    }
    public static Vector3 YXY(this Vector3 vec)
    {
        return new Vector3(vec.Y, vec.X, vec.Y);
    }

    public static Vector3 YXZ(this Vector3 vec)
    {
        return new Vector3(vec.Y, vec.X, vec.Z);
    }

    public static Vector3 YYX(this Vector3 vec)
    {
        return new Vector3(vec.Y, vec.Y, vec.X);
    }
    public static Vector3 YYY(this Vector3 vec)
    {
        return new Vector3(vec.Y, vec.Y, vec.Y);
    }
    public static Vector3 YYZ(this Vector3 vec)
    {
        return new Vector3(vec.Y, vec.Y, vec.Z);
    }
    public static Vector3 YZX(this Vector3 vec)
    {
        return new Vector3(vec.Y, vec.Z, vec.X);
    }
    public static Vector3 YZY(this Vector3 vec)
    {
        return new Vector3(vec.Y, vec.Z, vec.Y);
    }
    public static Vector3 YZZ(this Vector3 vec)
    {
        return new Vector3(vec.Y, vec.Z, vec.Z);
    }
    public static Vector3 ZXX(this Vector3 vec)
    {
        return new Vector3(vec.Z, vec.X, vec.X);
    }
    public static Vector3 ZXY(this Vector3 vec)
    {
        return new Vector3(vec.Z, vec.X, vec.Y);
    }
    public static Vector3 ZXZ(this Vector3 vec)
    {
        return new Vector3(vec.Z, vec.X, vec.Z);
    }
    public static Vector3 ZYX(this Vector3 vec)
    {
        return new Vector3(vec.Z, vec.Y, vec.X);
    }
    public static Vector3 ZYY(this Vector3 vec)
    {
        return new Vector3(vec.Z, vec.Y, vec.Y);
    }
    public static Vector3 ZYZ(this Vector3 vec)
    {
        return new Vector3(vec.Z, vec.Y, vec.Z);
    }
    public static Vector3 ZZX(this Vector3 vec)
    {
        return new Vector3(vec.Z, vec.Z, vec.X);
    }
    public static Vector3 ZZY(this Vector3 vec)
    {
        return new Vector3(vec.Z, vec.Z, vec.Y);
    }
    public static Vector3 ZZZ(this Vector3 vec)
    {
        return new Vector3(vec.Z, vec.Z, vec.Z);
    }
}
