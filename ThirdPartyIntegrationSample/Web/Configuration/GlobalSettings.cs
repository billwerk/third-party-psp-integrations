using Microsoft.Extensions.Options;
using Persistence.Interfaces;
using Persistence.Models;

namespace Web.Configuration
{
    public class GlobalSettings : IGlobalSettings
    {
        public GlobalSettings(IOptions<ProjectSettings> options)
        {
            MongoHost = options?.Value?.MongoHost;
            TestUser = options?.Value?.TestUser;
        }

        public string MongoHost { get; }
        public User TestUser { get; }
    }
}