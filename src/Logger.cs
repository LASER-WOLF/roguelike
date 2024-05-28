namespace Core;

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

}
