using LeadUpdater;
using LeadUpdater.Business;

IHost host = Host.CreateDefaultBuilder(args)
    .UseWindowsService(options =>
    {
        options.ServiceName = "LeadUpdater";
    })
    .ConfigureServices(services =>
    {
        services.AddHostedService<Worker>();
        services.AddHttpClient();
        services.AddScoped<IHttpClientService, HttpClientService>();
    })
    .Build();
await host.RunAsync();