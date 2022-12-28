using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Autofac;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ploch.Common;
using Ploch.Common.CommandLine;

namespace Ploch.EditorConfigTools.ConsoleUI
{
    public static class ConfigurationSetup
    {
        // public static AppBuilder UseConfiguration(this AppBuilder appBuilder,
        //                                           string[] args,
        //                                           string configurationFileName = "appsettings.json",
        //                                           Action<IConfigurationBuilder>? configurationBuilderAction = null)
        // {
        //     appBuilder.Configure(container => )
        // }
        // private static AppBuilder SetupConfiguration(this IServiceCollection serviceCollection, string[] args,  string configurationFileName = "appsettings.json", Action<IConfigurationBuilder>? configurationBuilderAction = null)
        // {
        //     return SetupConfiguration(serviceCollection, args, out _, configurationFileName, configurationBuilderAction);
        // }
        //
        // private static void SetupConfiguration(this IServiceCollection serviceCollection, string[] args, out IConfiguration configuration,  string configurationFileName = "appsettings.json", Action<IConfigurationBuilder>? configurationBuilderAction = null)
        // {
        //     builder.AddConfigurationAction(container => SetupConfiguration(container, args, new[] { configurationFileName }));
        //     return SetupConfiguration(serviceCollection, args, new []{ configurationFileName}, out configuration, configurationBuilderAction);
        // }

        public static void DefaultFileConfiguration(IConfigurationBuilder configurationBuilder, IEnumerable<string>? configurationFileNames = null, Action<IConfigurationBuilder>? configurationBuilderAction = null)
        {
          //  var configurationBuilder = new ConfigurationBuilder();
            
            if (configurationFileNames == null)
            {
                configurationFileNames = new[] { "appsettings.json" };
            }

            var basePath = EnvironmentUtilities.GetCurrentAppPath();
            
            var builder = configurationBuilder.SetBasePath(basePath);
            foreach (var fileName in configurationFileNames)
            {
                if (File.Exists(Path.Combine(basePath, fileName)))
                {
                    builder.AddJsonFile(fileName);
                }
            }

            builder.AddCommandLine(EnvironmentUtilities.GetEnvironmentCommandLine().ToArray()).AddEnvironmentVariables();
            configurationBuilderAction?.Invoke(configurationBuilder);
           //  var configuration = configurationBuilder.Build();
           // // serviceCollection.AddSingleton(configuration);
           //
           //  return configuration;
        }
    }
}