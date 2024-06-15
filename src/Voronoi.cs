using System.Numerics;

namespace Core;

/// <summary>
/// Voronoi diagram.
/// </summary>
public static class Voronoi
{
    
    // Run the algorithm and return a 2D map of booleans
    public static int[,] Run(int width, int height, int numSeeds = 10, bool manhattan = false)
    {
        int[,] result = new int[width, height];
       
        // Create an array of unique Vec2 seeds
        Vec2[] seeds = new Vec2[numSeeds];
        for (int i = 0; i < numSeeds; i++)
        {
            while (seeds[i] == null)
            {
                // Select a random point
                Vec2 point = new Vec2(Random.Shared.Next(width - 1), Random.Shared.Next(height - 1));
                
                // Only add the point to the seeds if it doesn't exist already
                bool pointExists = false;
                foreach (Vec2 seed in seeds.Where(item => item != null))
                {
                    if (seed.x == point.x && seed.y == point.y) { pointExists = true; break; }
                }
                if (!pointExists) { seeds[i] = point; }
            }
        }

        // Check all cells in the map and find nearest seed
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int nearest = 0;
                float nearestDistance = 0f;
                for (int i = 0; i < numSeeds; i++)
                {
                    float distance = manhattan ? (float)Math.Abs(seeds[i].x - x) + (float)Math.Abs(seeds[i].y - y) : Vec2.Distance(new Vec2(x, y), seeds[i]);
                    if (i == 0 || distance < nearestDistance)
                    {
                        nearest = i;
                        nearestDistance = distance;
                    }
                }
                result[x, y] = nearest;
            }
        }

        return result;
    }
}
