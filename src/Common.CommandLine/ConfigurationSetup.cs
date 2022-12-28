using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Autofac;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ploch.Common;

namespace Ploch.EditorConfigTools.ConsoleUI
{
    public static class ConfigurationSetup
    {
        public static IServiceCollection SetupConfiguration(this IServiceCollection serviceCollection, string[] args,  string configurationFileName = "appsettings.json", Action<IConfigurationBuilder>? configurationBuilderAction = null)
        {
            return SetupConfiguration(serviceCollection, args, out _, configurationFileName, configurationBuilderAction);
        }

        public static IServiceCollection SetupConfiguration(this IServiceCollection serviceCollection, string[] args, out IConfiguration configuration,  string configurationFileName = "appsettings.json", Action<IConfigurationBuilder>? configurationBuilderAction = null)
        {
            return SetupConfiguration(serviceCollection, args, new []{ configurationFileName}, out configuration, configurationBuilderAction);
        }

        public static IServiceCollection SetupConfiguration(this IServiceCollection serviceCollection, string[] args, IEnumerable<string>? configurationFileNames, out IConfiguration configuration, Action<IConfigurationBuilder>? configurationBuilderAction = null)
        {
            if (configurationFileNames == null)
            {
                configurationFileNames = new[] { "appsettings.json" };
            }

            var basePath = EnvironmentUtilities.GetCurrentAppPath();
            var configurationBuilder = new ConfigurationBuilder();
            var builder = configurationBuilder.SetBasePath(basePath);
            foreach (var fileName in configurationFileNames)
            {
                if (File.Exists(Path.Combine(basePath, fileName)))
                {
                    builder.AddJsonFile(fileName);
                }
            }
            builder.AddCommandLine(args).AddEnvironmentVariables();
            configurationBuilderAction?.Invoke(configurationBuilder);
            configuration = configurationBuilder.Build();
            serviceCollection.AddSingleton(configuration);

            return serviceCollection;
        }
    }
}