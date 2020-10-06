namespace Core.Interfaces
{
    public interface IGlobalSettings
    {
        string MongoHost { get; }

        IUser TestUser { get; }
        
        string EncoderKey { get; }
        
        string EncoderIv { get; }
    }
}