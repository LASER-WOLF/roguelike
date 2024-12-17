namespace Core;

/// <summary>
/// A entry in the Logger.
/// </summary>
public class LogEntry
{
    public readonly string message;
    public readonly bool error;
    public readonly long time;

    public LogEntry(string message, bool error = false)
    {
        this.message = message;
        this.error = error;
        this.time = DateTime.Now.Ticks;
    }
}
