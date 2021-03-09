using CG.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Ploch.Common.ConsoleApplication.Core;
using Ploch.Common.DependencyInjection;

namespace Ploch.Common.ConsoleApplication.Runner
{
    public class DefaultServices : IServicesBundle
    {
        /// <inheritdoc />
        public void Configure(IServiceCollection serviceCollection)
        {
            serviceCollection.AddLogging(cfg => cfg.AddConsole());
            serviceCollection.AddSingleton<IOutput, ConsoleOutput>();
        }
    }
}