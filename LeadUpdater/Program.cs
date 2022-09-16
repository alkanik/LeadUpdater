using LeadUpdater;
using LeadUpdater.Policies;
using NLog.Extensions.Logging;
using NLog.Web;
using Polly;
using MassTransit;
using IncredibleBackendContracts.Events;
using IncredibleBackendContracts.Constants;
using LeadUpdater.RabbitMQ.Producer;
using LeadUpdater.Interfaces;
using IncredibleBackend.Messaging;
using IncredibleBackendContracts.Abstractions;

IHost host = Host.CreateDefaultBuilder(args)
    .UseWindowsService(options =>
    {
        options.ServiceName = "LeadUpdater";
    })
    .ConfigureLogging((hostContext, logging) =>
    {
        logging.ClearProviders();
        logging.SetMinimumLevel(LogLevel.Trace);
        logging.AddNLog(hostContext.Configuration, new NLogProviderOptions() { LoggingConfigurationSectionName = "NLog" });
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
               cfg.RegisterProducer<LeadsRoleUpdatedEvent>("LeadsRoleUpdateCrm");
            });
    })
    .Build();
await host.RunAsync();