// Copyright (c) billwerk GmbH. All rights reserved

namespace PaymentGateway.Shared;

public static class StreamExtensions
{
    public static async Task<string> ReadAsStringAndResetReaderAsync(this Stream stream)
    {
        stream.Seek(0, SeekOrigin.Begin);
        var sourceAsString = await new StreamReader(stream).ReadToEndAsync();
        stream.Seek(0, SeekOrigin.Begin);
        return sourceAsString;
    }
}
