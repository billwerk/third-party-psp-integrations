using Business.Interfaces;
using Newtonsoft.Json;

namespace Business.Services
{
    public abstract class RecurringTokenEncoderBase<T> : IRecurringTokenEncoder<T>
    {
        private readonly ITetheredPaymentInformationEncoder _tetheredPaymentInformationEncoder;

        protected RecurringTokenEncoderBase(ITetheredPaymentInformationEncoder tetheredPaymentInformationEncoder)
        {
            _tetheredPaymentInformationEncoder = tetheredPaymentInformationEncoder;
        }

        public string Encrypt(T token)
        {
            return _tetheredPaymentInformationEncoder.Encrypt(JsonConvert.SerializeObject(token));
        }

        public T Decrypt(string base64String)
        {
            return JsonConvert.DeserializeObject<T>(_tetheredPaymentInformationEncoder.Decrypt(base64String));
        }
    }
}