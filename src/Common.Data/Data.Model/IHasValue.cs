// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IHasValue.cs" company="Catel development team">
//   Copyright (c) 2008 - 2020 Catel development team. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Ploch.Common.Data.Model
{
    public interface IHasValue<TValue>
    {
        TValue Value { get; set; }
    }
}