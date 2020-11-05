namespace Core.Rest
{
    public class Error
    {
        public string ProviderCode { get; set; }
        public string Message { get; set; }
        public Error(string code, string message)
        {
            ProviderCode = code;
            Message = message;
        }
        public Error(string message)
        {
            Message = message;
        }
    }
}