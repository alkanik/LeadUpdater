using IncredibleBackend.Messaging;
using IncredibleBackend.Messaging.Extentions;
using IncredibleBackend.Messaging.Interfaces;
using IncredibleBackendContracts.Constants;
using IncredibleBackendContracts.Events;
using LeadUpdater;
using LeadUpdater.Extensions;
using LeadUpdater.Infrastructure;
using LeadUpdater.Policies;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using NLog;
using NLog.Extensions.Logging;
using NLog.Web;

var builder = WebApplication.CreateBuilder(args);
LogManager.Configuration.Variables[$"{builder.Environment: LOG_DIRECTORY}"] = "Logs";

IHost host = Host.CreateDefaultBuilder(args)
    .UseWindowsService(options =>
    {
        options.ServiceName = "LeadUpdater";
    })
    .ConfigureLogging((hostContext, logging) =>
    {
        logging.ClearProviders();
        logging.AddNLog();
    })
    .ConfigureServices(services =>
    {
        ServiceCollectionExtensions.AddServices(services);
        ServiceCollectionExtensions.ConfigureService(services, builder);
    })
    .Build();

await host.RunAsync();