using System.Text;
using Business.Interfaces;
using Core.Helpers;
using Core.Interfaces;

namespace Business.Services
{
    public class TetheredPaymentInformationEncoder : ITetheredPaymentInformationEncoder
    {
        private readonly IEncoder _encoder;
        private static Encoding Encoding => Encoding.GetEncoding("utf-8");

        public TetheredPaymentInformationEncoder(IEncoder encoder)
        {
            _encoder = encoder;
        }

        public string Encrypt(string json)
        {
            return UrlSafeBase64.Encode(_encoder.Encrypt(Encoding.GetBytes(json)));
        }

        public string Decrypt(string base64String)
        {
            return Encoding.GetString(_encoder.Decrypt(UrlSafeBase64.Decode(base64String)));
        }
    }
}