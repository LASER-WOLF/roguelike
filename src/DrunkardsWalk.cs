namespace Core;

/// <summary>
/// Drunkard's walk algorithm.
/// </summary>
public static class DrunkardsWalk
{

    // Run the algorithm and return a 2D map of booleans
    public static bool[,] Run(int width, int height, int splits = 2, int percentOpen = 10 )
    {
        bool[,] result = new bool[width, height];

        // Place a drunkard on the map and walk around, setting walked locations to true
        for (int i = 0; i < splits; i++)
        {
            
            // Check how many cells should be set to true for the walk to be considered finished
            int remaining = (int)((((float)width * (float)height) * ((float)percentOpen * 0.01f)) / (float)splits);
            
            // Select a random starting point
            Vec2 pos = new Vec2(Random.Shared.Next(width), Random.Shared.Next(height));
            
            // Walk until the desired number of cells has been set to true
            while (remaining > 0)
            {
                // Select a random direction to go next
                Vec2 dest = pos + new Vec2(Random.Shared.Next(-1,2), Random.Shared.Next(-1,2));
                
                // Abort and select a new direction if chosen direction is out of bounds
                if (dest.x < 0 || dest.x >= width || dest.y < 0 || dest.y >= height) { continue; }

                // Check if open space is found
                if (!result[dest.x, dest.y]) 
                { 
                    result[dest.x, dest.y] = true;
                    remaining--;
                }
                
                // Change to new position
                pos = dest;
            }
        }
        return result;
    }

}
