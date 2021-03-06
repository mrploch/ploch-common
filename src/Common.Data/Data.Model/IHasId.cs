﻿namespace Ploch.Common.Data.Model
{
    /// <summary>
    ///     Defines a type that has an identifier.
    /// </summary>
    /// <typeparam name="TId">The type of the identifier.</typeparam>
    public interface IHasId<TId>
    {
        TId Id { get; set; }
    }
}