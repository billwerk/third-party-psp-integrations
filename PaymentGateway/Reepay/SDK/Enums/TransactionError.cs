// Copyright (c) billwerk GmbH. All rights reserved

using System.Runtime.Serialization;

namespace Reepay.SDK.Enums;

/// <summary>
/// Reepay transaction errors (https://reference.reepay.com/api/#transaction-errors)
/// </summary>
public enum TransactionError
{
    /// <summary>
    /// Credit card expired
    /// </summary>
    [EnumMember(Value = "credit_card_expired")]
    CreditCardExpired,

    /// <summary>
    /// Transaction declined by acquirer for some reason
    /// </summary>
    [EnumMember(Value = "declined_by_acquirer")]
    DeclinedByAcquirer,

    /// <summary>
    /// Credit card lost or stolen
    /// </summary>
    [EnumMember(Value = "credit_card_lost_or_stolen")]
    CreditCardLostOrStolen,

    /// <summary>
    /// Credit card suspected fraud
    /// </summary>
    [EnumMember(Value = "credit_card_suspected_fraud")]
    CreditCardSuspectedFraud,

    /// <summary>
    /// The tried refund amount is too high
    /// </summary>
    [EnumMember(Value = "refund_amount_too_high")]
    RefundAmountTooHigh,

    /// <summary>
    /// Settle failed because the authorization has expired
    /// </summary>
    [EnumMember(Value = "authorization_expired")]
    AuthorizationExpired,

    /// <summary>
    /// Settle failed because requested amount exceeded authorized amount
    /// </summary>
    [EnumMember(Value = "authorization_amount_exceeded")]
    AuthorizationAmountExceeded,

    /// <summary>
    /// Settle failed because authorization has been voided
    /// </summary>
    [EnumMember(Value = "authorization_voided")]
    AuthorizationVoided,

    /// <summary>
    /// Transaction declined by acquirer because strong customer authentication (e.g. 3D Secure) is required
    /// </summary>
    [EnumMember(Value = "sca_required")]
    ScaRequired,

    /// <summary>
    /// Transaction was declined by a Reepay Risk Filter rule
    /// </summary>
    [EnumMember(Value = "risk_filter_block")]
    RiskFilterBlock,

    /// <summary>
    /// Transaction was declined by Reepay Fraud Detector
    /// </summary>
    [EnumMember(Value = "fraud_block")]
    FraudBlock,

    /// <summary>
    /// Valid payment method but insufficient funds to complete transaction
    /// </summary>
    [EnumMember(Value = "insufficient_funds")]
    InsufficientFunds,

    /// <summary>
    /// Settle of authorization blocked by acquirer or payment gateway
    /// </summary>
    [EnumMember(Value = "settle_blocked")]
    SettleBlocked,

    /// <summary>
    /// Communication with acquirer failed
    /// </summary>
    [EnumMember(Value = "acquirer_communication_error")]
    AcquirerCommunicationError,

    /// <summary>
    /// Error at the acquirer or payment gateway
    /// </summary>
    [EnumMember(Value = "acquirer_error")]
    AcquirerError,

    /// <summary>
    /// There is an error in the integration to the acquirer
    /// </summary>
    [EnumMember(Value = "acquirer_integration_error")]
    AcquirerIntegrationError,

    /// <summary>
    /// Provided authentication credentials are not valid
    /// </summary>
    [EnumMember(Value = "acquirer_authentication_error")]
    AcquirerAuthenticationError,

    /// <summary>
    /// Error in the configuration of the acquirer or payment gateway account
    /// </summary>
    [EnumMember(Value = "acquirer_configuration_error")]
    AcquirerConfigurationError,

    /// <summary>
    /// Acquirer rejected this specific transaction. E.g. amount too low or too high
    /// </summary>
    [EnumMember(Value = "acquirer_rejected_error")]
    AcquirerRejectedError,
}
