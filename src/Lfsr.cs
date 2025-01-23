namespace Core;

/// <summary>
/// Generate pseudo random numbers using Linear-Feedback Shift Register (LFSR).
/// </summary>
public static class Lfsr
{
    // Shift 16-bit seed, using taps: 15,14,12,3 (zero-indexed)
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

    // Shift 32-bit seed, using taps: 31,21,1,0 (zero-indexed)
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
    
    // Shift 64-bit seed, using taps: 63,62,60,59 (zero-indexed)
    public static void Shift64(ref ulong seed)
    {
        bool result = ((
        ((seed >> 63) & 0B_00000000_00000000_00000000_00000000_00000000_00000000_00000000_00000001) ^
        ((seed >> 62) & 0B_00000000_00000000_00000000_00000000_00000000_00000000_00000000_00000001) ^
        ((seed >> 60) & 0B_00000000_00000000_00000000_00000000_00000000_00000000_00000000_00000001) ^
        ((seed >> 59) & 0B_00000000_00000000_00000000_00000000_00000000_00000000_00000000_00000001)
        ) == 00000000_00000000_00000000_00000000_00000000_00000000_00000000_00000001);
        seed <<= 1;
        if (result) { seed |= 00000000_00000000_00000000_00000000_00000000_00000000_00000000_00000001; }
    }
    
    // Generate bool from 16-bit seed
    public static bool Check16(ref ushort seed)
    {
        Shift16(ref seed);
        if ((seed & 0B_00000000_00000001) == 0B_00000000_00000001) { return true; }
        return false;
    }

    // Generate bool from 32-bit seed
    public static bool Check32(ref uint seed)
    {
        Shift32(ref seed);
        if ((seed & 0B_00000000_00000000_00000000_00000001) == 0B_00000000_00000000_00000000_00000001) { return true; }
        return false;
    }
    
    // Generate bool from 64-bit seed
    public static bool Check64(ref ulong seed)
    {
        Shift64(ref seed);
        if ((seed & 0B_00000000_00000000_00000000_00000000_00000000_00000000_00000000_00000001) == 0B_00000000_00000000_00000000_00000000_00000000_00000000_00000000_00000001) { return true; }
        return false;
    }

    // Generate unsigned 16-bit integer from 16-bit seed
    public static ushort Make16(ref ushort seed)
    {
        ushort result = 0B_00000000_00000000;
        ushort mask   = 0B_00000000_00000001;
        for (int i = 0; i < 16; i++)
        {
            Shift16(ref seed);
            if ((seed & 0B_00000000_00000001) == 0B_00000000_00000001) { result |= mask; }
            mask <<= 1;
        }
        return result;
    }

    // Generate unsigned 32-bit integer from 32-bit seed
    public static uint Make32(ref uint seed, uint min = 0, uint max = 0)
    {
        uint result = 0B_00000000_00000000_00000000_00000000;
        uint mask   = 0B_00000000_00000000_00000000_00000001;
        for (int i = 0; i < 32; i++)
        {
            Shift32(ref seed);
            if ((seed & 0B_00000000_00000000_00000000_00000001) == 0B_00000000_00000000_00000000_00000001) { result |= mask; }
            mask <<= 1;
        }
        if (max > min)
        {
            if (result > max) { result %= max; }
            if (result < min) { result += (min - result); }
        }
        return result;
    }
    
    // Generate unsigned 64-bit integer from 64-bit seed
    public static ulong Make64(ref ulong seed)
    {
        ulong result = 0B_00000000_00000000_00000000_00000000_00000000_00000000_00000000_00000000;
        ulong mask   = 0B_00000000_00000000_00000000_00000000_00000000_00000000_00000000_00000001;
        for (int i = 0; i < 64; i++)
        {
            Shift64(ref seed);
            if ((seed & 0B_00000000_00000000_00000000_00000000_00000000_00000000_00000000_00000001) == 0B_00000000_00000000_00000000_00000000_00000000_00000000_00000000_00000001) { result |= mask; }
            mask <<= 1;
        }
        return result;
    }

    // Generate signed 16-bit integer from 16-bit seed
    public static short MakeShort(ref ushort seed, bool negative = false)
    {
        if (negative) { return (short)Make16(ref seed); }
        return (short)(Make16(ref seed) & 0B_01111111_11111111);
    }
    
    // Generate signed 32-bit integer from 32-bit seed
    public static int MakeInt(ref uint seed, bool negative = false, uint min = 0, uint max = 0)
    {
        if (negative) { return (int)Make32(ref seed); }
        return (int)(Make32(ref seed, min, max) & 0B_01111111_11111111_11111111_11111111);
    }
    
    // Generate signed 64-bit integer from 64-bit seed
    public static long MakeLong(ref ulong seed, bool negative = false)
    {
        if (negative) { return (long)Make64(ref seed); }
        return (long)(Make64(ref seed) & 0B_01111111_11111111_11111111_11111111_11111111_11111111_11111111_11111111);
    }
}
