using TeamMonitoring.EventProcessor.Events;

namespace TeamMonitoring.EventProcessor.Queues
{
    public delegate void MemberLocationRecordedEventReceivedDelegate(MemberLocationRecordedEvent evt);

    public interface IEventSubscriber
    {
        void Subscribe();
        void Unsubscribe();

        event MemberLocationRecordedEventReceivedDelegate MemberLocationRecordedEventReceived;
    }
}