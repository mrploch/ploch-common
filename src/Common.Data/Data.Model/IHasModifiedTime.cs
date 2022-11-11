using System;

namespace Ploch.Common.Data.Model
{
    public interface IHasModifiedTime
    {
        DateTime? ModifiedTime { get; set; }
    }
}