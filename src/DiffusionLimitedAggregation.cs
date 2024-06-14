namespace Core;

/// <summary>
/// Diffusion-limited aggregation (DLA) algorithm.
/// </summary>
public static class DiffusionLimitedAggregation
{
    
    // Run the algorithm and return a 2D map of booleans
    public static bool[,] Run(int width, int height, int percentOpen = 20, bool outwards = false)
    {
        bool[,] result = new bool[width, height];

        // Check how many cells should be set to true for the generation to be considered finished
        int remaining = (int)(((float)width * (float)height) * ((float)percentOpen * 0.01f));
        
        // Set center area to true
        result[(int)(width / 2) + 0, (int)(height / 2) + 0] = true;
        result[(int)(width / 2) + 0, (int)(height / 2) + 1] = true;
        result[(int)(width / 2) + 1, (int)(height / 2) + 0] = true;
        result[(int)(width / 2) + 1, (int)(height / 2) + 1] = true;

        // Start the loop
        while (remaining > 0)
        {
            // Select a random starting point
            Vec2 pos = new Vec2(outwards ? (int)(width / 2) : Random.Shared.Next(width), outwards ? (int)(height / 2) : Random.Shared.Next(height));
            
            // Walk in a random direction until open space is found
            while(true)
            {
                // Select a random direction to go next
                Vec2 dest = pos + new Vec2(Random.Shared.Next(-1, 2), Random.Shared.Next(-1, 2));

                // Abort and select a new direction if chosen direction is out of bounds
                if (dest.x < 0 || dest.x >= width || dest.y < 0 || dest.y >= height) { break; }

                // Check if open space is found
                else if (outwards ? !result[dest.x, dest.y] : result[dest.x, dest.y])
                { 
                    if (outwards) { result[dest.x, dest.y] = true; }
                    else { result[pos.x, pos.y] = true; }
                    remaining--; 
                    break; 
                }

                // Change to new position
                pos = dest;
            }
        }
        return result;
    }

}
