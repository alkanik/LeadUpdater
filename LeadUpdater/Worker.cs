using Cronos;
using LeadUpdater.Infrastructure;
using NLog.Extensions.Logging;

namespace LeadUpdater;

public class Worker : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<Worker> _logger;

    public Worker(IServiceProvider serviceProvider, ILogger<Worker> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("LeadUpdater running at: {time}", DateTimeOffset.Now);

            using (var scope = _serviceProvider.CreateScope())
            {
                IScheduler scheduler =
                    scope.ServiceProvider.GetRequiredService<IScheduler>();
                var delayTimeSpan = scheduler.GetDelayTimeSpan();
                await Task.Delay(delayTimeSpan, stoppingToken);
            }
            //var now = DateTime.UtcNow;
            //var nextUtc = _cronJob.GetNextOccurrence(now);
            //var delayTimeSpan = (nextUtc.Value - now);
            //await Task.Delay(delayTimeSpan, stoppingToken);

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
        }
    }
}