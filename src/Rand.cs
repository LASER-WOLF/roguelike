namespace Core;

static class Rand 
{
    public static Random random { get; private set; } = new Random();
    
    public static bool Percent(int i)
    {
        return (random.Next(99) < i); 
    }
}
