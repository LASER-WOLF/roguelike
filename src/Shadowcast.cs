namespace Core;

/// <summary>
/// Symmetric Shadowcasting alogrithm.
/// Implementation based on:
/// Albert Ford, "Symmetric Shadowcasting"
/// https://www.albertford.com/shadowcasting/
/// </summary>
public static class Shadowcast
{

    // Walk through every quadrant and set visible locations
    public static void Run(Map map, Vec2 origin, int range = 99)
    {
        // Set current position to visible
        map.SetVisible(origin);

        // Scan all four quadrants (north, south, east, west)
        for (int quadrant = 0; quadrant < 4; quadrant++)
        {
            Scan(map, origin, quadrant, 1, -1, 1, range);
        }
    }
    
    // Recursively scan through rows and columns in a given quadrant
    private static void Scan(Map map, Vec2 origin, int quadrant, int row, float startSlope, float endSlope, int range)
    {
        // Set start and end column numbers based on slope
        bool rowVisible = false;
        int minCol = (int)Math.Floor(((float)row * (float)startSlope) + (float)0.5);
        int maxCol = (int)Math.Ceiling(((float)row * (float)endSlope) - (float)0.5);
        for (int col = minCol; col <= maxCol; col++)
        {
            // Set current world position
            Vec2 pos = Location(origin, quadrant, row, col);

            // Check if current column is visible
            if ((map.GetVisionBlocking(pos) || Symmetric(row, col, startSlope, endSlope)) && (row + Math.Abs(col)) <= range)
            {
                map.SetVisible(pos);
                rowVisible = true;
            }

            // Check if not the first column
            if (col != minCol)
            {
                // Set world position for previous column
                Vec2 posPrev = Location(origin, quadrant, row, col -1);

                // Check if previous location was wall and current location is floor
                if (map.GetVisionBlocking(posPrev) && !map.GetVisionBlocking(pos))
                {
                    startSlope = Slope(row, col);
                }

                // Check if previous location was floor and current location is wall
                if (!map.GetVisionBlocking(posPrev) && map.GetVisionBlocking(pos) && row < range)
                {
                    Scan(map, origin, quadrant, row + 1, startSlope, Slope(row, col), range);
                }
            }

            // Check if last column is floor
            if (col == maxCol && !map.GetVisionBlocking(pos) && rowVisible && row < range)
            {
                Scan(map, origin, quadrant, row + 1, startSlope, endSlope, range);
            }
        }
    }

    // Calculate start slope or end slope
    private static float Slope(int row, int col)
    {
        return ((float)2.0 * (float)col - (float)1.0) / ((float)2.0 * (float)row);
    }

    // Checks if a given location can be seen symmetrically from origin location
    private static bool Symmetric(int row, int col, float startSlope, float endSlope)
    {
        return ((float)col >= (float)row * startSlope && (float)col <= (float)row * endSlope);
    }

    // Transform row/column in quadrant to location in world space
    private static Vec2 Location(Vec2 origin, int quadrant, int row, int col)
    {
        switch (quadrant)
        {
            // North
            case 0: return new Vec2(origin.x + col, origin.y - row);
            // South
            case 1: return new Vec2(origin.x + col, origin.y + row);
            // East
            case 2: return new Vec2(origin.x + row, origin.y + col);
            // West
            case 3: return new Vec2(origin.x - row, origin.y + col);
        }
        return null;
    }
}
