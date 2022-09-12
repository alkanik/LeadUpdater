using Cronos;
using LeadUpdater.Infrastructure;

namespace LeadUpdater;

public class Worker : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<Worker> _logger;
    private readonly CronExpression _cronJob;

    public Worker(IServiceProvider serviceProvider, ILogger<Worker> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _cronJob = CronExpression.Parse(Constant.CronExpressionTest, CronFormat.IncludeSeconds);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("LeadUpdater running at: {time}", DateTimeOffset.Now);

            var now = DateTime.UtcNow;
            var nextUtc = _cronJob.GetNextOccurrence(now);
            var delayTimeSpan = (nextUtc.Value - now);
            await Task.Delay(delayTimeSpan, stoppingToken);

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