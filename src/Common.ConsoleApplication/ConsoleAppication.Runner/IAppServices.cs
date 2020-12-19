using Microsoft.Extensions.DependencyInjection;

namespace Ploch.Common.ConsoleApplication.Runner
{
    public interface IAppServices
    {
        void Configure(IServiceCollection serviceCollection);

    }
}