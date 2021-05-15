
namespace TeamMonitoring.Common.Queues
{
    public delegate void Delegate<T>(T evt);

    public interface IEventSubscriber<T>
    {
        void Subscribe();
        void Unsubscribe();

        event Delegate<T> EventReceived;
    }
}