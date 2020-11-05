using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Billwerk.Payment.SDK.Enums;
using Billwerk.Payment.SDK.Interfaces;
using Billwerk.Payment.SDK.Interfaces.Models;
using Billwerk.Payment.SDK.Models;
using Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace Core.Rest
{
    public class RestClient : IPspClient
    {
        private readonly HttpClient _httpClient;
        private readonly IEnumerable<IHttpContentFactory> _httpContentFactories;
        private readonly ILogger<RestClient> _logger;

        public RestClient(
            IEnumerable<IHttpContentFactory> httpContentFactories, 
            ILogger<RestClient> logger, IHttpClientHandlerFactory handlerFactory)
        {
            _httpClient = new HttpClient(handlerFactory.Create());
            _httpContentFactories = httpContentFactories;
            _logger = logger;
        }

        public async Task<PspResult> ExecuteAsync(string absoluteUriPath, 
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

            var result = new PspResult
            {
                StatusCode = response.StatusCode,
                Data = responseData
            };
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning(responseData);
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

        public async Task<IPspResponse> ExecuteRequestAsync(string absoluteUriPath, HttpMethod httpMethod, IPspRequest request, PspLoggingContext loggingContext,
            HttpContentType? httpContentType = null, int? timeoutInMilliseconds = null)
        {
            return await ExecuteAsync(absoluteUriPath, httpMethod, request, httpContentType);
        }
    }
}