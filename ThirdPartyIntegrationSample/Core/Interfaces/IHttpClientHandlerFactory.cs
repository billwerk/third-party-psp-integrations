using System.Net.Http;

namespace Core.Interfaces
{
    public interface IHttpClientHandlerFactory
    {
        public HttpClientHandler Create();
    }
}