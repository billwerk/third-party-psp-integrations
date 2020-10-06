namespace Business.Interfaces
{
    public interface ITetheredPaymentInformationEncoder
    {
        public string Encrypt(string json);
        public string Decrypt(string base64String);
    }
}