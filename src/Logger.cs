namespace Main;

/// <summary>
/// Logs messages and errors for debugging.
/// </summary>
static class Logger
{
    public static List<LogEntry> log { get; private set; } = new List<LogEntry>();

    public static void Log(string message)
    {
        log.Add(new LogEntry(message));
    }
    
    public static void Err(string message)
    {
        log.Add(new LogEntry(message, error: true));
    }

    public static void Print()
    {
        foreach (LogEntry logEntry in log)
        {
            if (logEntry.error) { Console.ForegroundColor = ConsoleColor.Red; }
            Console.WriteLine(logEntry.message);
            if (logEntry.error) { Console.ResetColor(); }
        }
    }

}
