namespace Main;

public class Tile
{
    public char symbol { set; get; }
    public ConsoleColor color { set; get; }

    public Tile(char symbol, ConsoleColor color)
    {
        this.symbol = symbol;
        this.color = color;
    }
}
