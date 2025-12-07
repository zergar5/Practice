using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;

void ConfigureServices(IServiceCollection services)
{
    IConfiguration configuration = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .Build();

    services.AddSingleton(configuration);

    Log.Logger = new LoggerConfiguration()
        .ReadFrom.Configuration(configuration)
        .Enrich.FromLogContext()
        .CreateLogger();
    services.AddLogging(loggingBuilder =>
        loggingBuilder.AddSerilog(dispose: true));
}
var services = new ServiceCollection();
ConfigureServices(services);
var provider = services.BuildServiceProvider();

var logger = provider.GetRequiredService<ILogger<Program>>();
logger.LogInformation("DirectProblemConsole, You're just a miserable copy of me!");
logger.LogCritical("No, I'm the upgrade!");
