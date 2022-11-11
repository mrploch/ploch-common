using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Ploch.Common.ConsoleApplication.Runner;
using Ploch.Common.DependencyInjection;
using Serilog;
using Serilog.Events;

namespace ConsoleApplication.Logging.Serilog
{
    public class LoggingServices : IServicesBundle
    {
        private const string DefaultTemplate = "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level}] [{SourceContext}] {Message}{NewLine}{Exception}";
        
        private readonly IConfiguration? _configuration;
        private readonly string _template;
        private readonly Action<LoggerConfiguration>? _loggerConfigurationAction;

        public LoggingServices(IConfiguration? configuration = null, string? template = DefaultTemplate, Action<LoggerConfiguration>? loggerConfigurationAction = null)
        {
            _configuration = configuration;
            _template = template ?? DefaultTemplate;
            _loggerConfigurationAction = loggerConfigurationAction;
        }

        public void Configure(IServiceCollection serviceCollection)
        {
            var loggerConfiguration = new LoggerConfiguration().Enrich
                                                               .FromLogContext()
                                                               // .WriteTo.PersistentFile(logPath,
                                                               //                         preserveLogFilename: true,
                                                               //                         rollOnFileSizeLimit: true,
                                                               //                         fileSizeLimitBytes: sizeLimitBytes,
                                                               //                         outputTemplate: template,
                                                               //                         hooks: logRolloverAction != null ?
                                                               //                                    new DelegateFileLifecycleHook(logRolloverAction) :
                                                               //                                    null)
                                                               .MinimumLevel.Information()
                                                               .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                                                               .WriteTo.Console(outputTemplate: _template)
                                                               .Enrich.FromLogContext();
            
            _loggerConfigurationAction?.Invoke(loggerConfiguration);

            if (_configuration != null)
            {
                loggerConfiguration = loggerConfiguration.ReadFrom.Configuration(_configuration);
            }

            var logger = loggerConfiguration.CreateLogger();

            serviceCollection.AddLogging(builder =>
                                         {
                                             builder.AddSerilog(logger);
                                             builder.AddFilter("Microsoft.EntityFrameworkCore.Database.Command", LogLevel.Warning);
                                         });
        }
    }

    public static class AppBuilderSerilogExtensions
    {
        public static TAppBuilder WithSerilog<TAppBuilder>(this TAppBuilder appBuilder, IConfiguration? configuration = null, string? template = null, Action<LoggerConfiguration>? loggerConfigurationAction = null) where TAppBuilder : AppBuilder
        {
            appBuilder.WithServices(new LoggingServices(configuration, template, loggerConfigurationAction));
            return appBuilder;
        }
    }
}