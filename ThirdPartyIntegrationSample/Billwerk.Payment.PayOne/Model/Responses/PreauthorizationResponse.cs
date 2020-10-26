﻿namespace Billwerk.Payment.PayOne.Model.Responses
{
    public class PreauthorizationResponse : ResponseBase
    {
        public PreauthorizationResponse(string response)
            : base(response)
        {
        }

        public string UserId { get; set; }
        public string CustomerMessage { get; set; }
        public string Protect_Result_AVS { get; set; }

        public string Mandate_Identification { get; set; }

        //YYYYMMDD
        public string Mandate_Dateofsignature { get; set; }
        public string Creditor_Identifier { get; set; }
    }
}