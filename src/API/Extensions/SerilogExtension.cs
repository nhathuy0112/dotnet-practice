using Serilog;

namespace API.Extensions;

public static class SerilogExtension
{
    public static IHostBuilder AddSeriLog(this IHostBuilder host, IConfiguration configuration)
    {
        return host.UseSerilog((_, config) =>
        {
            config.ReadFrom.Configuration(configuration);
        });
    }
}