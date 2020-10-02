namespace Core.Rest
{
    public enum HttpContentType
    {
        FormUrlEncodedContent,
        MultipartContent,
        StreamContent,
        StringContent,
        ByteArrayContent,
        MultipartFormDataContent,
        ReadOnlyMemoryContent
    }
}