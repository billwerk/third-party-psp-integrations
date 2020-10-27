using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using Billwerk.Payment.PayOne.Helpers;
using Billwerk.Payment.PayOne.Interfaces;
using Billwerk.Payment.PayOne.Model;
using Billwerk.Payment.PayOne.Model.Requests;
using Billwerk.Payment.PayOne.Model.Responses;
using Billwerk.Payment.PayOne.Models;
using Billwerk.Payment.SDK.DTO;
using Billwerk.Payment.SDK.DTO.ExternalIntegration;
using Billwerk.Payment.SDK.DTO.ExternalIntegration.Cancellation;
using Billwerk.Payment.SDK.DTO.ExternalIntegration.Payment;
using Billwerk.Payment.SDK.DTO.ExternalIntegration.Preauth;
using Billwerk.Payment.SDK.DTO.ExternalIntegration.Refund;
using Billwerk.Payment.SDK.Enums;
using Billwerk.Payment.SDK.Interfaces;
using Billwerk.Payment.SDK.Services;
using Microsoft.Extensions.Logging;

namespace Billwerk.Payment.PayOne.Services
{
    public class PayOnePaymentService : ExternalPspServiceBase, IPaymentService
    {
        private const string PayOneDateFormat = "yyyyMMdd";

        private readonly ILogger<PayOnePaymentService> _logger;
        private readonly IPayOneWrapper _payOneWrapper;
        private readonly IPayOneInitialTokenDecoder _initialTokenDecoder;
        

        public PayOnePaymentService(ILogger<PayOnePaymentService> logger, IPayOneWrapper payOneWrapper, IPayOneInitialTokenDecoder initialTokenDecoder)
        {
            _logger = logger;
            _payOneWrapper = payOneWrapper;
            _initialTokenDecoder = initialTokenDecoder;
        }

        public async Task<ExternalPaymentResponseWrapperDTO> SendPayment(ExternalPaymentRequestWrapperDTO paymentRequest, IRecurringToken token)
        {
            var isCapture =
                !string.IsNullOrWhiteSpace(paymentRequest.PaymentRequestDto.PaymentMeansReference.PreauthTransactionId) &&
                paymentRequest.PreauthRequestDto != null;
            if (isCapture)
            {
                return await ProcessCaptureTransactionAsync(paymentRequest);
            }

            return await ProcessTransaction(paymentRequest, token);
        }

        public async Task<ExternalRefundTransactionDTO> SendRefund(ExternalRefundRequestDTO dto, IPaymentTransaction targetTransaction)
        {
            var settings = GetPspSettings<PayOnePSPSettings>(dto.MerchantSettings);
            var sequenceNumber = targetTransaction.SequenceNumber + 1;
            var refundReference = sequenceNumber.ToString(CultureInfo.InstalledUICulture);
            //Todo is Initial Payment
            var request = new Refund(true, settings)
            {
                Amount = ((int)( - dto.RequestedAmount * 100)).ToString(CultureInfo.InvariantCulture),
                Currency = dto.Currency,
                Narrative_Text = refundReference,
                SequenceNumber = sequenceNumber.ToString(CultureInfo.InvariantCulture),
                TxId = targetTransaction.PspTransactionId,
                Transaction_Param = targetTransaction.InvoiceReferenceCode
            };

            var restResult = await _payOneWrapper.ExecutePayOneRequestAsync(request);
            var response = new RefundResponse(restResult.Data);

            _logger.LogInformation(
                $"Processed PayOne refund {response.TxId} for payment transaction {dto.TransactionId}. Status: {response.Status}");

            return BuildAndPopulateExternalRefundTransactionDto(dto, response);
        }

        public async Task<ExternalPaymentResponseWrapperDTO> SendPreauth(ExternalPreauthRequestDTO dto, IRecurringToken token)
        {
            var payer = dto.PayerData;
            var initialPayment = !string.IsNullOrWhiteSpace(dto.PaymentMeansReference.InitialToken);
            var settings = GetPspSettings<PayOnePSPSettings>(dto.MerchantSettings);
            PayOnePspBearer tetheredPaymentInformation;
            string userId = null;
            IPayOneRecurringToken recurringToken = null;
            if (initialPayment)
            {
                tetheredPaymentInformation = _initialTokenDecoder.Decode(dto.PaymentMeansReference.InitialToken) as PayOnePspBearer;
            }
            else
            {
                recurringToken = token as IPayOneRecurringToken;
                tetheredPaymentInformation = recurringToken.PspBearer;
                userId = recurringToken.UserId;
            }
            
            var role = dto.PaymentMeansReference.Role;
            var request = new Preauthorization(initialPayment, settings)
            {
                Amount = ((int) (dto.RequestedAmount * 100)).ToString(CultureInfo.InvariantCulture),
                Currency = dto.Currency,
                ClearingType = GetClearingType(role),
                Narrative_Text = role == PaymentProviderRole.OnAccount
                    ? dto.TransactionInvoiceReferenceText
                    : dto.TransactionReferenceText,
                Reference = BuildReference(dto.TransactionId),
                Param = dto.TransactionId,
                PaymentBearer = GetPaymentBearer(tetheredPaymentInformation, role),
                Customer = GetPayOneCustomer(payer, userId),
                SuccessUrl = dto.PaymentMeansReference.SuccessReturnUrl,
                ErrorUrl = dto.PaymentMeansReference.ErrorReturnUrl,
                BackUrl = dto.PaymentMeansReference.AbortReturnUrl,
                ECommerceMode = initialPayment ? null : "internet"
            };

            var restResult = await _payOneWrapper.ExecutePayOneRequestAsync(request);
            var response = new PreauthorizationResponse(restResult.Data);

            _logger.LogInformation(
                $"Processed PayOne Preapproval {response.TxId} for payment transaction {dto.TransactionId}. Status: {response.Status}");

            return BuildAndPopulateExternalPreauthTransactionDto(dto, response, tetheredPaymentInformation, recurringToken);
        }

        public async Task<ExternalPaymentCancellationDTO> SendCancellation(ExternalPreauthRequestDTO dto, IPreauthTransaction targetTransaction)
        {
            var paymentRequest = new ExternalPaymentRequestWrapperDTO
            {
                PreauthRequestDto = dto,
                PspTransactionId = targetTransaction.PspTransactionId 
            };

            var result = await ProcessCaptureTransactionAsync(paymentRequest);
            if (result.PaymentDto.Status == PaymentTransactionNewStatus.Succeeded)
            {
                return new ExternalPaymentCancellationDTO
                {
                    CancellationStatus = result.PaymentDto.Status.ToString(),
                    TransactionId = result.PaymentDto.TransactionId
                };
            }

            return new ExternalPaymentCancellationDTO
            {
                Error = result.PaymentDto.Error
            };
        }

        public Task<ExternalPaymentTransactionDTO> FetchPayment(string transactionId)
        {
            throw new NotImplementedException();
        }

        public Task<ExternalRefundTransactionDTO> FetchRefund(string transactionId)
        {
            throw new NotImplementedException();
        }

        public Task<ExternalPreauthTransactionDTO> FetchPreauth(string transactionId)
        {
            throw new NotImplementedException();
        }

        private ExternalPaymentResponseWrapperDTO BuildAndPopulateExternalPreauthTransactionDto(ExternalPreauthRequestDTO dto,
            PreauthorizationResponse response, PayOnePspBearer tetheredPaymentInformation, IPayOneRecurringToken recurringToken)
        {
            var role = dto.PaymentMeansReference.Role;
            var preauth = BuildAndPopulateExternalTransactionBaseDto<ExternalPreauthTransactionDTO>(dto, response.TxId);
            var result = new ExternalPaymentResponseWrapperDTO(preauth, null);

            preauth.AuthorizedAmount = dto.RequestedAmount;
            //ToDo: Should be clarified
            preauth.ExpiresAt = DateTimeOffset.UtcNow.AddYears(1);
            preauth.Bearer = GetPaymentBearerDto(tetheredPaymentInformation, role);

            PopulateTransactionStatus(response, preauth);

            if (!string.IsNullOrEmpty(response.RedirectUrl))
            {
                preauth.RedirectUrl = response.RedirectUrl;
            }

            if (preauth.Status == PaymentTransactionNewStatus.Succeeded && role == PaymentProviderRole.OnAccount)
            {
                preauth.Status = PaymentTransactionNewStatus.Pending;
            }

            if (preauth.Status == PaymentTransactionNewStatus.Failed) return result;

            UpdateMandateIfRequired(preauth.Bearer, role, response.Mandate_Identification, response.Mandate_Dateofsignature, response.Creditor_Identifier);

            result.RecurringToken=PopulateRecurringToken(response, tetheredPaymentInformation, recurringToken, preauth);

            return result;
        }

        private ExternalRefundTransactionDTO BuildAndPopulateExternalRefundTransactionDto(ExternalRefundRequestDTO dto,
            RefundResponse response)
        {
            var result = BuildAndPopulateExternalTransactionBaseDto<ExternalRefundTransactionDTO>(dto, response.TxId);
            PopulateTransactionStatus(response, result);

            return result;
        }

        private IPayOneRecurringToken PopulateRecurringToken(PreauthorizationResponse response, PayOnePspBearer tetheredPaymentInformation,
            IPayOneRecurringToken recurringToken, ExternalPreauthTransactionDTO result)
        {
            recurringToken ??= new PayOneRecurringToken();
            recurringToken.UserId = response.UserId;
            recurringToken.PaymentBearer = result.Bearer;
            recurringToken.PspBearer = tetheredPaymentInformation;
            return recurringToken;
        }

        private static PaymentBearerDTO GetPaymentBearerDto(PayOnePspBearer payOnePspBearer, PaymentProviderRole role)
        {
            switch (role)
            {
                case PaymentProviderRole.CreditCard:
                    return new PaymentBearerCreditCardDTO
                    {
                        Holder = payOnePspBearer.Holder,
                        CardType = ConvertToLongCardType(payOnePspBearer.CardType),
                        Last4 = payOnePspBearer.TruncatedCardPan != null
                            ? payOnePspBearer.TruncatedCardPan.Substring(payOnePspBearer.TruncatedCardPan.Length - 4)
                            : "****",
                        MaskedCardPan = payOnePspBearer.TruncatedCardPan,
                        ExpiryMonth = payOnePspBearer.ExpiryMonth,
                        ExpiryYear = payOnePspBearer.ExpiryYear
                    };

                case PaymentProviderRole.Debit:
                {
                    var bearerBankAccount = new PaymentBearerBankAccountDTO();
                    if (string.IsNullOrWhiteSpace(payOnePspBearer.Iban) == false)
                    {
                        bearerBankAccount.Country = payOnePspBearer.Iban.Substring(0, 2).ToUpperInvariant();
                        bearerBankAccount.BIC = payOnePspBearer.Bic;
                        bearerBankAccount.IBAN = payOnePspBearer.Iban;
                    }
                    else
                    {
                        bearerBankAccount.Account = payOnePspBearer.Account;
                        bearerBankAccount.Country = payOnePspBearer.Country;
                        bearerBankAccount.Code = payOnePspBearer.Code;
                    }

                    bearerBankAccount.Holder = payOnePspBearer.Holder;
                    bearerBankAccount.CreditorId = payOnePspBearer.CreditorId;
                    bearerBankAccount.MandateReference = payOnePspBearer.MandateReference;
                    bearerBankAccount.MandateSignatureDate = payOnePspBearer.MandateSignatureDate;
                    bearerBankAccount.MandateText = payOnePspBearer.MandateText;

                    return bearerBankAccount;
                }
                case PaymentProviderRole.Default:
                case PaymentProviderRole.BlackLabel:
                case PaymentProviderRole.None:
                case PaymentProviderRole.OnAccount:
                default:
                    return null;
            }
        }

        private static void UpdateMandateIfRequired(PaymentBearerDTO bearer, PaymentProviderRole role,
            string mandateIdentification, string mandateDateofsignature, string creditorIdentifier)
        {
            if (role != PaymentProviderRole.Debit)
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(mandateIdentification) == false)
            {
                ((PaymentBearerBankAccountDTO) bearer).MandateReference = mandateIdentification;
            }

            if (string.IsNullOrWhiteSpace(mandateDateofsignature) == false)
            {
                ((PaymentBearerBankAccountDTO) bearer).MandateSignatureDate = ParsePayOneDate(mandateDateofsignature);
            }
            
            if (string.IsNullOrWhiteSpace(creditorIdentifier) == false)
            {
                ((PaymentBearerBankAccountDTO) bearer).CreditorId = creditorIdentifier;
            }
        }

        private static DateTime ParsePayOneDate(string stringDate)
        {
            return DateTime.ParseExact(stringDate, PayOneDateFormat, CultureInfo.InvariantCulture);
        }

        private static string GetClearingType(PaymentProviderRole role)
        {
            switch (role)
            {
                case PaymentProviderRole.CreditCard:
                    return "cc";
                case PaymentProviderRole.Debit:
                    return "elv";
                case PaymentProviderRole.OnAccount:
                    return "rec";
                default:
                    throw new NotSupportedException($"Role {role} not supported for PayOne payments");
            }
        }

        private static PaymentBearer GetPaymentBearer(PayOnePspBearer payOnePspBearer, PaymentProviderRole role)
        {
            return role switch
            {
                PaymentProviderRole.CreditCard => new PseudoCreditCard
                {
                    PseudoCardPan = payOnePspBearer.PseudoCardPan,
                    ExpiryMonth = payOnePspBearer.ExpiryMonth > 0 ? payOnePspBearer.ExpiryMonth.ToString() : null,
                    ExpiryYear = payOnePspBearer.ExpiryYear > 0 ? payOnePspBearer.ExpiryYear.ToString() : null,
                    CardType = payOnePspBearer.CardType
                },
                PaymentProviderRole.Debit => new DirectDebit
                {
                    BankAccount = payOnePspBearer.Account,
                    BankAccountHolder = payOnePspBearer.Holder,
                    BankCode = payOnePspBearer.Code,
                    BankCountry = payOnePspBearer.Country,
                    IBAN = payOnePspBearer.Iban,
                    BIC = payOnePspBearer.Bic,
                    Mandate_Identification = payOnePspBearer.MandateReference
                },
                PaymentProviderRole.OnAccount => new Invoice(),
                _ => throw new NotSupportedException($"Role={role} is not supported!")
            };
        }

        private static Customer GetPayOneCustomer(PayerDataDTO payer, string pspUserId)
        {
            var poCustomer = new Customer
            {
                FirstName = payer.FirstName,
                LastName = payer.LastName,
                Company = payer.CompanyName,
                UserId = pspUserId
            };

            var address = payer.Address;

            if (address == null || string.IsNullOrWhiteSpace(address.Country))
            {
                // TODO: Default should be the users country if customer country is null
                poCustomer.Country = "DE";
            }
            else
            {
                poCustomer.Country = address.Country;
                poCustomer.City = address.City;
            }

            // PayOne uses the "RS" country code for Kosovo the same as for Serbia
            if (poCustomer.Country == "XK")
            {
                poCustomer.Country = "RS";
            }

            return poCustomer;
        }

        private static void PopulateTransactionStatus(ResponseBase response, ExternalPaymentTransactionBaseDTO resultDto)
        {
            resultDto.Status = MapStatusValue(response);
            if (resultDto.Status != PaymentTransactionNewStatus.Failed)
            {
                return;
            }

            resultDto.Error = new ExternalIntegrationErrorDTO
            {
                PspErrorCode = response.ErrorCode,
                // Default Error Code if int parsing fails or no corresponding code could be found
                ErrorCode = PaymentErrorCode.UnmappedError
            };

            if (int.TryParse(resultDto.Error.PspErrorCode, out var errorCode))
            {
                if (errorCode >= 2 && errorCode <= 5 || errorCode >= 12 && errorCode <= 14 ||
                    errorCode >= 34 && errorCode <= 62 || errorCode >= 701 && errorCode <= 876)
                {
                    resultDto.Error.ErrorCode = PaymentErrorCode.Rejected;
                }
                else
                    switch (errorCode)
                    {
                        case 887:
                            resultDto.Error.ErrorCode = PaymentErrorCode.InvalidBic;
                            break;
                        case 888:
                            resultDto.Error.ErrorCode = PaymentErrorCode.InvalidIBAN;
                            break;
                        case 2010:
                            resultDto.Error.ErrorCode = PaymentErrorCode.RateLimit;
                            break;
                        default:
                        {
                            if (errorCode >= 1000 && errorCode < 2000 || errorCode == 2011)
                            {
                                resultDto.Error.ErrorCode = PaymentErrorCode.InvalidData;
                            }
                            else if (errorCode >= 2000 && errorCode < 3000 || errorCode == 4001 || errorCode == 4002 ||
                                     errorCode >= 4101 && errorCode < 4219)
                            {
                                resultDto.Error.ErrorCode = PaymentErrorCode.PermissionDenied;
                            }
                            else if (errorCode == 4010 || errorCode == 4011 || errorCode == 4743)
                            {
                                resultDto.Error.ErrorCode = PaymentErrorCode.InvalidCountry;
                            }
                            else if (errorCode >= 3000 && errorCode < 4000)
                            {
                                resultDto.Error.ErrorCode = PaymentErrorCode.InvalidPreconditions;
                            }
                            else if (errorCode >= 4000 && errorCode < 5000)
                            {
                                resultDto.Error.ErrorCode = PaymentErrorCode.InvalidData;
                            }

                            break;
                        }
                    }
            }

            resultDto.Error.ErrorMessage = response.ErrorMessage;
        }

        private static PaymentTransactionNewStatus MapStatusValue(ResponseBase response)
        {
            return response.Status switch
            {
                "APPROVED" => PaymentTransactionNewStatus.Succeeded,
                "REDIRECT" => PaymentTransactionNewStatus.Pending,
                _ => PaymentTransactionNewStatus.Failed
            };
        }

        private async Task<ExternalPaymentResponseWrapperDTO> ProcessCaptureTransactionAsync(ExternalPaymentRequestWrapperDTO paymentRequest)
        {
            var paymentDto = paymentRequest.PaymentRequestDto;
            var preauthDto = paymentRequest.PreauthRequestDto;
            var dto = (ExternalPaymentTransactionBaseRequestDTO) paymentDto ?? preauthDto;
            var settings = GetPspSettings<PayOnePSPSettings>(dto.MerchantSettings);
            var role = paymentDto?.PaymentMeansReference?.Role ?? preauthDto.PaymentMeansReference.Role;
            var captureAmount = paymentDto?.RequestedAmount ?? 0M;
            var request = new Capture(true, settings)
            {
                Amount = ((int) (captureAmount * 100)).ToString(CultureInfo.InvariantCulture),
                Currency = dto.Currency,
                Narrative_Text = role == PaymentProviderRole.OnAccount
                    ? preauthDto.TransactionInvoiceReferenceText
                    : preauthDto.TransactionReferenceText,
                TxId = paymentRequest.PspTransactionId,
                Transaction_Param = preauthDto.InvoiceReferenceCode,
                Due_Time = GetDueTime(paymentDto)
            };

            var restResult = await _payOneWrapper.ExecutePayOneRequestAsync(request);
            var response = new CaptureResponse(restResult.Data);

            _logger.LogInformation(
                $"Processed PayOne capture payment {response.TxId} for payment transaction {paymentDto?.TransactionId ?? preauthDto.TransactionId}. Status: {response.Status}");

            var result = BuildAndPopulateExternalTransactionBaseDto<ExternalPaymentTransactionDTO>(dto, response.TxId);

            result.Bearer = paymentRequest.BearerDto;

            PopulateTransactionStatus(response, result);

            if (result.Status == PaymentTransactionNewStatus.Succeeded && role == PaymentProviderRole.OnAccount)
            {
                result.Status = PaymentTransactionNewStatus.Pending;
            }

            var captureResult = new ExternalPaymentResponseWrapperDTO(null, result);
            if (result.Status == PaymentTransactionNewStatus.Failed) 
                return captureResult;

            if (captureResult.PaymentDto.Bearer != null)
            {
                UpdateMandateIfRequired(captureResult.PaymentDto.Bearer, role, response.Mandate_Identification,
                    response.Mandate_Dateofsignature, response.Creditor_Identifier);
            }

            PopulatePspDueDate(captureResult.PaymentDto, role, response.Clearing_Date);

            return captureResult;
        }

        
        
        private async Task<ExternalPaymentResponseWrapperDTO> ProcessTransaction(ExternalPaymentRequestWrapperDTO paymentRequest, IRecurringToken token)
        {
            var settings = GetPspSettings<PayOnePSPSettings>(paymentRequest.PaymentRequestDto.MerchantSettings);
            var paymentDto = paymentRequest.PaymentRequestDto;
            var role = paymentDto.PaymentMeansReference.Role;
            var recurringToken = token as IPayOneRecurringToken;

            var request = new Authorization(false, settings)
            {
                Amount = ((int) (paymentDto.RequestedAmount * 100)).ToString(CultureInfo.InvariantCulture),
                Currency = paymentDto.Currency,
                ClearingType = GetClearingType(role),
                Narrative_Text = role == PaymentProviderRole.OnAccount
                    ? paymentDto.TransactionInvoiceReferenceText
                    : paymentDto.TransactionReferenceText,
                Reference = BuildReference(paymentDto.TransactionId),
                Param = paymentDto.TransactionId,
                Transaction_Param = paymentDto.InvoiceReferenceCode,
                PaymentBearer = GetPaymentBearer(recurringToken.PspBearer as PayOnePspBearer, role),
                Customer = GetPayOneCustomer(paymentDto.PayerData, recurringToken.UserId),
                SuccessUrl = paymentDto.PaymentMeansReference.SuccessReturnUrl,
                ErrorUrl = paymentDto.PaymentMeansReference.ErrorReturnUrl,
                BackUrl = paymentDto.PaymentMeansReference.AbortReturnUrl,
                ECommerceMode = "internet",
                Due_Time = GetDueTime(paymentDto)
            };

            var restResult = await _payOneWrapper.ExecutePayOneRequestAsync(request);
            var response = new AuthorizationResponse(restResult.Data);

            _logger.LogInformation(
                $"Processed PayOne payment {response.TxId} for payment transaction {paymentDto.TransactionId}. Status: {response.Status}");

            var result = BuildAndPopulateExternalTransactionBaseDto<ExternalPaymentTransactionDTO>(paymentDto, response.TxId);

            result.Bearer = recurringToken.PaymentBearer;

            PopulateTransactionStatus(response, result);

            if (result.Status == PaymentTransactionNewStatus.Succeeded && role == PaymentProviderRole.OnAccount)
            {
                result.Status = PaymentTransactionNewStatus.Pending;
            }

            var recurringResult = new ExternalPaymentResponseWrapperDTO(null, result);
            
            if (result.Status == PaymentTransactionNewStatus.Failed) 
                return recurringResult;

            UpdateMandateIfRequired(recurringResult.PaymentDto.Bearer, role, response.Mandate_Identification, response.Mandate_Dateofsignature, response.Creditor_Identifier);

            PopulatePspDueDate(recurringResult.PaymentDto, role, response.Clearing_Date);

            //TODO Why do we skip filling RecurringToken in response with this condition?
            if (response.UserId == recurringToken.UserId)
            {
                return recurringResult;
            }

            recurringToken.UserId = response.UserId;
            recurringResult.RecurringToken = recurringToken;

            return recurringResult;
        }

        private static void PopulatePspDueDate(ExternalPaymentTransactionDTO dto, PaymentProviderRole role,
            string responseClearingDate)
        {
            if (role == PaymentProviderRole.Debit && !string.IsNullOrEmpty(responseClearingDate))
            {
                dto.DueDate = ParsePayOneDate(responseClearingDate);
            }
            else
            {
                dto.DueDate = null;
            }
        }

        private static string GetDueTime(ExternalPaymentRequestDTO paymentDto)
        {
            return paymentDto?.PlannedExecutionDate?.AtMidnight().ToDateTimeUnspecified().ToUnixTime().ToString();
        }

        private static string BuildReference(string transactionId)
        {
            return Base32.Encode(Encoding.UTF8.GetBytes(transactionId)).Substring(transactionId.Length - 20, 19);
        }
    }
}