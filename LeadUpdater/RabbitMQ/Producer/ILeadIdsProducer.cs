namespace LeadUpdater.RabbitMQ.Producer
{
    public interface ILeadIdsProducer
    {
        Task SendMessage<T>(T message);
    }
}