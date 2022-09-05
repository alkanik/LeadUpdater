using LeadUpdater.Business;

namespace LeadUpdater
{
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
                await DoWorkAsync(stoppingToken);
            }
        }

        private async Task DoWorkAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation(
                $"{nameof(Worker)} is working.");

            using (IServiceScope scope = _serviceProvider.CreateScope())
            {
                IHttpClientService httpClientService =
                    scope.ServiceProvider.GetRequiredService<IHttpClientService>();

                await httpClientService.Execute();
            }
        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation(
                $"{nameof(Worker)} is stopping.");

            await base.StopAsync(stoppingToken);
        }
    }
}