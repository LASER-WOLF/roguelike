using Raylib_cs;
using System.Numerics;

namespace Core;

/// <summary>
/// Draw on the screen.
/// </summary>
public static class Draw
{
    public static void Circle3d(float radius, Matrix4x4 matrix, Color color)
    {
        int points = 20 + (int)(radius * 4f);
        Vector3[] circlePoints = new Vector3[points];
        for (int i = 0; i < points; i++)
        {
            float circlePos = 1f / (float)points * (float)i * MathF.PI * 2f;
            circlePoints[i].X = MathF.Sin(circlePos) * radius;
            circlePoints[i].Z = MathF.Cos(circlePos) * radius;
        }
        for (int i = 0; i < points; i++)
        {
            Raylib.DrawLine3D(Raymath.Vector3Transform(circlePoints[i], matrix), Raymath.Vector3Transform(circlePoints[(i + 1) % points], matrix), color);
        }
    }

    public static void Grid3d(int size, Matrix4x4 matrix, Color color)
    {
        for (int i = -size; i <= size; i++)
        {
            Raylib.DrawLine3D(Raymath.Vector3Transform(new Vector3(-(float)size, 0f, (float)i), matrix), Raymath.Vector3Transform(new Vector3((float)size, 0f, (float)i), matrix), color);
            Raylib.DrawLine3D(Raymath.Vector3Transform(new Vector3((float)i, 0f, -(float)size), matrix), Raymath.Vector3Transform(new Vector3((float)i, 0f, (float)size), matrix), color);
        }
    }

    public static void Axes3d(float size, Matrix4x4 matrix)
    {
            Raylib.DrawLine3D(Raymath.Vector3Transform(new Vector3(-size, 0f, 0f), matrix), Raymath.Vector3Transform(new Vector3(size, 0f, 0f), matrix), Color.Red);
            Raylib.DrawLine3D(Raymath.Vector3Transform(new Vector3(0f, -size, 0f), matrix), Raymath.Vector3Transform(new Vector3(0f, size, 0f), matrix), Color.Blue);
            Raylib.DrawLine3D(Raymath.Vector3Transform(new Vector3(0f, 0f, -size), matrix), Raymath.Vector3Transform(new Vector3(0f, 0f, size), matrix), Color.Green);
    }

    public static void Box3d(float size, Matrix4x4 matrix, Color color)
    {
        Vector3[] points = new Vector3[8];
        points[0] = Raymath.Vector3Transform(new Vector3(-size,  size, -size), matrix);
        points[1] = Raymath.Vector3Transform(new Vector3( size,  size, -size), matrix);
        points[2] = Raymath.Vector3Transform(new Vector3(-size, -size, -size), matrix);
        points[3] = Raymath.Vector3Transform(new Vector3( size, -size, -size), matrix);
        points[4] = Raymath.Vector3Transform(new Vector3(-size,  size,  size), matrix);
        points[5] = Raymath.Vector3Transform(new Vector3( size,  size,  size), matrix);
        points[6] = Raymath.Vector3Transform(new Vector3(-size, -size,  size), matrix);
        points[7] = Raymath.Vector3Transform(new Vector3( size, -size,  size), matrix);
        Raylib.DrawLine3D(points[0], points[1], color);
        Raylib.DrawLine3D(points[2], points[3], color);
        Raylib.DrawLine3D(points[0], points[2], color);
        Raylib.DrawLine3D(points[1], points[3], color);
        Raylib.DrawLine3D(points[4], points[5], color);
        Raylib.DrawLine3D(points[6], points[7], color);
        Raylib.DrawLine3D(points[4], points[6], color);
        Raylib.DrawLine3D(points[5], points[7], color);
        Raylib.DrawLine3D(points[0], points[4], color);
        Raylib.DrawLine3D(points[1], points[5], color);
        Raylib.DrawLine3D(points[2], points[6], color);
        Raylib.DrawLine3D(points[3], points[7], color);
    }
}
