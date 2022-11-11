using ConsoleApplication.Simple;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Ploch.EditorConfigTools.UI.ConsoleUI2
{
    public class ConsoleHost : IHost
    {
        private readonly IHostApplicationLifetime _applicationLifetime;
        private readonly ILogger<ConsoleHost> _logger;
        private readonly IEnumerable<ICommand> _commands;

        public ConsoleHost(IServiceProvider serviceProvider, IHostApplicationLifetime applicationLifetime, ILogger<ConsoleHost> logger, IEnumerable<ICommand> commands)
        {
            _applicationLifetime = applicationLifetime;
            _logger = logger;
            _commands = commands;
            Services = serviceProvider;
        }

        public void Dispose()
        {
            
        }

        public Task StartAsync(CancellationToken cancellationToken = new())
        {
            Console.WriteLine("Application is starting");
            
            var app = new CommandLineApplication();
            
            app.UseServiceProvider(Services);
            foreach (var command in _commands)
            {
                app.Command(command.GetType())
            }
            
            app.Execute(Environment.GetCommandLineArgs());
            _applicationLifetime.StopApplication();
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken = new())
        {
            Console.WriteLine("Application is stopping");
            return Task.CompletedTask;
        }

        public IServiceProvider Services { get; }
    }
}