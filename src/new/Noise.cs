using System.Numerics;

namespace Core;

public static class Noise 
{
    public static float Simplex3(long seed, Vector3 pos)
    {
        return ((OpenSimplex2.Noise3_Fallback(seed, (double)pos.X, (double)pos.Y, (double)pos.Z) + 1f) * 0.5f);
    }
    
    public static float Simplex2(long seed, Vector2 pos)
    {
        return ((OpenSimplex2.Noise2(seed, (double)pos.X, (double)pos.Y) + 1f) * 0.5f);
    }
}

/// <summary>
/// Based on Improved Perlin Noise
/// </summary>
public static class Perlin {    
    
    private static readonly int[] p = new int[512];
    private static readonly int[] permutation = { 151,160,137,91,90,15,
        131,13,201,95,96,53,194,233,7,225,140,36,103,30,69,142,8,99,37,240,21,10,23,
        190, 6,148,247,120,234,75,0,26,197,62,94,252,219,203,117,35,11,32,57,177,33,
        88,237,149,56,87,174,20,125,136,171,168, 68,175,74,165,71,134,139,48,27,166,
        77,146,158,231,83,111,229,122,60,211,133,230,220,105,92,41,55,46,245,40,244,
        102,143,54, 65,25,63,161, 1,216,80,73,209,76,132,187,208, 89,18,169,200,196,
        135,130,116,188,159,86,164,100,109,198,173,186, 3,64,52,217,226,250,124,123,
        5,202,38,147,118,126,255,82,85,212,207,206,59,227,47,16,58,17,182,189,28,42,
        223,183,170,213,119,248,152, 2,44,154,163, 70,221,153,101,155,167, 43,172,9,
        129,22,39,253, 19,98,108,110,79,113,224,232,178,185, 112,104,218,246,97,228,
        251,34,242,193,238,210,144,12,191,179,162,241, 81,51,145,235,249,14,239,107,
        49,192,214, 31,181,199,106,157,184, 84,204,176,115,121,50,45,127, 4,150,254,
        138,236,205,93,222,114,67,29,24,72,243,141,128,195,78,66,215,61,156,180
    };

    // Constructor
    static Perlin() { for(int i = 0; i < 512; i++) { p[i] = permutation[i % 256]; } }

    // Generate combined octaves of 3D noise
    public static float Octave3(Vector3 pos, int octaves = 4, float persistence = 0.5f) {
    	float result = 0;
    	float frequency = 1;
    	float amplitude = 1;
    	float amount = 0;
    	for(int i = 0; i < octaves; i++)
        {
    		result += Noise3(pos * frequency) * amplitude;
    		amount += amplitude;
    		amplitude *= persistence;
    		frequency *= 2;
    	}
    	return result / amount;
    }

    // Generate 3D noise
    public static float Noise3(Vector3 pos) {
        // Find unit cube that contains point
        int x = (int)MathF.Floor(pos.X) & 255;
        int y = (int)MathF.Floor(pos.Y) & 255;
        int z = (int)MathF.Floor(pos.Z) & 255;

        // Find relative position of point in cube
        pos.X -= MathF.Floor(pos.X);
        pos.Y -= MathF.Floor(pos.Y);
        pos.Z -= MathF.Floor(pos.Z);
        
        // Compute fade curves
        float u = Fade(pos.X);
        float v = Fade(pos.Y);
        float w = Fade(pos.Z);
    
        // Hash coordinates of the cube corners
        int aaa = p[p[p[x  ]+y  ]+z  ];
        int aba = p[p[p[x  ]+y+1]+z  ];
        int aab = p[p[p[x  ]+y  ]+z+1];
        int abb = p[p[p[x  ]+y+1]+z+1];
        int baa = p[p[p[x+1]+y  ]+z  ];
        int bba = p[p[p[x+1]+y+1]+z  ];
        int bab = p[p[p[x+1]+y  ]+z+1];
        int bbb = p[p[p[x+1]+y+1]+z+1];
   
        // Return blend results from all corners of cube
        return (Lerp(
                    Lerp(
                        Lerp(
                            Grad (aaa, pos.X     , pos.Y     , pos.Z),
                            Grad (baa, pos.X - 1f, pos.Y     , pos.Z),
                            u),
                        Lerp(
                            Grad (aba, pos.X     , pos.Y - 1f, pos.Z),
                            Grad (bba, pos.X - 1f, pos.Y - 1f, pos.Z),
                            u),
                        v),
                    Lerp(
                        Lerp(
                            Grad (aab, pos.X     , pos.Y     , pos.Z - 1f),
                            Grad (bab, pos.X - 1f, pos.Y     , pos.Z - 1f),
                            u),
                        Lerp(
                            Grad (abb, pos.X     , pos.Y - 1f, pos.Z - 1f),
                            Grad (bbb, pos.X - 1f, pos.Y - 1f, pos.Z - 1f),
                            u),
                        v),
                    w)
                + 1f) / 2f;
    }

    public static float Grad(int hash, float x, float y, float z) {
        // Mask lo 4 bits of hash code
        // h = 1 1 1 1
        //     | | | |
        //     | | | bit 0
        //     | | bit 1
        //     | bit 2
        //     bit 3
        int h = (hash & 15);
        
        // u = x if hash bit 3 is 0
        float u = h < 8 ? x : y;
		
        // v = y if hash bit 2 and 3 are both 0
        // else: v = x if hash bits are 1100 or 1110
        // else: v = z
        float v = h < 4 ? y : h == 12 || h == 14 ? x : z;
    	
        // Set u to negative if bit 0 is 1
        // Set u to negative if bit 1 is 1
        // Return u + v
        return ((h & 1) == 0 ? u : -u) + ((h & 2) == 0 ? v : -v);
    }

    // Linear interpolation between two floats
    public static float Lerp(float a, float b, float x) { return a + x * (b - a); }
    
    // Apply an ease curve to a float value by using the function 6t^5 - 15t^4 + 10t^3
    public static float Fade(float t) { return t * t * t * (t * (t * 6f - 15f) + 10f); }
}
