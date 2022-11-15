namespace PaymentGateway;

public static class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args).ConfigureBuilder();
        var app = builder.Build().ConfigureApplication();

        await app.RunAsync();
    }
}
