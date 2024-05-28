namespace Core;

/// <summary>
/// Shared class for random generation.
/// </summary>
static class Rand 
{
    public static Random random { get; private set; } = new Random();
    
    public static bool Percent(int i)
    {
        return (random.Next(99) < i); 
    }
}
