using TeamMonitoring.LocationReporter.API.Events;

namespace TeamMonitoring.LocationReporter.API.Queues
{
    public interface IEventEmitter
    {
        void EmitLocationRecordedEvent(MemberLocationRecordedEvent locationRecordedEvent);
    }
}