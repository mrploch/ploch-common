using System;
using System.Collections.Generic;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ploch.EditorConfigTools.ConsoleUI;

namespace Ploch.Common.CommandLine
{
    public class AppConstructionContainer
    {
        public AppConstructionContainer(CommandLineApplication application, IServiceCollection serviceCollection, IConfiguration configuration)
        {
            Application = application;
            ServiceCollection = serviceCollection;
            Configuration = configuration;
        }

        public CommandLineApplication Application { get; }
        
        public IServiceCollection ServiceCollection { get; }

        public IConfiguration Configuration { get; }

        public Func<IServiceProvider>? ServiceProviderFactory { get; set; }
        //   public IConfiguration Configuration { get; }

    }
    
    public class AppBuilder
    {
        private readonly List<Action<IConfigurationBuilder>> _configurationBuilderActions = new();
        private Action<IConfigurationBuilder>? _configurationBuilderAction;

        private readonly Func<CommandLineApplication> _appBuildFunc;
        private readonly List<Action<AppConstructionContainer>> _configurationActions = new();

        private AppBuilder(Func<CommandLineApplication>? appBuildFunc, params Action<AppConstructionContainer>[] configurationActions )
        {
           // _configurationBuilderAction = configurationBuilderAction;
            _appBuildFunc = appBuildFunc ?? (() => new CommandLineApplication());
            foreach (var configurationAction in configurationActions)
            {
                _configurationActions.Add(configurationAction);
            }
                
        }

        public static AppBuilder CreateDefault(Action<IConfigurationBuilder>? configurationAction = null)
        {
          //  var configurationBuilderAction = configurationAction ?? 
            var builder = new AppBuilder(() => new CommandLineApplication(),
                                         container => container.Application.Conventions.UseDefaultConventions().UseCommandAttribute().UseCommandNameFromModelType());

            return builder.WithConfiguration(configurationAction ?? (configurationBuilder => ConfigurationSetup.DefaultFileConfiguration(configurationBuilder)));
        }
        
        public AppBuilder Configure(Action<AppConstructionContainer> configurationAction)
        {
            _configurationActions.Add(configurationAction);

            return this;
        }

        private AppBuilder WithConfiguration(Action<IConfigurationBuilder> configurationBuilderAction)
        {
            _configurationBuilderActions.Add(configurationBuilderAction);
            
            return this;
        }

        public CommandLineApplication Build()
        {
            var app = _appBuildFunc();
            var serviceCollections = new ServiceCollection();

            var configurationBuilderAction = _configurationBuilderAction ?? (builder => ConfigurationSetup.DefaultFileConfiguration(builder));

            var configurationBuilder = new ConfigurationBuilder();

            configurationBuilderAction(configurationBuilder);

            var configuration = configurationBuilder.Build();

            var configurationContainer = new AppConstructionContainer(app, serviceCollections, configuration);

            foreach (var configurationAction in _configurationActions)
            {
                configurationAction(configurationContainer);
            }

            var serviceProviderFactory = configurationContainer.ServiceProviderFactory ?? (() => configurationContainer.ServiceCollection.BuildServiceProvider());

            app.Conventions.UseConstructorInjection(serviceProviderFactory());

            return app;
        }
    }
}