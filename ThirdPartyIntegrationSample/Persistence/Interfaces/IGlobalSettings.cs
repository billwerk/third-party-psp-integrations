using Persistence.Models;

namespace Persistence.Interfaces
{
    public interface IGlobalSettings
    {
        string MongoHost { get; }

        User TestUser { get; }
    }
}