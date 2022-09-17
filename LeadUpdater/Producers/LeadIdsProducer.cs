using IncredibleBackend.Messaging;
using LeadUpdater.Interfaces;

namespace LeadUpdater.Producers;

public class LeadIdsProducer : ILeadIdsProducer
{
    private readonly MessageProducer _messageProducer;
    private readonly ILogger<LeadIdsProducer> _logger;

    public LeadIdsProducer(MessageProducer messageProducer, ILogger<LeadIdsProducer> logger)
    {
        _messageProducer = messageProducer;
        _logger = logger;
    }

    public async Task SendMessage<T> (T message)
    {
        try
        {
            await _messageProducer.ProduceMessage(message, "Sent Lead's Ids to Queue");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
        }
    }
}
