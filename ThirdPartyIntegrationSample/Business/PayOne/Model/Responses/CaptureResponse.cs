namespace Business.PayOne.Model.Responses
{
    public class CaptureResponse : ResponseBase
    {
        public CaptureResponse(string response) 
            : base(response)
        {
        }

        public string MandateIdentification { get; set; }
        //YYYYMMDD
        public string MandateDateofsignature { get; set; }
        public string CreditorIdentifier { get; set; }
        //YYYYMMDD
        public string ClearingDate { get; set; }
    }
}