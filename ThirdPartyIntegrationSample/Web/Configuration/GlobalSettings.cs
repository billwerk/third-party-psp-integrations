using Core.Interfaces;
using Microsoft.Extensions.Options;

namespace Web.Configuration
{
    public class GlobalSettings : IGlobalSettings
    {
        public GlobalSettings(IOptions<ProjectSettings> options)
        {
            MongoHost = options?.Value?.MongoHost;
            TestUser = options?.Value?.TestUser;
            EncoderKey = options?.Value?.EncoderKey;
            EncoderIv = options?.Value?.EncoderIv;
            UseProxy = options.Value.UseProxy;
            ProxyHost = options.Value.ProxyHost;
            ProxyPort = options.Value.ProxyPort;
        }

        public string MongoHost { get; }
        public IUser TestUser { get; }
        public string EncoderKey { get; }
        public string EncoderIv { get; }
        public bool UseProxy { get; set; }
        public string ProxyHost { get; set; }
        public int ProxyPort { get; set; }
    }
}