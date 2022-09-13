using MassTransit;
using Microsoft.Extensions.Logging;

namespace LeadUpdater.RabbitMQ.Producer
{
    public class LeadIdsProducer : ILeadIdsProducer
    {
        private readonly IPublishEndpoint _publisEndpoint;

        private readonly ILogger _logger;

        public LeadIdsProducer(IPublishEndpoint publisEndpoint, ILogger logger)
        {
            _publisEndpoint = publisEndpoint;
            _logger = logger;
        }

        public async Task SendMessage<T> (T message)
        {
            try
            {
                await _publisEndpoint.Publish(message);
                _logger.LogInformation("Send Lead Ids to Queue");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }
    }
}
