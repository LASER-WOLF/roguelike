namespace Core;

/// <summary>
/// A entry in the Logger.
/// </summary>
public class LogEntry
{
    public readonly string message;
    public readonly bool error;

    public LogEntry(string message, bool error = false)
    {
        this.message = message;
        this.error = error;
    }
}
