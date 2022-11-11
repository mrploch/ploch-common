using System;

namespace Ploch.Common.Data.Model
{
    public interface IHasCreatedTime
    {
        DateTime? CreatedTime { get; set; }
    }
}