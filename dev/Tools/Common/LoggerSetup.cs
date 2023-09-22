using Serilog;

namespace TvTime.Common;
public static class LoggerSetup
{
    public static ILogger Logger { get; private set; }

    public static void ConfigureLogger()
    {
        var desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        var logFileDirPath = Path.Combine(desktopPath, Constants.AppName);
        var logFilePath = Path.Combine(logFileDirPath, "Log.txt");
        if (!Directory.Exists(logFileDirPath))
        {
            Directory.CreateDirectory(logFileDirPath);
        }

        Logger = new LoggerConfiguration()
            .Enrich.WithProperty("Version", App.Current.AppVersion)
            .WriteTo.File(logFilePath, rollingInterval: RollingInterval.Day)
            .WriteTo.Debug()
            .CreateLogger();
    }
}
