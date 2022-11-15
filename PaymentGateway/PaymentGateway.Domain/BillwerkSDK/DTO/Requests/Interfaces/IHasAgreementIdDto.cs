namespace Billwerk.Payment.SDK.DTO.Requests
{
    /// <summary>
    /// Specify that dto model must include billwerk agreement id.
    /// </summary>
    public interface IHasAgreementIdDto
    {
        /// <summary>
        /// Id which specify agreement between 3-rd party integration and billwerk according payment actions,
        /// which initiated by billwerk for specific customer and supported by 3-rd party integration.
        /// Mandatory.
        /// </summary>
        string AgreementId { get; set; }
    }
}
