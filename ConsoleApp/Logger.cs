using System;

namespace ConsoleApp
{
  public class Logger
  {
    public enum LogLevelEnumeration
    {
      All,
      Trace,
      Info,
      Warn,
      Error,
      Fatal,
    }

    public static Logger Instance { get; } = new();

    private readonly object lockObject = new();

    public string DirectoryPath
    {
      get
      {
        var current = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) ?? string.Empty;

        var now = DateTime.Now;

        var path = System.IO.Path.Combine(
          current,
          "Logs",
          $"{now:yyyyMMdd}"
        );

        return path;
      }
    }

    private Logger()
    {
    }

    public void Write(LogLevelEnumeration level, string? type, string? message)
    {
      this.Write(level, type, message, null);
    }

    public void Write(LogLevelEnumeration level, string? type, string? message, string? split)
    {
      lock (this.lockObject)
      {
        var app = System.Reflection.Assembly.GetEntryAssembly()?.GetName()?.Name;
        var now = DateTime.Now;
        var output = $"[{now:yyyy/MM/dd HH:mm:ss.fff}][{level}][{type}] {message}";

        var logFilePath = System.IO.Path.Combine(
          this.DirectoryPath,
          string.IsNullOrEmpty(split) ? $"{app}.log" : $"{app}.{split}.log"
        );

        var directoryName = System.IO.Path.GetDirectoryName(logFilePath);
        if (directoryName is null)
          return;

        System.IO.Directory.CreateDirectory(directoryName);
        using (var stream = new System.IO.StreamWriter(logFilePath, true))
          stream.WriteLine(output);
      }
    }

    public void Write(LogLevelEnumeration level, string? type, Exception exception)
    {
      this.Write(level, type, exception, null);
    }

    public void Write(LogLevelEnumeration level, string? type, Exception exception, string? split)
    {
      #region LocalMethod
      static List<string> __ExceptionMessage(Exception e)
      {
        var list = new List<string>();

        if (e is null)
          return list;

        list.Add($" Type={e.GetType()}");
        list.Add($" Object={e.Source ?? "null"}");
        list.Add($" Method={e.TargetSite?.ToString() ?? "null"}");
        list.Add($" StackTrace={e.StackTrace ?? "null"}");
        list.Add($" HResult={e.HResult.ToString() ?? "null"}");
        list.Add($" Message={e.Message ?? "null"}");

        foreach (var key in e.Data.Keys)
          list.Add($" Data[{key}]={e.Data[key]?.ToString() ?? "null"}");

        list.Add($" HelpLink={e.HelpLink ?? "null"}");

        return list;
      }
      #endregion

      var messages = new List<string>();

      messages.Add("Exception");
      messages.AddRange(
        __ExceptionMessage(exception)
      );

      var innerException = exception.InnerException;
      while (innerException is not null)
      {
        messages.Add("InnerException");
        messages.AddRange(
          __ExceptionMessage(innerException)
        );

        innerException = innerException.InnerException;
      }

      this.Write(level, type, messages, split);
    }

    public void Write(LogLevelEnumeration level, string? type, List<string> messages)
    {
      this.Write(level, type, messages.ToArray(), null);
    }

    public void Write(LogLevelEnumeration level, string? type, List<string> messages, string? split)
    {
      this.Write(level, type, messages.ToArray(), split);
    }

    public void Write(LogLevelEnumeration level, string? type, string[] messages)
    {
      this.Write(level, type, messages, null);
    }

    public void Write(LogLevelEnumeration level, string? type, string[] messages, string? split)
    {
      lock (this.lockObject)
      {
        var app = System.Reflection.Assembly.GetEntryAssembly()?.GetName()?.Name;
        var now = DateTime.Now;

        var output = new System.Text.StringBuilder();
        output.AppendLine($"[{now:yyyy/MM/dd HH:mm:ss.fff}][{level}][{type}]");

        foreach (var message in messages)
          output.AppendLine(message);

        var logFilePath = System.IO.Path.Combine(
          this.DirectoryPath,
          string.IsNullOrEmpty(split) ? $"{app}.log" : $"{app}.{split}.log"
        );

        var directoryName = System.IO.Path.GetDirectoryName(logFilePath);
        if (directoryName is null)
          return;

        System.IO.Directory.CreateDirectory(directoryName);
        using (var stream = new System.IO.StreamWriter(logFilePath, true))
          stream.Write(output.ToString());
      }
    }
  }
}
