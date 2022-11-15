// Copyright (c) billwerk GmbH. All rights reserved

namespace Reepay.SDK.Models.RecurringSession;

/// <summary>
/// https://docs.reepay.com/reference/createrecurringsession
/// </summary>
/// <param name="Email">Customer email. Validated against RFC 822.</param>
/// <param name="FirstName">Customer first name</param>
/// <param name="LastName">Customer last name</param>
/// <param name="Handle">Per account unique handle for the customer. Max length 255 with allowable characters [a-zA-Z0-9_.-@]. Must be provided if generate_handle is not defined.</param>
public record RecurringSessionCustomer(
    string Email,
    string FirstName,
    string LastName,
    string Handle);
