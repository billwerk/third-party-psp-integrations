using System.Net;
using System.Net.Http;
using Core.Interfaces;

namespace Core.Rest
{
    public class HttpClientHandlerFactory : IHttpClientHandlerFactory
    {
        private readonly IGlobalSettings _settings;

        public HttpClientHandlerFactory(IGlobalSettings settings)
        {
            _settings = settings;
        }

        public HttpClientHandler Create()
        {
            var clientHandler = new HttpClientHandler();
            if (_settings.UseProxy)
            {
                clientHandler.Proxy = new WebProxy(_settings.ProxyHost, _settings.ProxyPort)
                {
                    BypassProxyOnLocal = false
                };
            }

            return clientHandler;
        }
    }
}