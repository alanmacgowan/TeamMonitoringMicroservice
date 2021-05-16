namespace TeamMonitoring.Common.Queues
{
    public interface IEventPublisher<T>
    {
        void PublishEvent(T emmitEvent);
    }
}
