namespace Core.Rest
{
    public enum HttpContentType
    {
        FormUrlEncodedContent,
        MultipartContent,
        StreamContent,
        StringContent,
        JsonStringContent,
        ByteArrayContent,
        MultipartFormDataContent,
        ReadOnlyMemoryContent
    }
}