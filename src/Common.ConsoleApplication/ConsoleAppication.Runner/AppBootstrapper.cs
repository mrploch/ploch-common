using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Ardalis.GuardClauses;
using CommandLine;
using CommandLine.Text;
using Microsoft.Extensions.DependencyInjection;
using Ploch.Common.ConsoleApplication.Core;
using Unity.Microsoft.DependencyInjection;
using Validation;

namespace Ploch.Common.ConsoleApplication.Runner
{
    /// <summary>
    ///     The application bootstrapper.
    ///     Configures services, parses command line arguments and executes the application.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Application, or App in the framework is an implementation of <see cref="ICommand" /> or
    ///         <see cref="ICommand{TOptions}" /> interface. App is providing all business logic.
    ///     </para>
    ///     <para>
    ///         An App instance is resolved by the <c>AppBootstrapper</c> using dependency injection container.
    ///         Any services required by an App can be registered: [TODO: Registrations]
    ///     </para>
    ///     <para>
    ///         It is also possible to have many discrete functions provided by an application.
    ///         In this case, it is also possible to split the implementation into several sub-commands.
    ///         Each of them can then have separate command line arguments.
    ///         Sub-commands are defined the same way as single app though only the generic, <see cref="ICommand{TOptions}" />
    ///         is supported.
    ///     </para>
    /// </remarks>
    public class AppBootstrapper
    {
        private readonly Func<IServiceCollection, IServiceProvider> _serviceProviderFunc;

        public AppBootstrapper() : this(services => services.BuildServiceProvider())
        { }

        public AppBootstrapper(Func<IServiceCollection, IServiceProvider> serviceProviderFunc)
        {
            _serviceProviderFunc = Guard.Against.Default(serviceProviderFunc, nameof(serviceProviderFunc));
            //_serviceProviderFunc = serviceProviderFunc ?? throw new ArgumentNullException(nameof(serviceProviderFunc));
        }

        /// <summary>
        ///     Executes the app.
        /// </summary>
        /// <param name="args">The args.</param>
        public void ExecuteApp<TApp>(string[] args) where TApp : ICommand
        {
            var serviceProvider = InitializeServiceProvider();
            var application = serviceProvider.GetService<TApp>();
            application.Execute(args);
        }

        /// <summary>
        ///     Executes the apps.
        /// </summary>
        /// <param name="args">The args.</param>
        /// <param name="commands">The commands.</param>
        [SuppressMessage("ReSharper", "PossibleMultipleEnumeration",
            Justification = "Does not have any performance implications in this case and would make code less readable.")]
        public void ExecuteApp(IEnumerable<string> args, IEnumerable<Type> commands)
        {
            Requires.NotNull(args, nameof(args));
            Requires.NotNull(commands, nameof(commands));

            var appArgumentsMapping = commands.ToDictionary(AppCommandsResolver.GetArgumentsType, app => app);

            var serviceProvider = InitializeServiceProvider();
            var parser = new Parser(settings =>
                                    {
                                        settings.CaseSensitive = false;
                                        settings.HelpWriter = null;
                                    });

            var parserResult = parser.ParseArguments(args, appArgumentsMapping.Keys.ToArray());
            
            parserResult.WithParsed(parsedArgs =>
                                    {
                                        var commandType = appArgumentsMapping[parsedArgs.GetType()];
                                        var command = serviceProvider.GetService(commandType);
                                        var executeMethod = commandType.GetMethod("Execute");
                                        
                                        if (executeMethod != null)
                                            executeMethod.Invoke(command, new[] {parsedArgs});
                                        else
                                            throw new ArgumentException("One of the supplied application types is invalid.", nameof(commands));
                                    })
                        .WithNotParsed(errors => DisplayHelp(parserResult, errors));
        }

        /// <summary>
        ///     Executes the app.
        /// </summary>
        /// <param name="args">The args.</param>
        public void ExecuteApp<TApp, TArgs>(string[] args) where TApp : ICommand<TArgs>
        {
            var serviceProvider = InitializeServiceProvider();
            var application = serviceProvider.GetService<TApp>();
            var parser = new Parser(with => with.HelpWriter = null);
            var parserResult = parser.ParseArguments<TArgs>(args);
            parserResult.WithParsed(parsed => application.Execute(parsed)).WithNotParsed(errors => DisplayHelp(parserResult, errors));
        }

        /// <summary>
        ///     Initializes the service provider.
        /// </summary>
        /// <returns>An IServiceProvider.</returns>
        private IServiceProvider InitializeServiceProvider()
        {
            var services = new ServiceCollection();
            var serviceConfig = new DefaultServices();
            serviceConfig.Configure(services);
            var provider = _serviceProviderFunc(services);
            if (provider == null) throw new InvalidOperationException("Could not initialize service provider.");

            return provider;
        }

        /// <summary>
        ///     Displays the help.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <param name="errs">The errs.</param>
        private static void DisplayHelp<T>(ParserResult<T> result, IEnumerable<Error> errs)
        {
            var helpText = HelpText.AutoBuild(result, h =>
                                                      {
                                                          //   h.AdditionalNewLineAfterOption = false;
                                                          //   h.Heading = "Myapp 2.240.0-beta"; //change header
                                                          //   h.Copyright = "Copyright (c) 2019 Global.com"; //change copyright text
                                                          return HelpText.DefaultParsingErrorsHandler(result, h);
                                                      }, e => e);
            Console.WriteLine(helpText);
        }
    }
}