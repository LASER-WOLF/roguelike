namespace Core;

/// <summary>
/// Pseudo random numbers using Linear-Feedback Shift Register (LFSR).
/// </summary>
public static class RandomNumber
{
    
    // 16-bit number
    // taps: 16,15,13,4
    public static bool Next16(ref ushort seed)
    {
        bool tap16 = (seed & 0B_00000000_00000001) == 0B_00000000_00000001;
        bool tap15 = (seed & 0B_00000000_00000010) == 0B_00000000_00000010;
        bool tap13 = (seed & 0B_00000000_00001000) == 0B_00000000_00001000;
        bool tap4  = (seed & 0B_00010000_00000000) == 0B_00010000_00000000;
        seed >>= 1;
        if (tap16 ^ tap15 ^ tap13 ^ tap4)
        { 
            seed |= 0B_10000000_00000000;
            return true;
        }
        return false;
    }
    
    // Generate 16-bit number
    public static ushort Make16(ref ushort seed)
    {
        ushort result = 0B_00000000_00000000;
        if (Next16(ref seed)) { result |= 0B_00000000_00000001; }
        if (Next16(ref seed)) { result |= 0B_00000000_00000010; }
        if (Next16(ref seed)) { result |= 0B_00000000_00000100; }
        if (Next16(ref seed)) { result |= 0B_00000000_00001000; }
        if (Next16(ref seed)) { result |= 0B_00000000_00010000; }
        if (Next16(ref seed)) { result |= 0B_00000000_00100000; }
        if (Next16(ref seed)) { result |= 0B_00000000_01000000; }
        if (Next16(ref seed)) { result |= 0B_00000000_10000000; }
        if (Next16(ref seed)) { result |= 0B_00000001_00000000; }
        if (Next16(ref seed)) { result |= 0B_00000010_00000000; }
        if (Next16(ref seed)) { result |= 0B_00000100_00000000; }
        if (Next16(ref seed)) { result |= 0B_00001000_00000000; }
        if (Next16(ref seed)) { result |= 0B_00010000_00000000; }
        if (Next16(ref seed)) { result |= 0B_00100000_00000000; }
        if (Next16(ref seed)) { result |= 0B_01000000_00000000; }
        if (Next16(ref seed)) { result |= 0B_10000000_00000000; }
        return result;
    }

    // 32-bit number
    // taps: 32,22,2,1
    public static void Next32(ref uint seed)
    {
        bool tap32 = (seed & 0B_00000000_00000000_00000000_00000001) == 0B_00000000_00000000_00000000_00000001;
        bool tap22 = (seed & 0B_00000000_00000000_00000100_00000000) == 0B_00000000_00000000_00000100_00000000;
        bool tap2  = (seed & 0B_01000000_00000000_00000000_00000000) == 0B_01000000_00000000_00000000_00000000;
        bool tap1  = (seed & 0B_10000000_00000000_00000000_00000000) == 0B_10000000_00000000_00000000_00000000;
        seed >>= 1;
        if (tap32 ^ tap22 ^ tap2 ^ tap1) { seed |= 0B_10000000_00000000_00000000_00000000; }
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
