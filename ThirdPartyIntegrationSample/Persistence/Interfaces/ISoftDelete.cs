using System;

namespace Persistence.Interfaces
{
    public interface ISoftDelete
    {
        DateTime? DeletedAt { get; }
    }
}