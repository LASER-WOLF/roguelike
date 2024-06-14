
using System.Numerics;

namespace Core;

/// <summary>
/// Drunkard's walk algorithm.
/// Returns a 2D array of booleans.
/// </summary>
public static class DrunkardsWalk
{
    public static bool[,] Run(int width, int height, int iterations = 20, int steps = 100)
    {
        bool[,] result = new bool[width, height];
        
        for (int i = 0; i < iterations; i++)
        {
            Vec2 pos = new Vec2(Rand.random.Next(width), Rand.random.Next(height));
            for (int s = 0; s < steps; s++)
            {
                pos += new Vec2(Rand.random.Next(-1,2), Rand.random.Next(-1,2));
                if (pos.x < 0 || pos.x >= width || pos.y < 0 || pos.y >= height) { break; }
                result[pos.x, pos.y] = true;
            }
        }
        return result;
    }
}
