namespace Core;

/// <summary>
/// Cellular automata (CA) algorithm.
/// Returns a 2D array of booleans.
/// </summary>
public static class CellularAutomata
{
    public static bool[] Run(int width, int height, int iterations = 4, int percentWalls = 40)
    {
        bool[] result = new bool[width * height];
        
        // Setup random map
        int randomColumn = Random.Shared.Next(4, width - 4);
        for(int y = 0; y < height; y++)
        {
            for(int x = 0; x < width; x++)
            {
                // Place wall at the edge of the map
                if(x == 0 || y == 0 || x == width - 1 || y == height - 1)
                    result[x + y * width] = true;

                // Random chance to place a wall
                else if(x != randomColumn && Random.Shared.Next(100) < percentWalls)
                    result[x + y * width] = true;
            }
        }
        
        // Run iterations
        for(int i = 0; i < iterations; i++)
        {
            bool[] iterationResult = new bool[width * height];
            for(int y = 0; y < height; y++)
            {
                for(int x = 0; x < width; x++)
                {
                    // Edge of map
                    if(x == 0 || y == 0 || x == width - 1 || y == height - 1) { iterationResult[x + y * width] = true; }
                    // Cellular automata logic
                    else 
                    { 
                        int adjacent = AdjacentWalls(result, width, height, x, y);
                        //int nearby = NearbyWalls(result, width, height, x, y);
                        //iterationResult[x + y * width] = (adjacent >= 5 || nearby <= 2); 
                        iterationResult[x + y * width] = (adjacent == 0 || adjacent > 4); 
                    }
                }
            }
            result = iterationResult;
        }

        // Return the complete map
        return result;
    }

    // Count adjacent walls
    private static int AdjacentWalls(bool[] result, int width, int height, int x, int y)
    {
        int adjacent = 0;
        for (int newX = x - 1; newX <= x + 1; newX++)
        {
            for (int newY = y - 1; newY <= y + 1; newY++)
            {
                if (result[newX + newY * width]) { adjacent++; }
            }
        }
        return adjacent;
    }

    // Count nearby walls
    private static int NearbyWalls(bool[] result, int width, int height, int x, int y)
    {
        int nearby = 0;
        for (int newX = x - 2; newX <= x + 2; newX++)
        {
            for (int newY = y - 2; newY <= y + 2; newY++)
            {
                if (Math.Abs(newX - x) == 2 && Math.Abs(newY - y) == 2) { continue; }                                
                if (newX < 0 || newY < 0 || newX >= width || newY >= height) { continue; }
                if (result[newX + newY * width]) { nearby++; }
            }
        }
        return nearby;
    }
}
