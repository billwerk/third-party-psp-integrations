// Copyright (c) billwerk GmbH. All rights reserved

using PaymentGateway.Application.BillwerkSDK.DTO.Requests.Interfaces;

namespace PaymentGateway.Application.BillwerkSDK.DTO.Requests;

public class FinalizePreauthRequestDto : IRequestDto
{
    /// <summary>
    /// The transaction identifier from billwerk side.
    /// Mandatory.
    /// </summary>
    public string TransactionId { get; set; }

    public IDictionary<string, string> FinalizationData { get; set; }
}
