// Copyright (c) billwerk GmbH. All rights reserved

using System.Text.RegularExpressions;
using PaymentGateway.Shared;

namespace FakeProvider.Bearers;

public class DirectDebitSimpleBearer
{
    public string Holder { get; set; }

    public string IBAN { get; set; }

    public string MandateId { get; set; }

    public bool IsValid() => !IBAN.IsEmpty() && !Holder.IsEmpty()
                                             && Regex.IsMatch(IBAN, IBANRegexPattern);

    private const string IBANRegexPattern = @"^([A-Z]{2}[ \-]?[0-9]{2})(?=(?:[ \-]?[A-Z0-9]){9,30}$)((?:[ \-]?[A-Z0-9]{3,5}){2,7})([ \-]?[A-Z0-9]{1,3})?$";
}
