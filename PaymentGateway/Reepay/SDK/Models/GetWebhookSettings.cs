// Copyright (c) billwerk GmbH. All rights reserved

using Newtonsoft.Json;

namespace Reepay.SDK.Models;

public class GetWebhookSettings
{
    /// <summary>
    /// Represent set of urls to which Reepay will send webhooks. 
    /// </summary>
    [JsonProperty(PropertyName = "urls")]
    public string[] Urls { get; init; }

    /// <summary>
    /// Identify, is webhook mechanism on or off.
    /// </summary>
    [JsonProperty(PropertyName = "disabled")]
    public bool Disabled { get; init; }

    /// <summary>
    /// Set of events, for which Reepay will send webhooks.
    /// </summary>
    [JsonProperty(PropertyName = "event_types")]
    public string[] EventTypes { get; init; }
    
    /// <summary>
    /// Webhook secret. Used to verify received Reepay webhooks.
    /// </summary>
    [JsonProperty(PropertyName = "secret")]
    public string Secret { get; init; }

}
