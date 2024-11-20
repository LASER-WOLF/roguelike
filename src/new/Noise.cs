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
