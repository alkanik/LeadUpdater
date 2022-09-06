namespace LeadUpdater;

public interface IMessageProducer
{
    void SendMessage<T>(T message);
}