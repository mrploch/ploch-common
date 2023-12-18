﻿using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Ploch.Common.Data.Model;

namespace Ploch.Common.Data.GenericRepository;


public interface IReadWriteRepositoryAsync<TEntity, in TId> : IReadRepositoryAsync<TEntity, TId>, IWriteRepositoryAsync<TEntity, TId>
    where TEntity : class, IHasId<TId>
{ }