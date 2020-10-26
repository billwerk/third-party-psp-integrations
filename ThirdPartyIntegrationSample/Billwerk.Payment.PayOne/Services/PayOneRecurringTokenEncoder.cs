using Billwerk.Payment.SDK.Interfaces;

namespace Billwerk.Payment.PayOne.Services
{
    public class PayOneRecurringTokenEncoder : IRecurringTokenEncoder<IRecurringToken>
    {
        private readonly IJsonConvertService _jsonConvertService;
        private readonly ITetheredPaymentInformationEncoder _tetheredPaymentInformationEncoder;
        
        public PayOneRecurringTokenEncoder(ITetheredPaymentInformationEncoder tetheredPaymentInformationEncoder, IJsonConvertService jsonConvertService)
        {
            _jsonConvertService = jsonConvertService;
            _tetheredPaymentInformationEncoder = tetheredPaymentInformationEncoder;
        }
        
        public string Encrypt(IRecurringToken token)
        {
            return _tetheredPaymentInformationEncoder.Encrypt(_jsonConvertService.SerializeObject(token));
        }

        public IRecurringToken Decrypt(string base64String)
        {
            return _jsonConvertService.DeserializeObject<IRecurringToken>(_tetheredPaymentInformationEncoder.Decrypt(base64String));
        }        
    }
}