using LeadUpdater;
using LeadUpdater.Policies;

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
        services.AddSingleton<ClientPolicy>(new ClientPolicy());
    })
    .Build();
await host.RunAsync();