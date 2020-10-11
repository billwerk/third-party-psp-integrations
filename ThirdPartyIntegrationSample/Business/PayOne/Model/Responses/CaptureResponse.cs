namespace Business.PayOne.Model.Responses
{
    public class CaptureResponse : ResponseBase
    {
        public CaptureResponse(string response) 
            : base(response)
        {
        }

        public string Mandate_Identification { get; set; }

        //YYYYMMDD
        public string Mandate_Dateofsignature { get; set; }

        public string Creditor_Identifier { get; set; }

        //YYYYMMDD
        public string Clearing_Date { get; set; }
    }
}