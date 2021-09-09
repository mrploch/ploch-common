using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Ploch.Common.ConsoleApplication.Core;
using Ploch.Common.DependencyInjection;

namespace Ploch.Common.ConsoleApplication.Runner
{
    public class RequiredServices : IServicesBundle
    {
        private readonly IEnumerable<string> _args;
        private readonly IEnumerable<Type> _commandTypes;

        public RequiredServices(IEnumerable<string> args, IEnumerable<Type> commandTypes)
        {
            _args = args;
            _commandTypes = commandTypes;
        }

        public void Configure(IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton(new StartupContext(_args));
            foreach (var commandType in _commandTypes)
            {
                serviceCollection.AddTransient(commandType);
            }
            serviceCollection.AddSingleton<IOutput, ConsoleOutput>();
        }
    }
}