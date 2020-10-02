namespace Business.PayOne.Model.Responses
{
    public class PreauthorizationResponse : ResponseBase
    {
        public PreauthorizationResponse(string response) : base(response)
        {
        }

        public string UserId { get; set; }
        public string CustomerMessage { get; set; }
        public string ProtectResultAvs { get; set; }
        public string MandateIdentification { get; set; }
        //YYYYMMDD
        public string MandateDateofsignature { get; set; }
        public string CreditorIdentifier { get; set; }
    }
}