// unset

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Ploch.Common.Diagnostics;
using Ploch.TestApps.WorkerServiceApp;
using Serilog;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

// Attribute should be "registered" by adding as module or assembly custom attribute
[assembly: Interceptor]

namespace Ploch.TestApps.WorkerServiceApp
{
// Any attribute which provides OnEntry/OnExit/OnException with proper args
    public class Program
    {
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                         .Enrich.FromLogContext()
                         .MinimumLevel.Debug()
                         .WriteTo.Console()
                         .WriteTo.File("c:\\temp\\logfile.log", rollingInterval: RollingInterval.Day)
                         .CreateLogger();


            Log.Debug("Starting up");
            Log.Debug("Shutting down");

            IHost host = CreateHostBuilder(args).Build();
            ILogger<Program>? logger = host.Services.GetService<ILogger<Program>>();
            logger.LogInformation("Service built and starting...");
            IHostLifetime? hostLifetime = host.Services.GetService<IHostLifetime>();
            if (args.Contains("--install", StringComparer.OrdinalIgnoreCase))
            {
                string location = Assembly.GetExecutingAssembly().Location;
                string? directory = Path.GetDirectoryName(location);
                string? fileName = Path.GetFileNameWithoutExtension(location);
                string exePath = $"{directory}\\{fileName}.exe";
                Process process = ProcessHelper.Start("sc.exe",
                                                      eventArgs => logger.LogInformation(string.Format("sc.exe: {0}",
                                                                                                       eventArgs)),
                                                      "create",
                                                      "my-awesome-service",
                                                      $"binPath={exePath}",
                                                      "DisplayName='My Awesome Service'");
                process.WaitForExit();
                Console.WriteLine($"Service installed, exit code: {process.ExitCode}");

                process = ProcessHelper.Start("sc.exe",
                                              eventArgs => logger.LogInformation($"sc.exe: {eventArgs}"),
                                              "description",
                                              "my-awesome-service",
                                              "This is Mr Ploch's My Awesome Service");
                process.WaitForExit();
                Console.WriteLine($"Description set: {process.ExitCode}");

                return;
            }

            host.Run();
        }

        private void RunServiceCommand(string command, params string[] arguments)
        {
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                       .UseWindowsService()
                       .ConfigureServices((hostContext, services) =>
                                          {
                                              services.AddLogging(loggingBuilder => loggingBuilder.AddSerilog(dispose: true))
                                                      .AddHostedService<Worker>();
                                          });
        }
    }
}