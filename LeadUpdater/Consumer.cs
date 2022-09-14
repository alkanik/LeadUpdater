using IncredibleBackendContracts.Events;
using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeadUpdater
{
    public class Consumer : IConsumer<LeadsRoleUpdatedEvent>
    {
        private readonly ILogger _logger;

        public Consumer(ILogger<Consumer> logger)
        {
            _logger = logger;
        }
        public async Task Consume(ConsumeContext<LeadsRoleUpdatedEvent> context)
        {
            _logger.LogInformation("Message received");
        }
    }
}