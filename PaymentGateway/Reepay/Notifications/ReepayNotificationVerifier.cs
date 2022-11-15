// Copyright (c) billwerk GmbH. All rights reserved

using System.Security.Cryptography;
using System.Text;
using PaymentGateway.Shared;
using PaymentGateway.Domain.Modules.PSP.Settings;

namespace Reepay.Notifications;

internal static class ReepayNotificationVerifier
{
    public static bool Verify(ReepayNotification reepayNotification, in ReepaySettings reepaySettings) =>
        BuildEncoderWithWebhookSecret(reepaySettings)
            .ComputeHash(GetSignatureFromNotificationAsByteArray(reepayNotification))
            .ToHexString()
            .Equals(reepayNotification.Signature);

    private static HMACSHA256 BuildEncoderWithWebhookSecret(ReepaySettings reepaySettings) =>
        new(reepaySettings.WebhookSecret.Value.To(Encoding.ASCII.GetBytes));

    private static byte[] GetSignatureFromNotificationAsByteArray(ReepayNotification reepayNotification) =>
        Encoding.ASCII.GetBytes($"{reepayNotification.Timestamp}{reepayNotification.Id}");

    private static string ToHexString(this IReadOnlyCollection<byte> array)
        => string.Join("", array.Select(b => $"{b:x2}"));
}
