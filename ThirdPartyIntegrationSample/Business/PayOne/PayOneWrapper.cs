using System.Net.Http;
using System.Threading.Tasks;
using Business.PayOne.Model.Requests;
using Core.Interfaces;
using Core.Rest;

namespace Business.PayOne
{
    public class PayOneWrapper : IPayOneWrapper
    {
        private const string UrlPayOneApi = "https://api.pay1.de/post-gateway/";
        private readonly IRestClient _restClient;

        public PayOneWrapper(IRestClient restClient)
        {
            _restClient = restClient;
        }
        
        public async Task<RestResult<string>> ExecutePayOneRequestAsync(RequestBase requestDto)
        {
            return await _restClient.ExecuteAsync(UrlPayOneApi, HttpMethod.Post, requestDto, HttpContentType.FormUrlEncodedContent);
        }
    }
}
