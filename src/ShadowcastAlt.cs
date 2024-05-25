namespace Main;

// Implementation based on:
// Bob Nystrom, What the Hero Sees: Field-of-View for Roguelikes
// https://journal.stuffwithstuff.com/2015/09/07/what-the-hero-sees/

public static class ShadowcastAlt
{
    // Go through every octant and set visible locations
    public static void Run(Map map, Vec2 origin)
    {
        map.SetVisible(origin);
        
        // Scan all eight octants
        for (int octant = 0; octant < 8; octant++)
        {
            Scan(map, origin, octant);
        }
    }
    
    private class Shadow
    {
        public float start { get; set; }
        public float end { get; set; }
        
        public Shadow(int row, int col)
        {
            this.start = (float)col / ((float)row + (float)2.0);
            this.end = ((float)col + (float)1.0) / ((float)row + (float)1.0);
        }
    }
    
    private class Shadowlist
    {
        private List<Shadow> shadows = new List<Shadow>();
        public bool fullShadow { get; private set; }
    
        // Check if current location is visible or covered in shadow
        public bool IsVisible(Shadow projection)
        {
            foreach (Shadow shadow in shadows)
            {
                if (shadow.start <= projection.start && shadow.end >= projection.end) { return false; }
            }
            return true;
        }
    
        // Add shadow to shadowlist
        public void Add(Shadow shadow)
        {
            // Find out where to put the new shadow in the list
            int index = 0;
            for (index = 0; index < shadows.Count; index++)
            {
                // Stop when hitting the insertion point
                if (shadows[index].start >= shadow.start) { break; }
            }
    
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
        
            // Set fullshadow to true if shadow goes from 0 to 1
            fullShadow = (shadows.Count == 1) && (shadows[0].start == (float)0) && (shadows[0].end == (float)1.0);
        }
    }

    // Transform row/column in octant to world location 
    private static Vec2 Location(Vec2 origin, int row, int col, int octant)
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

    // Loop through every location in octant
    private static void Scan(Map map, Vec2 origin, int octant, int maxRows = 99)
    {
        // Create a new shadowlist
        Shadowlist shadowlist = new Shadowlist();

        // Loop through row by row until going out of bounds or reaching fullShadow
        bool endScan = false;
        for (int row = 1; row < maxRows; row++)
        {
            // Loop through columns in the current row
            bool rowVisible = false;
            for (int col = 0; col <= row; col++)
            {
                // Set the current world location
                Vec2 pos = Location(origin, row, col, octant);

                // Stop when going out of bounds or reaching fullShadow
                if (shadowlist.fullShadow || (!map.InBounds(pos) && col == 0)) { endScan = true; break; }
                else if (!map.InBounds(pos)) { break; }
                
                // Make shadow projection for current location
                Shadow projection = new Shadow(row, col);

                // Check if location is visible
                if (shadowlist.IsVisible(projection)) 
                { 
                    map.SetVisible(pos); 
                    rowVisible = true; 
                    
                    // Add shadow projection to shadowlist if location blocks view
                    if (map.GetBlocking(pos)) { shadowlist.Add(projection); }
                }
            }

            // End the scan
            if (!rowVisible || endScan){ break; }
        }
    }
}
