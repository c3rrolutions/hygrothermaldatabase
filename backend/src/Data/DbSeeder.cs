using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Database.Data;

public static partial class Log
{
    [LoggerMessage(
        EventId = 0,
        Level = LogLevel.Debug,
        Message = "Seeding the database")]
    public static partial void SeedingDatabase(
        this ILogger<DbSeeder> logger
    );
}

public sealed class DbSeeder
{
    public static async Task DoAsync(
        IServiceProvider services
    )
    {
        var logger = services.GetRequiredService<ILogger<DbSeeder>>();
        logger.SeedingDatabase();
        // var environment = services.GetRequiredService<IWebHostEnvironment>();
        // var appSettings = services.GetRequiredService<AppSettings>();
        await Task.FromResult(0);
    }
}