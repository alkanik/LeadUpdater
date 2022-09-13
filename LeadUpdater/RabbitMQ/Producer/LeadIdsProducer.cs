using IncredibleBackendContracts.Constants;
using MassTransit;
using NLog.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;
using LeadUpdater.Interfaces;

namespace LeadUpdater.RabbitMQ.Producer
{
    public class LeadIdsProducer : ILeadIdsProducer
    {
        private readonly IPublishEndpoint _publisEndpoint;
        private readonly ILogger<LeadIdsProducer> _logger;

        public LeadIdsProducer(IPublishEndpoint publisEndpoint, ILogger<LeadIdsProducer> logger)
        {
            _publisEndpoint = publisEndpoint;
            _logger = logger;
        }

        public async Task SendMessage<T> (T message)
        {
            try
            {
                var factory = new ConnectionFactory { HostName = "localhost" };
                var connection = factory.CreateConnection();
                using var channel = connection.CreateModel();


                channel.QueueDeclare(RabbitEndpoint.LeadsRoleUpdateCrm, exclusive: false);
                _logger.LogInformation($"Queue {RabbitEndpoint.LeadsRoleUpdateCrm} created ");


                var json = JsonConvert.SerializeObject(message);
                var body = Encoding.UTF8.GetBytes(json);

                //await _publisEndpoint.Publish(body);
                channel.BasicPublish(exchange: "", routingKey: RabbitEndpoint.LeadsRoleUpdateCrm, body: body);

                _logger.LogInformation("Send Lead Ids to Queue");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }
    }
}
