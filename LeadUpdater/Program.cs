using LeadUpdater.Extensions;
using LeadUpdater.Infrastructure;
using Microsoft.AspNetCore.Builder;
using NLog;
using NLog.Extensions.Logging;
using NLog.Web;

var builder = WebApplication.CreateBuilder(args);
LogManager.Configuration.Variables[$"{builder.Environment: LOG_DIRECTORY}"] = "Logs";

IHost host = Host.CreateDefaultBuilder(args)
    .UseWindowsService(options =>
    {
        options.ServiceName = Constant.ServiceName;
    })
    .ConfigureLogging((hostContext, logging) =>
    {
        logging.ClearProviders();
        logging.AddNLog();
    })
    .ConfigureServices((hostContext, services) =>
    {
        IConfiguration configuration = hostContext.Configuration;
        services.Configure<VipStatusConfiguration>(configuration.GetSection("VipStatusConfiguration"));
        ServiceCollectionExtensions.AddServices(services);
        ServiceCollectionExtensions.ConfigureService(services, builder);
    })
    .Build();

await host.RunAsync();