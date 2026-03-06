using AutoMapper;
using Ardalis.Result;
using Ploch.Data.Model;
using Ploch.Data.GenericRepository;
using Microsoft.Extensions.Caching.Memory;
using Ploch.Common.WebApi.Endpoints.Models;

namespace Ploch.Common.WebApi.Endpoints.CrudEndpoints.GetById;
public abstract class GetByIdEndpointHandler<TEntity, TId, TDto, TGetByIdRequest, TGetByIdResponse>(
    IReadRepositoryAsync<TEntity, TId> repository,
    IMapper mapper,
    EntityQueryOperations<TEntity> operations,
    IMemoryCache cache)
    : IGetByIdEndpointHandler<TEntity, TId, TDto, TGetByIdRequest, TGetByIdResponse>
    where TEntity : class, IHasId<TId> where TGetByIdRequest : IdRequest<TId> where TGetByIdResponse : DataTransferObjectResponse<TDto>
{
    public virtual async Task<Result<TGetByIdResponse>> HandleAsync(TGetByIdRequest request, CancellationToken cancellationToken)
    {
        if (cache.TryGetValue($"{typeof(TEntity).Name}_{request.Id}", out TEntity cachedEntity))
        {
            var fromCachedDto = mapper.Map<TDto>(cachedEntity);

            return CreateResponse(cachedEntity, fromCachedDto, request);
        }

        var entity = await GetEntity(request, cancellationToken);

        if (entity is null)
        {
            return Result<TGetByIdResponse>.NotFound($"Item with id {request.Id} was not found.");
        }

        cache.Set($"{typeof(TEntity).Name}_{request.Id}", entity, new MemoryCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5) });

        var dto = mapper.Map<TDto>(entity);

        return CreateResponse(entity, dto, request);
    }

    protected virtual Task<TEntity?> GetEntity(TGetByIdRequest request, CancellationToken cancellationToken) =>
        operations.GetByIdOperation != null
            ? repository.GetByIdAsync(request.Id, operations.GetByIdOperation, cancellationToken)
            : repository.GetByIdAsync(request.Id, cancellationToken: cancellationToken);

    protected abstract TGetByIdResponse CreateResponse(TEntity entity, TDto dto, TGetByIdRequest request);
}

public class GetByIdEndpointHandler<TEntity, TId, TDto, TGetByIdRequest>(
    IReadRepositoryAsync<TEntity, TId> repository,
    IMapper mapper,
    EntityQueryOperations<TEntity> operations,
    IMemoryCache cache)
    : GetByIdEndpointHandler<TEntity, TId, TDto, TGetByIdRequest, DataTransferObjectResponse<TDto>>(repository, mapper, operations, cache),
      IGetByIdEndpointHandler<TEntity, TId, TDto, TGetByIdRequest>
    where TEntity : class, IHasId<TId> where TGetByIdRequest : IdRequest<TId>
{
    protected override DataTransferObjectResponse<TDto> CreateResponse(TEntity entity, TDto dto, TGetByIdRequest request) => new(dto);
}

public class GetByIdEndpointHandler<TEntity, TId, TDto>(
    IReadRepositoryAsync<TEntity, TId> repository,
    IMapper mapper,
    EntityQueryOperations<TEntity> operations,
    IMemoryCache cache)
    : GetByIdEndpointHandler<TEntity, TId, TDto, IdRequest<TId>>(repository, mapper, operations, cache),
      IGetByIdEndpointHandler<TEntity, TId, TDto>
    where TEntity : class, IHasId<TId>
{ }
