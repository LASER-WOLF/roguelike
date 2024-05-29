using System.Numerics;

public static class ClassExtensions
{
    public static Vector3 XYY(this Vector3 vec)
    {
        return new Vector3(vec.X, vec.Y, vec.Y);
    }
    public static Vector3 ZZY(this Vector3 vec)
    {
        return new Vector3(vec.Z, vec.Z, vec.Y);
    }
    public static Vector3 YYX(this Vector3 vec)
    {
        return new Vector3(vec.Y, vec.Y, vec.X);
    }
    public static Vector3 YXX(this Vector3 vec)
    {
        return new Vector3(vec.Y, vec.X, vec.X);
    }
}
