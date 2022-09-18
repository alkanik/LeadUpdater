using IncredibleBackend.Messaging;
using IncredibleBackend.Messaging.Extentions;
using IncredibleBackendContracts.Constants;
using IncredibleBackendContracts.Events;
using LeadUpdater;
using LeadUpdater.Infrastructure;
using LeadUpdater.Interfaces;
using LeadUpdater.Policies;
using LeadUpdater.Producers;
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
        services.AddHostedService<Worker>();
        services.AddHttpClient("Reporting").AddPolicyHandler(
            request => request.Method == HttpMethod.Get ? new ClientPolicy().RetryPolicy : new ClientPolicy().RetryPolicy);
        services.AddScoped<IReportingClient, ReportingClient>();
        services.AddScoped<IVipStatusService, VipStatusService>();
        services.AddScoped<IScheduler, Scheduler>();
        services.AddScoped<ILeadIdsProducer, LeadIdsProducer>();
        services.AddSingleton<ClientPolicy>(new ClientPolicy());

        services.AddScoped<MessageProducer>();
        services.RegisterConsumersAndProducers(
            null, 
            null,
            (cfg) =>
            {
               cfg.RegisterProducer<LeadsRoleUpdatedEvent>(RabbitEndpoint.LeadsRoleUpdateCrm);
            });
        services.Configure<VipStatusConfiguration>(builder.Configuration);
    })
    .Build();

await host.RunAsync();