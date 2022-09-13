namespace LeadUpdater.Interfaces
{
    public interface ILeadIdsProducer
    {
        Task SendMessage<T>(T message);
    }
}