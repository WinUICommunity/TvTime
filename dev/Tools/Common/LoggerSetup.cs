using Serilog;

namespace TvTime.Common;
public static class LoggerSetup
{
    public static ILogger Logger { get; private set; }

    public static void ConfigureLogger()
    {
        Logger = new LoggerConfiguration()
            .Enrich.WithProperty("Version", App.Current.AppVersion)
            .WriteTo.File("Log.txt", rollingInterval: RollingInterval.Day)
            .CreateLogger();
    }
}
