using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Ardalis.GuardClauses;
using CommandLine;
using CommandLine.Text;
using Microsoft.Extensions.DependencyInjection;
using Ploch.Common.ConsoleApplication.Core;
using Ploch.Common.DependencyInjection;
using Ploch.Common.Reflection;
using Unity.Microsoft.DependencyInjection;

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
    public class AppBootstrapper: IAppBootstrapper
    {
        private readonly IServicesBundle _appServices;
        private readonly Func<IServiceCollection, IServiceProvider> _serviceProviderFunc;

        /// <summary>
        /// Constructs <c>AppBootstrapper</c> using default options.
        /// </summary>
        public AppBootstrapper() : this(new DefaultServices())
        { }

        /// <summary>
        /// Constructs <c>AppBootstrapper</c>
        /// </summary>
        /// <remarks>
        ///     Use this constructor to supply a custom <see cref="IServiceProvider"/>.
        ///     <para>
        ///         The delegate will be called with a default and pre-configured <see cref="IServiceCollection"/> which should be added
        ///         to the custom <c>IServiceProvider.</c>
        ///     </para>
        /// </remarks>
        /// <param name="serviceProviderFunc">Delegate for a custom service provider.</param>
        /// <param name="appServices"></param>
        public AppBootstrapper(IServicesBundle appServices): this(services => ServiceProviderExtensions.BuildServiceProvider(services), appServices)
        { }

        /// <summary>
        /// Constructs <c>AppBootstrapper</c>
        /// </summary>
        /// <remarks>
        ///     Use this constructor to supply a custom <see cref="IServiceProvider"/>.
        ///     <para>
        ///         The delegate will be called with a default and pre-configured <see cref="IServiceCollection"/> which should be added
        ///         to the custom <c>IServiceProvider.</c>
        ///     </para>
        /// </remarks>
        /// <param name="serviceProviderFunc">Delegate for a custom service provider.</param>
        /// <param name="appServices"></param>
        public AppBootstrapper(Func<IServiceCollection, IServiceProvider> serviceProviderFunc, IServicesBundle appServices)
        {
            _serviceProviderFunc = serviceProviderFunc;  // Guard.Against.Default(serviceProviderFunc, nameof(serviceProviderFunc));
            _appServices = appServices;
        }

        /// <summary>
        ///     Executes the app.
        /// </summary>
        /// <param name="args">The args.</param>
        /// <exception cref="InvalidOperationException">Thrown when command of type <typeparamref name="TApp"/> could not be resolved.</exception>
        public void ExecuteApp<TApp>(string[] args) where TApp : ICommand
        {
            var serviceProvider = InitializeServiceProvider();
            var application = serviceProvider.GetService<TApp>();
            if (application == null)
            {
                throw new InvalidOperationException($"Command of type {typeof(TApp)} could not be resolved!.");
            }
            application.Execute(args);
        }

        /// <summary>
        ///     Executes the apps.
        /// </summary>
        /// <param name="args">The args.</param>
        /// <param name="commands">The commands.</param>
        /// <exception cref="ArgumentException">Thrown when provided commands do not implement <see cref="ICommand{TOptions}"/> interface.</exception>
        [SuppressMessage("ReSharper", "PossibleMultipleEnumeration",
                         Justification = "Does not have any performance implications in this case and would make code less readable.")]
        public void ExecuteApp(IEnumerable<string> args, IEnumerable<Type> commands)
        {
            Guard.Against.Null(args, nameof(args));
            Guard.Against.Null(commands, nameof(commands));

            var appArgumentsMapping = commands.ToDictionary(AppCommandsResolver.GetArgumentsType, app => app);

            var serviceProvider = InitializeServiceProvider();
            var parser = new Parser(settings =>
                                          {
                                              settings.CaseSensitive = false;
                                              settings.HelpWriter = null;
                                          });
            var parserResult = parser.ParseArguments(args, appArgumentsMapping.Keys.ToArray())
                                     .WithParsed(parsedArgs =>
                                                 {
                                                     var commandType = appArgumentsMapping[parsedArgs.GetType()];
                                                     if (!commandType.IsImplementing(typeof(ICommand<>)))
                                                     {
                                                         throw new ArgumentException("One of the supplied application types is invalid.",
                                                                                     nameof(commands));
                                                     }

                                                     var command = serviceProvider.GetService(commandType);
                                                     var executeMethod = commandType.GetMethod("Execute");
                                                     Guard.Against.Null(executeMethod, nameof(executeMethod));
                                                     executeMethod.Invoke(command, new[] {parsedArgs});
                                                 });
                                                      
            parserResult.WithNotParsed(errors => DisplayHelp(parserResult, errors, serviceProvider));

        }

        /// <summary>
        ///     Executes the app.
        /// </summary>
        /// <param name="args">The args.</param>
        public void ExecuteApp<TApp, TArgs>(string[] args) where TApp : ICommand<TArgs>
        {
            var serviceProvider = InitializeServiceProvider();
            var application = serviceProvider.GetRequiredService<TApp>();
            var appEvents = serviceProvider.GetServices<IAppEvents>();
            appEvents = appEvents.OrderBy(ae => ae.Order);
            foreach (var appEvent in appEvents)
            {
                appEvent.OnStartup(serviceProvider);
            }

            var parser = new Parser(settings =>
                                    {
                                        settings.HelpWriter = null;
                                        settings.CaseSensitive = false;
                                    });
            
            var parserResult = parser.ParseArguments<TArgs>(args);
            
            parserResult.WithParsed(parsed => application.Execute(parsed)).WithNotParsed(errors => DisplayHelp(parserResult, errors, serviceProvider));
        }


        /// <summary>
        ///     Initializes the service provider.
        /// </summary>
        /// <returns>An IServiceProvider.</returns>
        private IServiceProvider InitializeServiceProvider()
        {
            var services = new ServiceCollection();
            _appServices.Configure(services);
            var provider = _serviceProviderFunc(services);
            return provider ?? throw new InvalidOperationException("Could not initialize Service Provider!");
        }

        /// <summary>
        ///     Displays the help.
        /// </summary>
        /// <param name="result">The result.</param>
        /// <param name="errors"></param>
        /// <param name="serviceProvider"></param>
        private static void DisplayHelp<T>(ParserResult<T> result, IEnumerable<Error> errors, IServiceProvider serviceProvider)
        {
            var helpText = HelpText.AutoBuild(result/*, h => HelpText.DefaultParsingErrorsHandler(result, h), e => e*/);
            var output = serviceProvider.GetRequiredService<IOutput>();

            output.WriteErrorLine(helpText);
        }
    }
}