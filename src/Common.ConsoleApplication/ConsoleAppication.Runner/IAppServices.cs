using Microsoft.Extensions.DependencyInjection;
using Ploch.Common.ConsoleApplication.Core;

namespace Ploch.Common.ConsoleApplication.Runner
{
    public interface IAppServices
    {
        void Configure(IServiceCollection serviceCollection);

    }

    
}