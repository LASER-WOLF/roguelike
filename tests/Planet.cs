using Raylib_cs;
//using System.Numerics;

namespace Core;

/// <summary>
/// ...
/// </summary>
public class Planet
{
    public readonly int size;
    private bool[,,] tilemaps;

    public Planet(int size)
    {
        this.size = size;
        this.tilemaps = new bool[6,size,size];
        Generate();
    }

    private void Generate()
    {
        //...
    }
}
