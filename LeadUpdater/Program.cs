using IncredibleBackend.Messaging;
using IncredibleBackendContracts.Constants;
using IncredibleBackendContracts.Events;
using LeadUpdater;
using LeadUpdater.Interfaces;
using LeadUpdater.Policies;
using LeadUpdater.Producers;
using Microsoft.Extensions.Hosting;
using NLog.Extensions.Logging;
using NLog.Web;

IHost host = Host.CreateDefaultBuilder(args)
    .UseWindowsService(options =>
    {
        options.ServiceName = "LeadUpdater";
    })
    .ConfigureAppConfiguration(config =>
    {
        config.AddEnvironmentVariables();
    })
    .ConfigureLogging((hostContext, logging) =>
    {
        logging.ClearProviders();
        logging.SetMinimumLevel(LogLevel.Information);
        logging.ConfigureNLog("nlog.config");
        //logging.AddNLog();
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
    })
    .Build();
await host.RunAsync();