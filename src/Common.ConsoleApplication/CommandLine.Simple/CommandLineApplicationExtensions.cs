using System.Reflection;
using System.Runtime.CompilerServices;
using McMaster.Extensions.CommandLineUtils;
using Ploch.Common;

namespace ConsoleApplication.Simple
{
    public static class CommandLineApplicationExtensions
    {
        public static CommandLineApplication<TModel> AddValidator<TModel>(this CommandLineApplication<TModel> application, PreExecuteCommandValidator validator)
            where TModel : class
        {
            application.Validators.Add(new DelegatedCommandValidator(validator));
            return application;
        }
        
        
        public static CommandLineApplication<TModel> Command<TModel>(this CommandLineApplication application,
                                                                     Action<CommandLineApplication<TModel>>? configuration = null) where TModel : class
        {
            var commandType = typeof(TModel);
            
            return application.Command(ValidateGetName(commandType), configuration);
        }
        
        public static CommandLineApplication Command(this CommandLineApplication application, Type commandType,
                                                                     Action<CommandLineApplication>? configuration = null)/* where TModel : class*/
        {
            return application.Command(ValidateGetName(commandType), configuration);
        }

        public static CommandLineApplication UseServiceProvider(this CommandLineApplication application, IServiceProvider serviceProvider)
        {
            application.Conventions.UseConstructorInjection(serviceProvider);
            return application;
        }

        private static string ValidateGetName(Type commandType)
        {
            var commandAttribute = commandType.GetCustomAttribute<CommandAttribute>(false);

            if (commandAttribute == null)
            {
                throw new InvalidOperationException($"Type {commandType.Name} is not decorated with {nameof(CommandAttribute)} attribute.");
            }

            if (commandAttribute.Name.IsNullOrEmpty())
            {
                throw new
                    InvalidOperationException($"{nameof(CommandAttribute)} on type {commandType.Name} has to have a non-null or empty {nameof(CommandAttribute.Name)} property");
            }

            return commandAttribute.Name;
        }
        
        // public static IConventionBuilder UseAutofac(this IConventionBuilder conventionBuilder, IContainer autofacContainer)
        // {
        //     var serviceProvider = new AutofacServiceProvider(autofacContainer);
        //     return conventionBuilder.UseConstructorInjection(serviceProvider);
        // }
        //
        // public static IConventionBuilder UseAutofac(this IConventionBuilder conventionBuilder,
        //                                             ContainerBuilder containerBuilder,
        //                                             IServiceCollection? serviceCollection = null)
        // {
        //     if (serviceCollection != null)
        //     {
        //         containerBuilder.Populate(serviceCollection);
        //     }
        //
        //     return conventionBuilder.UseAutofac(containerBuilder.Build());
        // }
        //
        // public static TApp UseAutofac<TApp>(this TApp application, 
        //                                     ContainerBuilder containerBuilder,
        //                                     IServiceCollection? serviceCollection = null) where TApp : CommandLineApplication
        // {
        //     application.Conventions.UseAutofac(containerBuilder, serviceCollection);
        //     return application;
        // }
    }
}