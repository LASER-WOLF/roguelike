namespace Main;

/// <summary>
/// Main class.
/// </summary>
class Game
{
    private enum ERROR {
        NONE,
        WINDOW_SIZE
    }

    public static bool debug {get; private set; } = false;
    private int windowWidth     = 0;
    private int windowHeight    = 0;
    private int minWidth        = 10;
    private int minHeight       = 10;
    private ERROR error         = ERROR.NONE;
    private List<string> buffer = new List<string>();
    private bool isRunning      = true;
    private Map map;

    public Game()
    {
        Run();
    }

    private void Run() 
    {
        // Clear console and hide cursor
        Console.CursorVisible = false;
        ConsoleClear();
        
        // Make the map
        map = new Map(128, 48);
        
        // Game loop
        while (isRunning)
        {
            buffer.Clear();
            ErrorCheckWindowSize();
            Update();
            Render();
            HandleInput();
        }
        
        // Reset console
        ConsoleClear();
        Console.CursorVisible = true;
    }

    private void Update()
    {
        if (debug) {
            buffer.Add("[DEBUG MODE]");
            buffer.Add("WINDOW SIZE: " + windowWidth + "x" + windowHeight);
            buffer.Add("");
        }
        else 
        {
            buffer.Add("PRESS [D] FOR DEBUG MODE");
            buffer.Add("");
        }
        
        buffer.Add("PRESS [ESC] TO QUIT");
    }

    private void Render() 
    {
        // Clear the console
        string blank = new String(' ', windowWidth);
        Console.SetCursorPosition(0, 0);
        for (int i = 0; i < windowHeight; i++)
        {
            Console.Write(blank + Environment.NewLine);
        }
        Console.SetCursorPosition(0, 0);
        
        if (error == ERROR.NONE)
        {
            // Print log
            if (debug)
            {
                Console.WriteLine();
                Logger.Print();
            }
        
            // Print buffer
            Console.WriteLine();
            foreach (string line in buffer)
            {
                Console.WriteLine(line);
            }

            // Render map
            Console.WriteLine();
            map.Render();

        }
        else
        {
            ErrorPrint();
        }
    }

    private void ErrorPrint() 
    {
        Console.WriteLine("ERROR!");
        switch (error)
        {
            case ERROR.WINDOW_SIZE:
                Console.WriteLine("WINDOW SIZE");
                Console.WriteLine("TOO SMALL!");
                break;
        }
    }

    private void ErrorCheckWindowSize()
    {
        if ((windowWidth = Console.WindowWidth) < minWidth || (windowHeight = Console.WindowHeight) < minHeight) 
        {
            error = ERROR.WINDOW_SIZE;
        }
    }

    // Reset console
    private void ConsoleClear() 
    {
        Console.ResetColor();
        Console.Clear();
    }

    private void HandleInput() 
    {
        // Loop until valid input is given
        bool validKey = false;
        while (!validKey)
        {
            // Wait for and get key input from user
            ConsoleKeyInfo key = Console.ReadKey(true);

            if (error == ERROR.NONE) { validKey = HandleInputMain(key); }
            else { validKey = HandleInputError(key); }
        }
    }

    private bool HandleInputMain(ConsoleKeyInfo key)
    {
            switch (key.Key)
            {
                case ConsoleKey.UpArrow:
                    return map.player.MoveUp();
                case ConsoleKey.DownArrow:
                    return map.player.MoveDown();
                case ConsoleKey.LeftArrow:
                    return map.player.MoveLeft();
                case ConsoleKey.RightArrow:
                    return map.player.MoveRight();
                case ConsoleKey.Escape:
                    isRunning = false;
                    return true;
                case ConsoleKey.D:
                    debug = !debug;
                    return true;
            }

            return false;
    }
    
    private bool HandleInputError(ConsoleKeyInfo key)
    {
            switch (key.Key)
            {
                case ConsoleKey.Escape:
                    isRunning = false;
                    return true;
            }

            return false;
    }
}
