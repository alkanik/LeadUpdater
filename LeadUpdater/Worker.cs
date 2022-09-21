using Cronos;
using IncredibleBackend.Messaging;
using IncredibleBackend.Messaging.Interfaces;
using LeadUpdater.Infrastructure;
using NLog.Extensions.Logging;

namespace LeadUpdater;

public class Worker : BackgroundService
{
    private readonly IMessageProducer _messageProducer;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<Worker> _logger;

    public Worker(IMessageProducer messageProducer, IServiceProvider serviceProvider, ILogger<Worker> logger)
    {
        _messageProducer = messageProducer;
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

                _logger.LogInformation("Next update will be at : {time}", (DateTimeOffset.Now + delayTimeSpan));
                await Task.Delay(delayTimeSpan, stoppingToken);

                IReportingClient httpClientService =
                    scope.ServiceProvider.GetRequiredService<IReportingClient>();
            
                var vipStatusService = scope.ServiceProvider.GetRequiredService<IVipStatusService>();
                await vipStatusService.GetVipLeadsIds();
            }
        }
    }
}