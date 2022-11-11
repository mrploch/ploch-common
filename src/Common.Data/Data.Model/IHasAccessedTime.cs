using System;

namespace Ploch.Common.Data.Model
{
    public interface IHasAccessedTime
    {
        DateTime? AccessedTime { get; set; }
    }
}