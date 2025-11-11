namespace UserService.Infrastructure.Messaging
{
    public interface IServiceBusPublisher
    {
        Task PublishAsync(string topicName, object payload, CancellationToken cancellationToken);
    }

}
