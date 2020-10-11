using Persistence.Models;

namespace Web.Configuration
{
    public class ProjectSettings
    {
        public string MongoHost { get; set; }
        
        public User TestUser { get; set; }
        
        public string EncoderKey { get; set; }
        
        public string EncoderIv { get; set; }
        
        public bool UseProxy { get; set; }
        
        public string ProxyHost { get; set; }
        
        public int ProxyPort { get; set; }
    }
}