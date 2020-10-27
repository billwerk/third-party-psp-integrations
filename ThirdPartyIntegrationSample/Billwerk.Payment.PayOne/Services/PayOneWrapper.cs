using System;
using System.Net.Http;
using System.Threading.Tasks;
using Billwerk.Payment.PayOne.Helpers;
using Billwerk.Payment.PayOne.Interfaces;
using Billwerk.Payment.PayOne.Model.Requests;
using Billwerk.Payment.SDK.Interfaces;
using Billwerk.Payment.SDK.Rest;

namespace Billwerk.Payment.PayOne
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
