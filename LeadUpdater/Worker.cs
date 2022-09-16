using Cronos;
using LeadUpdater.Infrastructure;
using LeadUpdater.Interfaces;
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
        _logger.LogInformation("LeadUpdater running at: {time}", DateTimeOffset.Now);
        
        while (!stoppingToken.IsCancellationRequested)
        {

            using (var scope = _serviceProvider.CreateScope())
            {
                IScheduler scheduler =
                    scope.ServiceProvider.GetRequiredService<IScheduler>();
                var delayTimeSpan = scheduler.GetDelayTimeSpan();
                _logger.LogInformation("LeadUpdater will start in : {time}", (DateTimeOffset.Now + delayTimeSpan));
                await Task.Delay(delayTimeSpan, stoppingToken);

                IReportingClient httpClientService =
                    scope.ServiceProvider.GetRequiredService<IReportingClient>();
            
                var vipStatusService = scope.ServiceProvider.GetRequiredService<IVipStatusService>();
                var vipLeadsIds = await vipStatusService.GetVipLeadsIds();

                ILeadIdsProducer leadIdsProducer = 
                    scope.ServiceProvider.GetRequiredService<ILeadIdsProducer>();



                await leadIdsProducer.SendMessage(vipLeadsIds);
            }
        }
    }
}