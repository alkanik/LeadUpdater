using LeadUpdater;
using LeadUpdater.Policies;
using NLog.Extensions.Logging;
using NLog.Web;
using Polly;

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
        services.AddSingleton<ClientPolicy>(new ClientPolicy());
    })
    .Build();
await host.RunAsync();