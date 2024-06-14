namespace Core;

/// <summary>
/// Diffusion-limited aggregation (DLA) algorithm.
/// Returns a 2D array of booleans.
/// </summary>
public static class DiffusionLimitedAggregation
{

    // Diffusion-Limited Aggregation (DLA) algorithm
    public static bool[,] Run(int width, int height, int steps = 10)
    {
        bool[,] result = new bool[width, height];
        
        // Set center area to true
        result[(int)(width / 2) + 0, (int)(height / 2) + 0] = true;
        result[(int)(width / 2) + 0, (int)(height / 2) + 1] = true;
        result[(int)(width / 2) + 1, (int)(height / 2) + 0] = true;
        result[(int)(width / 2) + 1, (int)(height / 2) + 1] = true;

        for (int i = 0; i < steps; i++)
        {
            Vec2 origin = new Vec2(Rand.random.Next(width), Rand.random.Next(height));
            while(true)
            {
                Vec2 dir = new Vec2(Rand.random.Next(-1,2), Rand.random.Next(-1,2));
                Vec2 dest = origin + dir;
                if (dest.x < 0 || dest.x >= width || dest.y < 0 || dest.y >= height) { break; }
                else if (result[dest.x, dest.y]){ result[origin.x, origin.y] = true; break; }
                origin = dest;
            }
        }
        return result;
    }
}
