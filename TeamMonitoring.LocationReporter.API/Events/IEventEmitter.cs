namespace TeamMonitoring.LocationReporter.API.Events
{
    public interface IEventEmitter
    {
        void EmitLocationRecordedEvent(MemberLocationRecordedEvent locationRecordedEvent);
    }
}