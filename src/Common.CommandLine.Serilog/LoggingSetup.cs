using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Events;

namespace Ploch.Common.CommandLine.Serilog
{
    public static class LoggingSetup
    {
        public static IServiceCollection SetupSerilog(this IServiceCollection serviceCollection, string? logName = null, string? logPath = null)
        {
            var loggerConfiguration = new LoggerConfiguration().Enrich.FromLogContext()
                                                               .Enrich.WithThreadId()
                                                               .Enrich.WithThreadName()
                                                               .Enrich.FromLogContext()
                                                               .WriteTo.File(BuildFullLogPath(logName, logPath),
                                                                             rollOnFileSizeLimit: true,
                                                                             fileSizeLimitBytes: 2 * (1024 * 1024),

                                                                             //  outputTemplate: template,
                                                                             retainedFileCountLimit: 10)
                                                               .WriteTo.Logger(l => l.Filter.ByIncludingOnly(logEvent =>
                                                                                                                 logEvent.Level is LogEventLevel.Error
                                                                                                                     or LogEventLevel.Warning
                                                                                                                     or LogEventLevel.Fatal))
                                                               .WriteTo.File(BuildFullLogPath(logName, logPath, "errors"),
                                                                             rollOnFileSizeLimit: true,
                                                                             fileSizeLimitBytes: 2 * (1024 * 1024),

                                                                             //  outputTemplate: template,
                                                                             retainedFileCountLimit: 10)
                                                               .WriteTo.Console();
            
            serviceCollection.AddLogging(builder => builder.AddSerilog(loggerConfiguration.CreateLogger()));

            return serviceCollection;
        }

        private static string BuildFullLogPath(string? logName, string? logPath, string? suffix = null)
        {
            logName = logName ?? EnvironmentUtilities.GetCurrentAppPath();
            logPath = logPath ?? AppDomain.CurrentDomain.BaseDirectory;
            suffix = suffix != null ? $"-{suffix}" : null;

            return Path.Combine(logPath, $"{logName}{suffix}.log");
        }
    }
}