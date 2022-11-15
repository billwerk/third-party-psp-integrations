// Copyright (c) billwerk GmbH. All rights reserved

using Billwerk.Payment.SDK.DTO.Responses.Error;
using Billwerk.Payment.SDK.DTO.Responses.Interfaces;
using PaymentGateway.Domain.Shared.ValueObjects;
using PaymentGateway.Shared;

namespace PaymentGateway.Application.Notification.Types.Transaction;

public interface ITransactionNotificationHandlingResult {}

public record NotificationHandlingResult(Result<ITransactionResponseDto, PaymentErrorDto> Result) : ITransactionNotificationHandlingResult;

public record IgnoredNotificationHandlingResult(NotEmptyString SkippingReason) : ITransactionNotificationHandlingResult;
