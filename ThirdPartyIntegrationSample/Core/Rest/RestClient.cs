using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace Core.Rest
{
    public class RestClient : IRestClient
    {
        private readonly HttpClient _httpClient;
        private readonly IEnumerable<IHttpContentFactory> _httpContentFactories;
        private readonly ILogger<RestClient> _logger;

        public RestClient(
            HttpClient httpClient,
            IEnumerable<IHttpContentFactory> httpContentFactories, 
            ILogger<RestClient> logger)
        {
            _httpClient = httpClient;
            _httpContentFactories = httpContentFactories;
            _logger = logger;
        }

        public async Task<RestResult<string>> ExecuteAsync(string absoluteUriPath, 
            HttpMethod httpMethod, 
            object entity = null, 
            HttpContentType? httpContentType = null)
        {
            _logger.Log(LogLevel.Debug, $"Full url: '{absoluteUriPath}'");

            var request = new HttpRequestMessage
            {
                Method = httpMethod,
                RequestUri = new Uri(absoluteUriPath)
            };
            
            AddContentIfRequired(entity, httpContentType, request);

            var response = await _httpClient.SendAsync(request);
            var responseData = string.Empty;
            if (response.Content != null)
            {
                responseData = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            }

            RestResult<string> result;
            if (!response.IsSuccessStatusCode)
            {
                result = new RestResult<string>(responseData)
                {
                    StatusCode = response.StatusCode
                };
                
                _logger.LogWarning(responseData);
            }
            else
            {
                result = new RestResult<string>
                {
                    StatusCode = response.StatusCode,
                    Data = responseData
                };
            }

            return result;
        }

        private void AddContentIfRequired(object entity, HttpContentType? httpContentType, HttpRequestMessage request)
        {
            if (entity == null || httpContentType == null) 
                return;
            
            var httpContentFactory = _httpContentFactories.FirstOrDefault(f => f.Type == httpContentType);
            if (httpContentFactory == null)
            {
                throw new NotSupportedException($"HttpContentFactory '{httpContentType}' not supported");
            }

            request.Content = httpContentFactory.Create(entity);
        }
    }
}