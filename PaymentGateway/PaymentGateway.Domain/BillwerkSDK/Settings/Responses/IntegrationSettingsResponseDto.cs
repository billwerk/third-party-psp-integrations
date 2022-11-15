namespace Billwerk.Payment.SDK.DTO.IntegrationInfo.Responses
{
    /// <summary>
    /// Represent state of external settings (set of fields for checking is specific payment method setup valid or not).
    /// Chosen fields (flags which set with true) indicate that this payment method can be configure and used by billwerk user.
    /// </summary>
    public class IntegrationSettingsResponseDto
    {
        public bool IsCreditCardValid { get; set; }

        public bool IsDebitValid { get; set; }

        public bool IsOnAccountValid { get; set; }

        public bool IsBlackLabelValid { get; set; }

        public bool IsAutogiroValid { get; set; }

        public bool IsAvtalegiroValid { get; set; }

        public bool IsBetalingsserviceValid { get; set; }
    }
}