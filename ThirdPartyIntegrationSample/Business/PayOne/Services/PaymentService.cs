using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using Billwerk.Payment.SDK.DTO;
using Billwerk.Payment.SDK.DTO.ExternalIntegration;
using Billwerk.Payment.SDK.DTO.ExternalIntegration.Cancellation;
using Billwerk.Payment.SDK.DTO.ExternalIntegration.Payment;
using Billwerk.Payment.SDK.DTO.ExternalIntegration.Preauth;
using Billwerk.Payment.SDK.DTO.ExternalIntegration.Refund;
using Billwerk.Payment.SDK.Enums;
using Business.Interfaces;
using Business.Models;
using Business.PayOne.Helpers;
using Business.PayOne.Interfaces;
using Business.PayOne.Model;
using Business.PayOne.Model.Requests;
using Business.PayOne.Model.Responses;
using Core.Helpers;
using Microsoft.Extensions.Logging;

namespace Business.PayOne.Services
{
    public class PaymentService : PaymentServiceBase, IPaymentService
    {
        private const string PayOneDateFormat = "yyyyMMdd";

        private readonly ILogger<PaymentService> _logger;
        private readonly IPayOneWrapper _payOneWrapper;
        private readonly IInitialTokenDecoder _initialTokenDecoder;
        private readonly IRecurringTokenEncoder<RecurringToken> _recurringTokenEncoder;

        public PaymentService(ILogger<PaymentService> logger, IPayOneWrapper payOneWrapper,
            IInitialTokenDecoder initialTokenDecoder, IRecurringTokenEncoder<RecurringToken> recurringTokenEncoder)
        {
            _logger = logger;
            _payOneWrapper = payOneWrapper;
            _initialTokenDecoder = initialTokenDecoder;
            _recurringTokenEncoder = recurringTokenEncoder;
        }

        public async Task<ExternalPaymentTransactionDTO> SendPayment(ExternalPaymentRequest paymentRequest)
        {
            var isCapture =
                string.IsNullOrWhiteSpace(paymentRequest.PaymentRequestDto.PaymentMeansReference.PreauthTransactionId) &&
                paymentRequest.PreauthRequestDto != null;
            if (isCapture)
            {
                return await ProcessCaptureTransactionAsync(paymentRequest);
            }

            return null;
        }

        public Task<ExternalRefundTransactionDTO> SendRefund(ExternalRefundRequestDTO dto)
        {
            throw new NotImplementedException();
        }

        public async Task<ExternalPreauthTransactionDTO> SendPreauth(ExternalPreauthRequestDTO dto)
        {
            var payer = dto.PayerData;
            var initialPayment = !string.IsNullOrWhiteSpace(dto.PaymentMeansReference.InitialToken);
            var settings = GetPspSettings<PayOnePSPSettings>(dto.MerchantSettings);
            var tetheredPaymentInformation = _initialTokenDecoder.Decode(dto.PaymentMeansReference.InitialToken);
            var role = dto.PaymentMeansReference.Role;
            var request = new Preauthorization(initialPayment, settings)
            {
                Amount = ((int) (dto.RequestedAmount * 100)).ToString(CultureInfo.InvariantCulture),
                Currency = dto.Currency,
                ClearingType = GetClearingType(role),
                Narrative_Text = role == PaymentProviderRole.OnAccount
                    ? dto.TransactionInvoiceReferenceText
                    : dto.TransactionReferenceText,
                Reference = Base32.Encode(Encoding.UTF8.GetBytes(dto.TransactionId))
                    .Substring(dto.TransactionId.Length - 20, 19),
                Param = dto.TransactionId,
                PaymentBearer = GetPaymentBearer(tetheredPaymentInformation, role),
                Customer = GetPayOneCustomer(payer),
                SuccessUrl = dto.PaymentMeansReference.ReturnUrl,
                ErrorUrl = dto.PaymentMeansReference.ReturnUrl,
                BackUrl = dto.PaymentMeansReference.ReturnUrl,
                ECommerceMode = initialPayment ? null : "internet"
            };

            var restResult = await _payOneWrapper.ExecutePayOneRequestAsync(request);
            var response = new PreauthorizationResponse(restResult.Data);

            _logger.LogInformation(
                $"Processed PayOne Preapproval {response.TxId} for payment transaction {dto.TransactionId}. Status: {response.Status}");

            return BuildAndPopulateExternalPreauthTransactionDto(dto, response, tetheredPaymentInformation);
        }

        public async Task<ExternalPaymentCancellationDTO> SendCancellation(ExternalPreauthRequestDTO dto)
        {
            var paymentRequest = new ExternalPaymentRequest
            {
                PreauthRequestDto = dto
            };

            var result = await ProcessCaptureTransactionAsync(paymentRequest);
            if (result.Status == PaymentTransactionNewStatus.Succeeded)
            {
                return new ExternalPaymentCancellationDTO
                {
                    CancellationStatus = result.Status.ToString(),
                    TransactionId = result.TransactionId
                };
            }

            return new ExternalPaymentCancellationDTO
            {
                Error = result.Error
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

        private ExternalPreauthTransactionDTO BuildAndPopulateExternalPreauthTransactionDto(ExternalPreauthRequestDTO dto,
            PreauthorizationResponse response, PayOnePspBearer tetheredPaymentInformation)
        {
            var role = dto.PaymentMeansReference.Role;
            var result = BuildAndPopulateExternalTransactionBaseDto<ExternalPreauthTransactionDTO>(dto, response.TxId);

            result.AuthorizedAmount = dto.RequestedAmount;
            //ToDo: Should be clarified
            result.ExpiresAt = DateTimeOffset.UtcNow.AddYears(1);
            result.Bearer = GetPaymentBearerDto(tetheredPaymentInformation, role);

            PopulateTransactionStatus(response, result);

            //ToDo: Should be discussed
            // if (result.Status == PaymentStatusValue.ThreeDSecurePending)
            // {
            //     postProcessUrl = response.RedirectUrl;
            // }

            if (result.Status == PaymentTransactionNewStatus.Succeeded && role == PaymentProviderRole.OnAccount)
            {
                result.Status = PaymentTransactionNewStatus.Pending;
            }

            if (result.Status == PaymentTransactionNewStatus.Failed) return result;

            UpdateMandateIfRequired(result.Bearer, role, response.Mandate_Identification, response.Mandate_Dateofsignature);

            result.RecurringToken = _recurringTokenEncoder.Encrypt(new RecurringToken
            {
                UserId = response.UserId
            });

            return result;
        }

        private static PaymentBearerDTO GetPaymentBearerDto(PayOnePspBearer payOnePspBearer, PaymentProviderRole role)
        {
            switch (role)
            {
                case PaymentProviderRole.CreditCard:
                    return new PaymentBearerCreditCardDTO()
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
                        bearerBankAccount.Holder = payOnePspBearer.Holder;
                    }
                    else
                    {
                        bearerBankAccount.Holder = payOnePspBearer.Holder;
                        bearerBankAccount.Account = payOnePspBearer.Account;
                        bearerBankAccount.Country = payOnePspBearer.Country;
                        bearerBankAccount.Code = payOnePspBearer.Code;
                    }

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
            string mandateIdentification, string mandateDateofsignature)
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
                _ => throw new ValidationException($"Role={role} is not supported!")
            };
        }

        private static Customer GetPayOneCustomer(PayerDataDTO payer)
        {
            var poCustomer = new Customer
            {
                FirstName = payer.FirstName,
                LastName = payer.LastName,
                Company = payer.CompanyName,
                //ToDo: Populate later
                //UserId = customerSettings?.PSPUserId
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

        private async Task<ExternalPaymentTransactionDTO> ProcessCaptureTransactionAsync(ExternalPaymentRequest paymentRequest)
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
                    : dto.TransactionReferenceText,
                TxId = paymentRequest.PspTransactionId,
                Transaction_Param = preauthDto.InvoiceReferenceCode,
                Due_Time = paymentDto?.PlannedExecutionDate.AtMidnight().ToDateTimeUnspecified().ToUnixTime().ToString()
            };

            var restResult = await _payOneWrapper.ExecutePayOneRequestAsync(request);
            var response = new CaptureResponse(restResult.Data);

            _logger.LogInformation(
                $"Processed PayOne capture payment {response.TxId} for payment transaction {paymentDto?.TransactionId ?? preauthDto.TransactionId}. Status: {response.Status}");

            var result = BuildAndPopulateExternalTransactionBaseDto<ExternalPaymentTransactionDTO>(paymentDto, response.TxId);

            result.Bearer = paymentRequest.BearerDto;

            PopulateTransactionStatus(response, result);

            if (result.Status == PaymentTransactionNewStatus.Succeeded && role == PaymentProviderRole.OnAccount)
            {
                result.Status = PaymentTransactionNewStatus.Pending;
            }

            if (result.Status == PaymentTransactionNewStatus.Failed) 
                return result;

            if (result.Bearer != null)
            {
                UpdateMandateIfRequired(result.Bearer, role, response.Mandate_Identification,
                    response.Mandate_Dateofsignature);
            }

            PopulatePspDueDate(result, role, response.Clearing_Date);

            return result;
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
    }
}