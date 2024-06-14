using System.Numerics;

namespace Core;

public static class Noise 
{
    public static float Three(long seed, Vector3 pos)
    {
        return ((OpenSimplex2.Noise3_Fallback(seed, (double)pos.X, (double)pos.Y, (double)pos.Z) + 1f) * 0.5f);
    }
}
