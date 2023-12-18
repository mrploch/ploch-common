using System;
using System.Collections.Generic;
using Dawn;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Ploch.Common.Collections;
using Ploch.Common.Data.Model;

namespace Ploch.Common.Data.GenericRepository.EFCore;

/// <summary>
///     Provides extension methods for registering repository types in the <see cref="IServiceCollection" />.
/// </summary>
public static class ServiceCollectionRegistration
{
    private static readonly Dictionary<Type, Type> RepositoryTypeMappings = new()
                                                                            {
                                                                                { typeof(IReadRepository<,>), typeof(ReadRepository<,>) },
                                                                                { typeof(IWriteRepository<,>), typeof(ReadWriteRepository<,>) },
                                                                                { typeof(IReadWriteRepository<,>), typeof(ReadWriteRepository<,>) }
                                                                            };

    private static readonly Dictionary<Type, Type> RepositoryAsyncTypeMappings = new()
                                                                                 {
                                                                                     {typeof(IReadRepositoryAsync<>), typeof(ReadRepositoryAsync<>)},
                                                                                     { typeof(IReadRepositoryAsync<,>), typeof(ReadRepositoryAsync<,>) },
                                                                                     { typeof(IWriteRepositoryAsync<,>), typeof(ReadWriteRepositoryAsync<,>) },
                                                                                     { typeof(IReadWriteRepositoryAsync<,>), typeof(ReadWriteRepositoryAsync<,>) }
                                                                                 };

    /// <summary>
    ///     Registers the repository types in the service collection using <c>AddScoped</c> method.
    /// </summary>
    /// <param name="serviceCollection">The service collection.</param>
    /// <typeparam name="TDbContext">The type of DbContext.</typeparam>
    /// <returns>The same service collection.</returns>
    public static IServiceCollection AddRepositories<TDbContext>(this IServiceCollection serviceCollection)
        where TDbContext : DbContext
    {
        Guard.Argument(serviceCollection, nameof(serviceCollection)).NotNull();

        AddRepositories<TDbContext>(serviceCollection, (collection, sourceType, targetType) => collection.AddScoped(sourceType, targetType));
        
        return serviceCollection;
    }

    /// <summary>
    ///     Registers the repository types in the service collection.
    /// </summary>
    /// <param name="serviceCollection">The service collection.</param>
    /// <param name="registrationFunction">The <see cref="IServiceCollection" /> method used for registration.</param>
    /// <typeparam name="TDbContext">The type of DbContext.</typeparam>
    /// <returns>The same service collection.</returns>
    public static IServiceCollection AddRepositories<TDbContext>(this IServiceCollection serviceCollection,
                                                                 Func<IServiceCollection, Type, Type, IServiceCollection> registrationFunction)
        where TDbContext : DbContext
    {
        Guard.Argument(serviceCollection, nameof(serviceCollection)).NotNull();
        Guard.Argument(registrationFunction, nameof(registrationFunction)).NotNull();

        serviceCollection.AddTransient<DbContext>(provider => provider.GetRequiredService<TDbContext>());

        // foreach (var repositoryTypeMapping in RepositoryTypeMappings)
        // {
        //     registrationFunction!(serviceCollection, repositoryTypeMapping.Key, repositoryTypeMapping.Value);
        // }

        RepositoryTypeMappings.ForEach(pair => registrationFunction!(serviceCollection, pair.Key, pair.Value));
        RepositoryAsyncTypeMappings.ForEach(pair => registrationFunction!(serviceCollection, pair.Key, pair.Value));

        serviceCollection.AddScoped<IUnitOfWork, UnitOfWork>();

        return serviceCollection;
    }

    /// <summary>
    ///     Registers a mapping of read / write async repository interfaces to custom repository type in the service
    ///     collection.
    /// </summary>
    /// <remarks>
    ///     This method registers the following mappings:
    ///     <list type="bullet">
    ///         <item>
    ///             <description>
    ///                 <typeparamref name="TRepositoryInterface" /> to <typeparamref name="TRepository" />
    ///             </description>
    ///         </item>
    ///         <item>
    ///             <description>
    ///                 <see cref="IReadWriteRepositoryAsync{TEntity,TId}" /> to <typeparamref name="TRepository" />
    ///             </description>
    ///         </item>
    ///         <item>
    ///             <description>
    ///                 <see cref="IWriteRepositoryAsync{TEntity,TId}" /> to <typeparamref name="TRepository" />
    ///             </description>
    ///         </item>
    ///         <item>
    ///             <description>
    ///                 <see cref="IReadRepositoryAsync{TEntity,TId}" /> to <typeparamref name="TRepository" />
    ///             </description>
    ///         </item>
    ///     </list>
    ///     It allows to specify how the <typeparamref name="TRepository" /> is registered in the service collection using
    ///     the <paramref name="registrationFunction" /> parameter.
    ///     <example>
    ///         Example below shows how to register the custom repository type using the
    ///         <see cref="IServiceCollection" /> <c>AddScoped</c> extension method:
    ///         <code>
    /// <![CDATA[
    ///     services.AddCustomAsyncRepository<ICarReadWriteRepository, CarReadWriteRepository, Car, int>
    ///                 ((collection,
    ///                 sourceType, targetType) => collection.AddScoped(sourceType, targetType));
    /// ]]>
    /// </code>
    ///     </example>
    /// </remarks>
    /// <param name="serviceCollection">The service collection to add the repositories to.</param>
    /// <param name="registrationFunction">The <see cref="IServiceCollection" /> method used for registration.</param>
    /// <typeparam name="TRepositoryInterface">The interface type of the custom repository.</typeparam>
    /// <typeparam name="TRepository">The implementation type of the custom repository.</typeparam>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <typeparam name="TId">The entity <c>Id</c> property type.</typeparam>
    /// <returns>
    ///     The same <see cref="IServiceCollection" /> passed in <paramref name="serviceCollection" /> used for method
    ///     chaining.
    /// </returns>
    public static IServiceCollection AddCustomAsyncRepository<TRepositoryInterface, TRepository, TEntity, TId>(this IServiceCollection serviceCollection,
                                                                                                               Func<IServiceCollection, Type, Type, IServiceCollection>
                                                                                                                   registrationFunction)
        where TRepositoryInterface : class, IReadWriteRepositoryAsync<TEntity, TId>
        where TRepository : class, TRepositoryInterface, IReadWriteRepositoryAsync<TEntity, TId>
        where TEntity : class, IHasId<TId>
    {
        Guard.Argument(serviceCollection, nameof(serviceCollection)).NotNull();
        Guard.Argument(registrationFunction, nameof(registrationFunction)).NotNull();

        // RepositoryTypeMappings.ForEach(pair => registrationFunction!(serviceCollection,
        //                                                             Type.MakeGenericSignatureType(pair.Key, typeof(TEntity), typeof(TId)),
        //                                                             typeof(TRepository)));
        // RepositoryAsyncTypeMappings.ForEach(pair => registrationFunction!(serviceCollection,
        //                                                                  Type.MakeGenericSignatureType(pair.Key, typeof(TEntity), typeof(TId)),
        //                                                                  typeof(TRepository)));

        registrationFunction!(serviceCollection, typeof(TRepositoryInterface), typeof(TRepository));

        return serviceCollection;
    }
}