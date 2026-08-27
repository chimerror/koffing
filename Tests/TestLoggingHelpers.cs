using Serilog;

public static class TestLoggingHelpers
{
	public static string LoggingPrefix = "KoffingTests";

	public static void SetupLogging()
	{
		Log.Logger = new LoggerConfiguration()
			.MinimumLevel.Debug()
			.WriteTo.Console(outputTemplate: "{Level:u3} {Message}{NewLine}{Exception}")
			.CreateLogger();
	}

	public static void PrefixInfo(string message)
	{
		Log.Information($"{LoggingPrefix}: {message}");
	}

	public static void TearDownLogging()
	{
		Log.CloseAndFlush();
	}
}