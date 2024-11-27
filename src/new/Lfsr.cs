namespace Core;

/// <summary>
/// Generate pseudo random numbers using Linear-Feedback Shift Register (LFSR).
/// </summary>
public static class Lfsr
{
    // Shift 16-bit seed, using zero-indexed taps: 15,14,12,3
    public static void Shift16(ref ushort seed)
    {
        bool result = ((
        ((seed >> 15) & 0B_00000000_00000001) ^
        ((seed >> 14) & 0B_00000000_00000001) ^
        ((seed >> 12) & 0B_00000000_00000001) ^
        ((seed >> 3)  & 0B_00000000_00000001)
        ) == 0B_00000000_00000001);
        seed <<= 1;
        if (result) { seed |= 0B_00000000_00000001; }
    }

    // Generate unsigned 16-bit integer from 16-bit seed
    public static ushort Make16(ref ushort seed)
    {
        ushort result = 0B_00000000_00000000;
        ushort mask   = 0B_00000000_00000001;
        for (int i = 0; i < 16; i++)
        {
            Shift16(ref seed);
            mask <<= 1;
            if ((seed & 0B_00000000_00000001) == 0B_00000000_00000001) { result |= mask; }
        }
        return result;
    }
    
    // Shift 32-bit seed, using zero-indexed taps: 31,21,1,0
    public static void Shift32(ref uint seed)
    {
        bool result = ((
        ((seed >> 31) & 0B_00000000_00000000_00000000_00000001) ^
        ((seed >> 21) & 0B_00000000_00000000_00000000_00000001) ^
        ((seed >> 1)  & 0B_00000000_00000000_00000000_00000001) ^
        ((seed >> 0)  & 0B_00000000_00000000_00000000_00000001)
        ) == 0B_00000000_00000000_00000000_00000001);
        seed <<= 1;
        if (result) { seed |= 0B_00000000_00000000_00000000_00000001; }
    }

    // Generate unsigned 32-bit integer from 32-bit seed
    public static uint Make32(ref uint seed, uint min = 0, uint max = 0)
    {
        uint result = 0B_00000000_00000000_00000000_00000000;
        uint mask   = 0B_00000000_00000000_00000000_00000001;
        for (int i = 0; i < 32; i++)
        {
            Shift32(ref seed);
            mask <<= 1;
            if ((seed & 0B_00000000_00000000_00000000_00000001) == 0B_00000000_00000000_00000000_00000001) { result |= mask; }
        }
        //if (max > min) { result % (max - min) + min }
        if (max > min) { result = result % (min > 0 ? max + min : max) + (min > 0 ? min : 0); }

        return result;
    }

    // Generate signed 16-bit integer from 16-bit seed
    public static int MakeShort(ref ushort seed)
    {
        return (short)Make16(ref seed);
    }
    
    // Generate signed 32-bit integer from 32-bit seed
    public static int MakeInt(ref uint seed, bool negative = false, int min = 0, int max = 0)
    {
        int result = (int)Make32(ref seed, (uint)min, (uint)max);
        if (!negative) { result &= 0B_01111111_11111111_11111111_11111111; }
        //Logger.Log("uint - seed: " + seed.ToString() + " - num: " + result.ToString());
        return result;
    }
    
    // 64-bit number 
    // taps: 64,63,61,60
    public static void Next64(ref ulong seed)
    {
        bool tap64 = (seed & 0B_00000000_00000000_00000000_00000000_00000000_00000000_00000000_00000001) == 0B_00000000_00000000_00000000_00000000_00000000_00000000_00000000_00000001;
        bool tap63 = (seed & 0B_00000000_00000000_00000000_00000000_00000000_00000000_00000000_00000010) == 0B_00000000_00000000_00000000_00000000_00000000_00000000_00000000_00000010;
        bool tap61 = (seed & 0B_00000000_00000000_00000000_00000000_00000000_00000000_00000000_00001000) == 0B_00000000_00000000_00000000_00000000_00000000_00000000_00000000_00001000;
        bool tap60 = (seed & 0B_00000000_00000000_00000000_00000000_00000000_00000000_00000000_00010000) == 0B_00000000_00000000_00000000_00000000_00000000_00000000_00000000_00010000;
        seed >>= 1;
        if (tap64 ^ tap63 ^ tap61 ^ tap60) { seed |= 0B_10000000_00000000_00000000_00000000_00000000_00000000_00000000_00000000; }
    }
}
