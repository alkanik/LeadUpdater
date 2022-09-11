using Microsoft.Extensions.DependencyInjection;

namespace LeadUpdater;

public class Worker : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<Worker> _logger;

    public Worker(IServiceProvider serviceProvider,
    ILogger<Worker> logger) =>
    (_serviceProvider, _logger) = (serviceProvider, logger);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("LeadUpdater running at: {time}", DateTimeOffset.Now);
            using (var scope = _serviceProvider.CreateScope())
            {
                IReportingClient httpClientService =
                    scope.ServiceProvider.GetRequiredService<IReportingClient>();
            }

            using (var scope = _serviceProvider.CreateScope())
            {
                var vipStatusService = scope.ServiceProvider.GetRequiredService<IVipStatusService>();
                await vipStatusService.GetVipLeadsIds();
            }

            await Task.Delay(5000, stoppingToken);
        }
    }
}