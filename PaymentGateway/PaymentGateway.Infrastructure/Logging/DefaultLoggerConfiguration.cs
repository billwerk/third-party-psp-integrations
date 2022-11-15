// Copyright (c) billwerk GmbH. All rights reserved

using PaymentGateway.Infrastructure.DataAccess.Database;
using Serilog;
using Serilog.Events;

namespace PaymentGateway.Infrastructure.Logging;

public static class DefaultLoggerConfiguration
{
    public static LoggerConfiguration Create(IDatabaseSettings databaseSettings) => new LoggerConfiguration()
        .MinimumLevel.Information()
        .MinimumLevel.Override("Microsoft", LogEventLevel.Error)
        .MinimumLevel.Override("System", LogEventLevel.Warning)
        .Enrich.FromLogContext()
        .WriteTo.MongoDB($"{databaseSettings.ConnectionString}/{databaseSettings.DatabaseName}", "Log");
}
