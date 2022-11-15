// Copyright (c) billwerk GmbH. All rights reserved

namespace Reepay.SDK.Enums;

///<summary>
/// Reepay error codes (https://reference.reepay.com/api/#errors)
/// Generated from https://api.reepay.com/v1/error_codes
///</summary>
public enum ReepayErrorCode
{
    ///<summary>
    ///Ok (http 200)
    ///</summary>
    Ok = 0,

    ///<summary>
    ///Invalid request (http 400)
    ///</summary>
    InvalidRequest = 1,

    ///<summary>
    ///Internal error (http 500)
    ///</summary>
    InternalError = 2,

    ///<summary>
    ///Invalid user or password (http 400)
    ///</summary>
    InvalidUserOrPassword = 3,

    ///<summary>
    ///No accounts for user (http 400)
    ///</summary>
    NoAccountsForUser = 4,

    ///<summary>
    ///Unknown account (http 404)
    ///</summary>
    UnknownAccount = 5,

    ///<summary>
    ///Not authenticated (http 401)
    ///</summary>
    NotAuthenticated = 6,

    ///<summary>
    ///Unauthorized (http 403)
    ///</summary>
    Unauthorized = 7,

    ///<summary>
    ///Not found (http 404)
    ///</summary>
    NotFound = 8,

    ///<summary>
    ///Customer not found (http 404)
    ///</summary>
    CustomerNotFound = 9,

    ///<summary>
    ///Subscription plan not found (http 404)
    ///</summary>
    SubscriptionPlanNotFound = 10,

    ///<summary>
    ///Duplicate handle (http 400)
    ///</summary>
    DuplicateHandle = 11,

    ///<summary>
    ///Subscription not found (http 404)
    ///</summary>
    SubscriptionNotFound = 12,

    ///<summary>
    ///Subscription expired (http 400)
    ///</summary>
    SubscriptionExpired = 13,

    ///<summary>
    ///Must be in future (http 400)
    ///</summary>
    MustBeInFuture = 14,

    ///<summary>
    ///Account not found (http 404)
    ///</summary>
    AccountNotFound = 15,

    ///<summary>
    ///User not found (http 404)
    ///</summary>
    UserNotFound = 17,

    ///<summary>
    ///Missing customer (http 400)
    ///</summary>
    MissingCustomer = 18,

    ///<summary>
    ///Card not found (http 404)
    ///</summary>
    CardNotFound = 19,

    ///<summary>
    ///Test data not allowed for live account (http 400)
    ///</summary>
    TestDataNotAllowedForLiveAccount = 20,

    ///<summary>
    ///Live data not allowed for test account (http 400)
    ///</summary>
    LiveDataNotAllowedForTestAccount = 21,

    ///<summary>
    ///Subscription cancelled (http 400)
    ///</summary>
    SubscriptionCancelled = 22,

    ///<summary>
    ///From date after to date (http 400)
    ///</summary>
    FromDateAfterToDate = 23,

    ///<summary>
    ///Missing amount (http 400)
    ///</summary>
    MissingAmount = 24,

    ///<summary>
    ///Additional cost not pending (http 400)
    ///</summary>
    AdditionalCostNotPending = 25,

    ///<summary>
    ///Additional cost not found (http 404)
    ///</summary>
    AdditionalCostNotFound = 26,

    ///<summary>
    ///Credit not found (http 404)
    ///</summary>
    CreditNotFound = 27,

    ///<summary>
    ///Credit not pending (http 400)
    ///</summary>
    CreditNotPending = 28,

    ///<summary>
    ///Invoice already cancelled (http 400)
    ///</summary>
    InvoiceAlreadyCancelled = 29,

    ///<summary>
    ///Invoice has active transactions (http 400)
    ///</summary>
    InvoiceHasActiveTransactions = 30,

    ///<summary>
    ///Invoice not found (http 404)
    ///</summary>
    InvoiceNotFound = 31,

    ///<summary>
    ///Customer has non expired subscriptions (http 400)
    ///</summary>
    CustomerHasNonExpiredSubscriptions = 32,

    ///<summary>
    ///Customer has pending invoices (http 400)
    ///</summary>
    CustomerHasPendingInvoices = 33,

    ///<summary>
    ///Invalid card token (http 400)
    ///</summary>
    InvalidCardToken = 34,

    ///<summary>
    ///Missing card (http 400)
    ///</summary>
    MissingCard = 35,

    ///<summary>
    ///Missing card token (http 400)
    ///</summary>
    MissingCardToken = 36,

    ///<summary>
    ///Start date cannot be more than one period away in the past (http 400)
    ///</summary>
    StartDateCannotBeMoreThanOnePeriodAwayInThePast = 37,

    ///<summary>
    ///Card not allowed for signup method (http 400)
    ///</summary>
    CardNotAllowedForSignupMethod = 38,

    ///<summary>
    ///Card token not allowed for signup method (http 400)
    ///</summary>
    CardTokenNotAllowedForSignupMethod = 39,

    ///<summary>
    ///Payment method not found (http 404)
    ///</summary>
    PaymentMethodNotFound = 40,

    ///<summary>
    ///Payment method not inactive (http 400)
    ///</summary>
    PaymentMethodNotInactive = 41,

    ///<summary>
    ///Payment method not active (http 400)
    ///</summary>
    PaymentMethodNotActive = 42,

    ///<summary>
    ///Not implemented (http 501)
    ///</summary>
    NotImplemented = 43,

    ///<summary>
    ///Dunning plan not found (http 404)
    ///</summary>
    DunningPlanNotFound = 44,

    ///<summary>
    ///Organisation not found (http 404)
    ///</summary>
    OrganisationNotFound = 45,

    ///<summary>
    ///Webhook not found (http 404)
    ///</summary>
    WebhookNotFound = 46,

    ///<summary>
    ///Event not found (http 404)
    ///</summary>
    EventNotFound = 47,

    ///<summary>
    ///Dunning plan in use (http 400)
    ///</summary>
    DunningPlanInUse = 48,

    ///<summary>
    ///Last dunning plan (http 400)
    ///</summary>
    LastDunningPlan = 49,

    ///<summary>
    ///Search error (http 400)
    ///</summary>
    SearchError = 50,

    ///<summary>
    ///Private key not found (http 404)
    ///</summary>
    PrivateKeyNotFound = 51,

    ///<summary>
    ///Public key not found (http 404)
    ///</summary>
    PublicKeyNotFound = 52,

    ///<summary>
    ///Mail not found (http 404)
    ///</summary>
    MailNotFound = 53,

    ///<summary>
    ///No order lines for invoice (http 400)
    ///</summary>
    NoOrderLinesForInvoice = 54,

    ///<summary>
    ///Agreement not found (http 404)
    ///</summary>
    AgreementNotFound = 55,

    ///<summary>
    ///Multiple agreements (http 400)
    ///</summary>
    MultipleAgreements = 56,

    ///<summary>
    ///Duplicate email (http 400)
    ///</summary>
    DuplicateEmail = 57,

    ///<summary>
    ///Invalid group (http 400)
    ///</summary>
    InvalidGroup = 58,

    ///<summary>
    ///User blocked due to failed logins (http 400)
    ///</summary>
    UserBlockedDueToFailedLogins = 59,

    ///<summary>
    ///Invalid template (http 400)
    ///</summary>
    InvalidTemplate = 60,

    ///<summary>
    ///Mail type not found (http 404)
    ///</summary>
    MailTypeNotFound = 61,

    ///<summary>
    ///Card gateway state live/test must match account state live/test (http 400)
    ///</summary>
    CardGatewayStateLiveTestMustMatchAccountStateLiveTest = 62,

    ///<summary>
    ///Subscription has pending or dunning invoices (http 400)
    ///</summary>
    SubscriptionHasPendingOrDunningInvoices = 63,

    ///<summary>
    ///Invoice not settled (http 400)
    ///</summary>
    InvoiceNotSettled = 64,

    ///<summary>
    ///Refund amount too high (http 400)
    ///</summary>
    RefundAmountTooHigh = 65,

    ///<summary>
    ///Refund failed (http 500)
    ///</summary>
    RefundFailed = 66,

    ///<summary>
    ///The subdomain is reserved (http 400)
    ///</summary>
    TheSubdomainIsReserved = 67,

    ///<summary>
    ///User email already verified (http 400)
    ///</summary>
    UserEmailAlreadyVerified = 68,

    ///<summary>
    ///Go live not allowed (http 400)
    ///</summary>
    GoLiveNotAllowed = 69,

    ///<summary>
    ///Transaction not found (http 404)
    ///</summary>
    TransactionNotFound = 70,

    ///<summary>
    ///Customer has been deleted (http 404)
    ///</summary>
    CustomerHasBeenDeleted = 71,

    ///<summary>
    ///Currency change not allowed (http 400)
    ///</summary>
    CurrencyChangeNotAllowed = 72,

    ///<summary>
    ///Invalid reminder emails days (http 400)
    ///</summary>
    InvalidReminderEmailsDays = 73,

    ///<summary>
    ///Concurrent resource update (http 400)
    ///</summary>
    ConcurrentResourceUpdate = 74,

    ///<summary>
    ///Subscription not eligible for invoice (http 400)
    ///</summary>
    SubscriptionNotEligibleForInvoice = 75,

    ///<summary>
    ///Payment method not provided (http 400)
    ///</summary>
    PaymentMethodNotProvided = 76,

    ///<summary>
    ///Transaction declined (http 400)
    ///</summary>
    TransactionDeclined = 77,

    ///<summary>
    ///Transaction processing error (http 500)
    ///</summary>
    TransactionProcessingError = 78,

    ///<summary>
    ///Invoice already settled (http 400)
    ///</summary>
    InvoiceAlreadySettled = 79,

    ///<summary>
    ///Invoice has processing transaction (http 400)
    ///</summary>
    InvoiceHasProcessingTransaction = 80,

    ///<summary>
    ///Online refund not supported, use manual refund (http 400)
    ///</summary>
    OnlineRefundNotSupported = 81,

    ///<summary>
    ///Invoice wrong state (http 400)
    ///</summary>
    InvoiceWrongState = 82,

    ///<summary>
    ///Discount not found (http 404)
    ///</summary>
    DiscountNotFound = 83,

    ///<summary>
    ///Subscription discount not found (http 404)
    ///</summary>
    SubscriptionDiscountNotFound = 84,

    ///<summary>
    ///Multiple discounts not allowed (http 400)
    ///</summary>
    MultipleDiscountsNotAllowed = 85,

    ///<summary>
    ///Coupon not found or not eligible (http 404)
    ///</summary>
    CouponNotFoundOrNotEligible = 86,

    ///<summary>
    ///Coupon already used (http 400)
    ///</summary>
    CouponAlreadyUsed = 87,

    ///<summary>
    ///Coupon code already exists (http 400)
    ///</summary>
    CouponCodeAlreadyExists = 88,

    ///<summary>
    ///Used coupon cannot be deleted (http 400)
    ///</summary>
    UsedCouponCannotBeDeleted = 89,

    ///<summary>
    ///Coupon not active (http 400)
    ///</summary>
    CouponNotActive = 90,

    ///<summary>
    ///Coupon cannot be updated (http 400)
    ///</summary>
    CouponCannotBeUpdated = 91,

    ///<summary>
    ///Cannot expire in current period (http 400)
    ///</summary>
    CannotExpireInCurrentPeriod = 93,

    ///<summary>
    ///Cannot uncancel in partial period (http 400)
    ///</summary>
    CannotUncancelInPartialPeriod = 94,

    ///<summary>
    ///Subscription on hold (http 400)
    ///</summary>
    SubscriptionOnHold = 95,

    ///<summary>
    ///Subscription in trial (http 400)
    ///</summary>
    SubscriptionInTrial = 96,

    ///<summary>
    ///Subscription not on hold (http 400)
    ///</summary>
    SubscriptionNotOnHold = 97,

    ///<summary>
    ///Invalid setup token (http 400)
    ///</summary>
    InvalidSetupToken = 98,

    ///<summary>
    ///Customer cannot be changed on invoice (http 400)
    ///</summary>
    CustomerCannotBeChangedOnInvoice = 99,

    ///<summary>
    ///Amount change not allowed on invoice (http 400)
    ///</summary>
    AmountChangeNotAllowedOnInvoice = 100,

    ///<summary>
    ///Request does not belong to invoice (http 400)
    ///</summary>
    RequestDoesNotBelongToInvoice = 101,

    ///<summary>
    ///Amount higher than authorized amount (http 400)
    ///</summary>
    AmountHigherThanAuthorizedAmount = 102,

    ///<summary>
    ///Card token already used (http 400)
    ///</summary>
    CardTokenAlreadyUsed = 103,

    ///<summary>
    ///Card token expired (http 400)
    ///</summary>
    CardTokenExpired = 104,

    ///<summary>
    ///Invoice already authorized (http 400)
    ///</summary>
    InvoiceAlreadyAuthorized = 105,

    ///<summary>
    ///Invoice must be authorized (http 400)
    ///</summary>
    InvoiceMustBeAuthorized = 106,

    ///<summary>
    ///Refund not found (http 404)
    ///</summary>
    RefundNotFound = 107,

    ///<summary>
    ///Transaction cancel failed (http 500)
    ///</summary>
    TransactionCancelFailed = 108,

    ///<summary>
    ///Transaction wrong state for operation (http 400)
    ///</summary>
    TransactionWrongStateForOperation = 109,

    ///<summary>
    ///Unknown or missing source (http 400)
    ///</summary>
    UnknownOrMissingSource = 110,

    ///<summary>
    ///Source not allowed for signup method (http 400)
    ///</summary>
    SourceNotAllowedForSignupMethod = 111,

    ///<summary>
    ///Invoice wrong type (http 400)
    ///</summary>
    InvoiceWrongType = 112,

    ///<summary>
    ///Add-on not found (http 404)
    ///</summary>
    AddOnNotFound = 113,

    ///<summary>
    ///Add-on already added to subscription (http 400)
    ///</summary>
    AddOnAlreadyAddedToSubscription = 114,

    ///<summary>
    ///Add-on quantity not allowed for on-off add-on type (http 400)
    ///</summary>
    AddOnQuantityNotAllowedForOnOffAddOnType = 115,

    ///<summary>
    ///Add-on not eligible for subscription plan (http 400)
    ///</summary>
    AddOnNotEligibleForSubscriptionPlan = 116,

    ///<summary>
    ///Subscription add-on not found (http 404)
    ///</summary>
    SubscriptionAddOnNotFound = 117,

    ///<summary>
    ///Subscription pending (http 400)
    ///</summary>
    SubscriptionPending = 118,

    ///<summary>
    ///Subscription must be pending (http 400)
    ///</summary>
    SubscriptionMustBePending = 119,

    ///<summary>
    ///Credit amount too high (http 400)
    ///</summary>
    CreditAmountTooHigh = 120,

    ///<summary>
    ///Discount is deleted (http 404)
    ///</summary>
    DiscountIsDeleted = 121,

    ///<summary>
    ///Request rate limit exceeded (http 429)
    ///</summary>
    RequestRateLimitExceeded = 122,

    ///<summary>
    ///Concurrent request limit exceeded (http 429)
    ///</summary>
    ConcurrentRequestLimitExceeded = 123,

    ///<summary>
    ///Payment method in use (http 400)
    ///</summary>
    PaymentMethodInUse = 124,

    ///<summary>
    ///Subscription has pending payment method (http 400)
    ///</summary>
    SubscriptionHasPendingPaymentMethod = 125,

    ///<summary>
    ///Payment method not pending (http 400)
    ///</summary>
    PaymentMethodNotPending = 127,

    ///<summary>
    ///Payment method pending (http 400)
    ///</summary>
    PaymentMethodPending = 128,

    ///<summary>
    ///Multiple settles not allowed for payment method (http 400)
    ///</summary>
    MultipleSettlesNotAllowedForPaymentMethod = 129,

    ///<summary>
    ///Partial settle not allowed for payment method (http 400)
    ///</summary>
    PartialSettleNotAllowedForPaymentMethod = 130,

    ///<summary>
    ///Multiple refunds not allowed for payment method (http 400)
    ///</summary>
    MultipleRefundsNotAllowedForPaymentMethod = 131,

    ///<summary>
    ///Partial refund not allowed for payment method (http 400)
    ///</summary>
    PartialRefundNotAllowedForPaymentMethod = 132,

    ///<summary>
    ///Payout processing (http 400)
    ///</summary>
    PayoutProcessing = 133,

    ///<summary>
    ///Payout already paid (http 400)
    ///</summary>
    PayoutAlreadyPaid = 134,

    ///<summary>
    ///Payment method not allowed for payout (http 400)
    ///</summary>
    PaymentMethodNotAllowedForPayout = 135,

    ///<summary>
    ///Customer cannot be changed on payout (http 400)
    ///</summary>
    CustomerCannotBeChangedOnPayout = 136,

    ///<summary>
    ///Payout not found (http 404)
    ///</summary>
    PayoutNotFound = 137,

    ///<summary>
    ///No suitable card verification agreement found (http 400)
    ///</summary>
    NoSuitableCardVerificationAgreementFound = 138,

    ///<summary>
    ///Currency not supported by payment method (http 400)
    ///</summary>
    CurrencyNotSupportedByPaymentMethod = 139,

    ///<summary>
    ///Source type must be reusable (http 400)
    ///</summary>
    SourceTypeMustBeReusable = 140,

    ///<summary>
    ///Too many settle attempts (http 400)
    ///</summary>
    TooManySettleAttempts = 141,

    ///<summary>
    ///Invalid MFA verification code (http 400)
    ///</summary>
    InvalidMfaVerificationCode = 142,

    ///<summary>
    ///MFA authentication required (http 400)
    ///</summary>
    MfaAuthenticationRequired = 143,

    ///<summary>
    ///Query took too long, adjust time range (http 400)
    ///</summary>
    QueryTookTooLong = 144,

    ///<summary>
    ///Invoice has zero amount (http 400)
    ///</summary>
    InvoiceHasZeroAmount = 145,

    ///<summary>
    ///Non positive amount (http 422)
    ///</summary>
    NonPositiveAmount = 146,
}
