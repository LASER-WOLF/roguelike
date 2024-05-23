namespace Main;

// https://www.albertford.com/shadowcasting/

public static class Shadowcast
{

    public static void FieldOfView(Map map, Vec2 origin)
    {
        map.SetVisible(origin);

        // Scan all four quadrants (north, east, south, west)
        for (int dir = 0; dir < 4; dir++)
        {
            Scan(map, origin, dir, 1, -1, 1);
        }
    }
    
    // Recursively scan through rows of columns in a given quadrant
    private static void Scan(Map map, Vec2 origin, int dir, int row, float startSlope, float endSlope)
    {
        int minCol = (int)Math.Floor((row * startSlope) + 0.5);
        int maxCol = (int)Math.Ceiling((row * endSlope) - 0.5);
        for (int col = minCol; col <= maxCol; col++)
        {
            // Check if current column is visible
            if (map.GetBlocking(Location(origin, dir, row, col)) || IsSymmetric(row, col, startSlope, endSlope))
            {
                map.SetVisible(Location(origin, dir, row, col));
            }

            // Check if not the first column
            if (col != minCol)
            {
                // Check if previous location was wall and current location is floor
                if (map.GetBlocking(Location(origin, dir, row, col - 1)) && !map.GetBlocking(Location(origin, dir, row, col)))
                {
                    startSlope = MakeSlope(row, col);
                }

                // Check if previous location was floor and current location is wall
                if (!map.GetBlocking(Location(origin, dir, row, col - 1)) && map.GetBlocking(Location(origin, dir, row, col)))
                {
                    Scan(map, origin, dir, row + 1, startSlope, MakeSlope(row, col));
                }
            }

            // Check if last column is floor
            if (col == maxCol && !map.GetBlocking(Location(origin, dir, row, col)))
            {
                Scan(map, origin, dir, row + 1, startSlope, endSlope);
            }
        }
    }

    // Calculate start slope or end slope
    private static float MakeSlope(int row, int col)
    {
        return (2 * col - 1) / (2 * row);
    }

    // Checks if a given location can be seen symmetrically from origin location
    private static bool IsSymmetric(int row, int col, float startSlope, float endSlope)
    {
        return (col >= row * startSlope && col <= row * endSlope);
    }

    // Transform row/column in quadrant to location in world space
    private static Vec2 Location(Vec2 origin, int dir, int row, int col)
    {
        switch (dir)
        {
            // North
            case 0:
                return new Vec2(origin.x + col, origin.y - row);
            // South
            case 1:
                return new Vec2(origin.x + col, origin.y + row);
            // East
            case 2:
                return new Vec2(origin.x + row, origin.y + col);
            // West
            case 3:
                return new Vec2(origin.x - row, origin.y + col);
        }
        return null;
    }
}
