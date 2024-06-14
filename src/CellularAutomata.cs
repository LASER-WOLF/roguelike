namespace Core;

/// <summary>
/// Cellular automata (CA) algorithm.
/// </summary>
public static class CellularAutomata
{
    
    // Run the algorithm and return a 2D map of booleans
    public static bool[,] Run(int width, int height, int iterations = 4, int percentOpen = 60)
    {
        bool[,] result = new bool[width, height];
        
        // Setup random map
        int randomColumn = Random.Shared.Next(4, width - 4);
        for(int y = 0; y < height; y++)
        {
            for(int x = 0; x < width; x++)
            {
                // Random change to set open space (exclude map edges)
                if(
                    !(x == 0 || y == 0 || x == width - 1 || y == height - 1)
                    && (x == randomColumn || Random.Shared.Next(100) < percentOpen)
                )
                    result[x, y] = true;
            }
        }
        
        // Run iterations
        for(int i = 0; i < iterations; i++)
        {
            bool[,] iterationResult = new bool[width, height];
            for(int y = 0; y < height; y++)
            {
                for(int x = 0; x < width; x++)
                {
                    // Cellular automata logic (exclude map edges)
                    if(!(x == 0 || y == 0 || x == width - 1 || y == height - 1))
                    {
                        // Set rules
                        int adjacent = Adjacent(result, width, height, x, y);
                        //int nearby = Nearby(result, width, height, x, y);
                        iterationResult[x, y] = (adjacent > 0 && adjacent < 5); 
                    }
                }
            }
            result = iterationResult;
        }

        // Return the complete map
        return result;
    }

    // Count adjacent non-open spaces
    private static int Adjacent(bool[,] result, int width, int height, int x, int y)
    {
        int adjacent = 0;
        for (int newX = x - 1; newX <= x + 1; newX++)
        {
            for (int newY = y - 1; newY <= y + 1; newY++)
            {
                if (!result[newX, newY]) { adjacent++; }
            }
        }
        return adjacent;
    }

    // Count nearby non-open spaces
    private static int Nearby(bool[,] result, int width, int height, int x, int y, int distance = 2)
    {
        int nearby = 0;
        for (int newX = x - distance; newX <= x + distance; newX++)
        {
            for (int newY = y - 2; newY <= y + 2; newY++)
            {
                if (Math.Abs(newX - x) == distance && Math.Abs(newY - y) == distance) { continue; }                                
                if (newX < 0 || newY < 0 || newX >= width || newY >= height) { continue; }
                if (!result[newX, newY]) { nearby++; }
            }
        }
        return nearby;
    }

}
