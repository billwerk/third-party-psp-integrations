using System;
using System.Net.Http;
using System.Threading.Tasks;
using Business.PayOne.Helpers;
using Business.PayOne.Interfaces;
using Business.PayOne.Model.Requests;
using Core.Interfaces;
using Core.Rest;

namespace Business.PayOne
{
    public class PayOneWrapper : IPayOneWrapper
    {
        private const string UrlPayOneApi = "https://api.pay1.de/post-gateway/";
        private readonly IRestClient _restClient;
        private readonly NvCodec _nvCodec;

        public PayOneWrapper(IRestClient restClient)
        {
            _restClient = restClient;
            _nvCodec = new NvCodec();
        }
        
        public async Task<RestResult<string>> ExecutePayOneRequestAsync(RequestBase requestDto)
        {
            try
            {
                requestDto.Encode(_nvCodec);

                return await _restClient.ExecuteAsync(UrlPayOneApi, HttpMethod.Post, _nvCodec,
                    HttpContentType.FormUrlEncodedContent);
            }
            catch (Exception ex)
            {
                return new RestResult<string>(ex.Message);
            }
        }
    }
}
