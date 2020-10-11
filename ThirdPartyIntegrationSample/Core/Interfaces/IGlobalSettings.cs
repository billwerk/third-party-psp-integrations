namespace Core.Interfaces
{
    public interface IGlobalSettings
    {
        string MongoHost { get; }

        IUser TestUser { get; }
        
        string EncoderKey { get; }
        
        string EncoderIv { get; }
        
        public bool UseProxy { get; set; }
        
        public string ProxyHost { get; set; }
        
        public int ProxyPort { get; set; }
    }
}