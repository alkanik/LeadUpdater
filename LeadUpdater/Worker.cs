using LeadUpdater.Infrastructure;

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
        _logger.LogInformation($"{Constant.LogMessageLeadUpdaterRun}{DateTimeOffset.Now}");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    IScheduler scheduler =
                        scope.ServiceProvider.GetRequiredService<IScheduler>();

                    var delayTimeSpan = scheduler.GetDelayTimeSpan();

                    _logger.LogInformation($"{Constant.LogMessageNextUpdate}{(DateTimeOffset.Now + delayTimeSpan)}");
                    await Task.Delay(delayTimeSpan, stoppingToken);

                    IReportingClient httpClientService =
                        scope.ServiceProvider.GetRequiredService<IReportingClient>();

                    var vipStatusService = scope.ServiceProvider.GetRequiredService<IVipStatusService>();
                    await vipStatusService.GetVipLeadsIds();
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"{ex.Message}");
            }
            await Task.Delay(1000, stoppingToken);
        }
    }
}