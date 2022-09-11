using LeadUpdater;
using LeadUpdater.Business;
using Polly.Extensions.Http;
using Polly;
using Polly.Retry;

IHost host = Host.CreateDefaultBuilder(args)
    .UseWindowsService(options =>
    {
        options.ServiceName = "LeadUpdater";
    })
    .ConfigureServices(services =>
    {
        services.AddHostedService<Worker>();
        services.AddHttpClient();
        services.AddScoped<IReportingClient, ReportingClient>();
    })
    .Build();
await host.RunAsync();