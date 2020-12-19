using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Ploch.Common.ConsoleApplication.Core;

namespace Ploch.Common.ConsoleApplication.Runner
{
    public class DefaultServices : IAppServices
    {
        /// <inheritdoc />
        public void Configure(IServiceCollection serviceCollection)
        {
            serviceCollection.AddLogging(cfg => cfg.AddConsole());
            serviceCollection.AddSingleton<IOutput, ConsoleOutput>();
        }
    }
}