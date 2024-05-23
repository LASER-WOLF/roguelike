namespace Main;

// https://journal.stuffwithstuff.com/2015/09/07/what-the-hero-sees/

public class Shadow
{
    public float start { get; set; }
    public float end { get; set; }
    
    public Shadow(int row, int col)
    {
        this.start = (float)col / ((float)row + (float)2.0);
        this.end = ((float)col + (float)1.0) / ((float)row + (float)1.0);
    }
}

public class ShadowLine
{
    public List<Shadow> shadows = new List<Shadow>();

    public bool IsVisible(Shadow projection)
    {
        foreach (Shadow shadow in shadows)
        {
            //Logger.Log("shadow: " + shadow.start.ToString() + " - " + shadow.end.ToString() + " | projection: " + projection.start.ToString() + " - " + projection.end.ToString());
            if (shadow.start <= projection.start && shadow.end >= projection.end) { return false; }
        }
        
        return true;
    }

    public bool Add(Shadow shadow)
    {
        // Find out where to put the new shadow in the list
        int index = 0;
        for (index = 0; index < shadows.Count; index++)
        {
            // Stop when hitting the insertion point
            if (shadows[index].start >= shadow.start) { break; }
        }

        Logger.Log("index: " + index.ToString());

        // Check if the new shadow overlaps the previous shadow
        Shadow overlappingPrev = null;
        if (index > 0 && shadows[index - 1].end >= shadow.start)
        {
            overlappingPrev = shadows[index - 1];
        }

        // Check if the new shadow overlaps the next shadow
        Shadow overlappingNext = null;
        if (index < shadows.Count && shadows[index].start <= shadow.end)
        {
            overlappingNext = shadows[index];
        }

        // Overlaps with the next shadow
        if (overlappingNext != null)
        {

            // Overlaps with both shadows so unify one and delete the other
            if (overlappingPrev != null)
            {
                overlappingPrev.end = overlappingNext.end;
                shadows.RemoveAt(index);
            }

            // Overlaps with only the next one so unify with that
            else if (overlappingNext.start > shadow.start)
            {
                overlappingNext.start = shadow.start;
            }
        }

        // Does not overlap with the next shadow
        else
        {

            // Overlaps with only the previous one so unify with that
            if (overlappingPrev != null)
            {
                if (overlappingPrev.end < shadow.end)
                {
                    overlappingPrev.end = shadow.end;
                }
            }

            // Does not overlap with anything so insert to the list
            else
            {
                shadows.Insert(index, shadow);
            }
        }
    
        // Return true if shadow is full
        return (shadows.Count == 1) && (shadows[0].start == 0) && (shadows[0].end == 1.0);
    }


}

// 1. Walk over overy octant
// 2. Walk every tile in an octant
// 3. Update visibility of the current tile
// 4. Update the shadow line if the tile is opaque
public static class FieldOfView
{
    //private static List<Shadow> shadows = new List<Shadow>();

    private static Vec2 TransformOctant(Vec2 origin, int row, int col, int octant)
    {
        switch (octant)
        {
            case 0: return new Vec2(origin.x + col, origin.y + row);
            case 1: return new Vec2(origin.x + col, origin.y - row);
            case 2: return new Vec2(origin.x - col, origin.y + row);
            case 3: return new Vec2(origin.x - col, origin.y - row);
            case 4: return new Vec2(origin.x + row, origin.y + col);
            case 5: return new Vec2(origin.x + row, origin.y - col);
            case 6: return new Vec2(origin.x - row, origin.y + col);
            case 7: return new Vec2(origin.x - row, origin.y - col);
        }
        return null;
    }

    public static void RefreshVisibility(Map map, Vec2 origin)
    {
        map.SetVisible(origin);
        
        for (int octant = 0; octant < 8; octant++)
        {
            RefreshOctant(map, origin, octant);
        }
    }

    private static void RefreshOctant(Map map, Vec2 origin, int octant, int maxRows = 999)
    {
        ShadowLine line = new ShadowLine();
        bool fullShadow = false;

        //shadows.Clear();

        for (int row = 1; row < maxRows; row++)
        {

            // Set current position
            //Vec2 pos = origin + TransformOctant(row, 0, octant);

            // Stop when going out of bounds
            if (!map.InBounds(TransformOctant(origin, row, 0, octant))) { break; }

            // Loop through columns
            for (int col = 0; col <= row; col++)
            {
                if (!fullShadow)
                
                {
                    // Set the current position
                    Vec2 pos = TransformOctant(origin, row, col, octant);

                    // Stop when going out of bounds
                    if (!map.InBounds(pos)) { break; }
                

                    // if (fullShadow)
                    // {
                    //     tiles[pos].isVisible = false;
                    // }
                    // else
                    //Shadow projection = GetProjection(row, col);
                    Shadow projection = new Shadow(row, col);

                    // Set the visibility of the current tile
                    bool visible = line.IsVisible(projection);
                    //map.SetVisible(pos, visible);
                    if (visible) { map.SetVisible(pos); }
                    if (visible) { map.debugMapFov[pos.x, pos.y] = (char)('0' + octant); }
                    //visible = true;
                    //tiles[pos].isVisible = visible;

                    // Add any opaque tile to the shadow map
                    //if (visible && map.GetBlocking(pos))
                    bool blocking = map.GetBlocking(pos);
                    if (blocking)
                    {
                        //fullShadow = line.Add(projection);
                        //fullShadow = line.IsFullShadow;
                        fullShadow = line.Add(projection);
                        //map.debugMapFov[pos.x, pos.y] = (char)('x');
                    }
                    
                    foreach (Shadow shadow in line.shadows)
                    {
                        Logger.Log(shadow.start.ToString() + " " + shadow.end.ToString());
                    }
                    Logger.Err(octant.ToString() + " row:" + row.ToString() + " col:" + col.ToString() + " visible:" + visible.ToString() + " blocking:" + blocking.ToString() + " " + projection.start.ToString() + " " + projection.end.ToString() + " " + fullShadow.ToString());
                }

            }
        }

        //Shadow endprojection = new Shadow(row, col);
        //Logger.Err(octant.ToString() + " " + endprojection.start.ToString() + " " + endprojection.end.ToString());
    }

    // private static Shadow GetProjection(int row, int col)
    // {
    //     float topLeft = (float)col / ((float)row + (float)2);
    //     float bottomRight = ((float)col + (float)1) / ((float)row + (float)1);
    //     return new Shadow(topLeft, bottomRight);
    // }
    
    // private static bool IsInShadow(Shadow projection)
    // {
    //     foreach (Shadow shadow in shadows)
    //     {
    //         if (shadow.Contains(projection)) { return true; }
    //     }
    //     return false;
    // }
    
    // private static bool AddShadow(Shadow shadow)
    // {
    //     // Find out where to put the new shadow in the list
    //     int index = 0;
    //     for (index = 0; index < shadows.Count; index++)
    //     {
    //         // Stop when hitting the insertion point
    //         if (shadows[index].start > shadow.start) { break; }
    //     }

    //     // Check if the new shadow overlaps the previous shadow
    //     Shadow overlappingPrev = null;
    //     if ((index > 0) && (shadows[index - 1].end > shadow.start))
    //     {
    //         overlappingPrev = shadows[index - 1];
    //     }

    //     // Check if the new shadow overlaps the next shadow
    //     Shadow overlappingNext = null;
    //     if ((index < shadows.Count) && (shadows[index].start < shadow.end))
    //     {
    //         overlappingNext = shadows[index];
    //     }

    //     // Overlaps with the next shadow
    //     if (overlappingNext != null)
    //     {

    //         // Overlaps with both shadows so unify one and delete the other
    //         if (overlappingPrev != null)
    //         {
    //             overlappingPrev.end = overlappingNext.end;
    //             shadows.RemoveAt(index);
    //         }

    //         // Overlaps with only the next one so unify with that
    //         else 
    //         {
    //             overlappingNext.start = shadow.start;
    //         }
    //     }

    //     // Does not overlap with the next shadow
    //     else
    //     {

    //         // Overlaps with only the previous one so unify with that
    //         if (overlappingPrev != null)
    //         {
    //             overlappingPrev.end = shadow.end;
    //         }

    //         // Does not overlap with anything so insert to the list
    //         else
    //         {
    //             shadows.Insert(index, shadow);
    //         }
    //     }

    //     // Return true if shadowing everything
    //     return (shadows.Count == 1) && (shadows[0].start == 0) && (shadows[0].end == 1.0);
    // }
}
