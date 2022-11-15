// Copyright (c) billwerk GmbH. All rights reserved

namespace PaymentGateway.Shared;

public static class StringExtensions
{
    public static string Format(this string str, params string[] args) => string.Format(str, args);

    public static bool IsEmpty(this string str) => string.IsNullOrWhiteSpace(str);
}
