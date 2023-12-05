using System;
using System.IO;

namespace EventLogger;

public enum LogLevel
{
    Default,
    Filter,
    Transform,
    Save
}

public class EventLogger
{
    private static EventLogger _instance;
    private static readonly object lockObject = new object();
    private string _logFilePath = Path.Combine("D:\\Valera\\122_22_2\\OOP\\XMLAnalyzer\\EventLogger\\" , "event_log.txt");

    private EventLogger() { }

    public static EventLogger Instance
    {
        get
        {
            lock (lockObject)
            {
                return _instance ??= new EventLogger();
            }
        }
    }

    public void LogEvent(LogLevel level, string message)
    {
        string logEntry = $"{DateTime.Now:dd.MM.yyyy HH:mm:ss} {level}. {message}";
        
        SaveLogToFile(logEntry);
    }

    private void SaveLogToFile(string logEntry)
    {
        try
        {
            using (StreamWriter writer = new StreamWriter(_logFilePath, true))
            {
                writer.WriteLine(logEntry);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving log to file: {ex.Message}");
        }
    }
}

