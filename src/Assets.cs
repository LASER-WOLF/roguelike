namespace Main;

/// <summary>
/// Shared assets.
/// </summary>
static class Assets 
{
    public static Dictionary<string, Tile> tiles { set; get; } = new Dictionary<string, Tile>();

    static Assets()
    {
        tiles.Add("void", new Tile('.', ConsoleColor.Red));
        tiles.Add("wall", new Tile('#', ConsoleColor.Gray));
        tiles.Add("empty", new Tile(' ', ConsoleColor.White));
        tiles.Add("grass", new Tile('`', ConsoleColor.Green));
        tiles.Add("test", new Tile('!', ConsoleColor.DarkRed));
        tiles.Add("door", new Tile('D', ConsoleColor.Cyan));
        tiles.Add("light", new Tile('L', ConsoleColor.Black));
    }
}
