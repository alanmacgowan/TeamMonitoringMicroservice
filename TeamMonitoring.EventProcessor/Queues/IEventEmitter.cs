using TeamMonitoring.EventProcessor.Events;

namespace TeamMonitoring.EventProcessor.Queues
{
    public interface IEventEmitter
    {
        void EmitProximityDetectedEvent(ProximityDetectedEvent proximityDetectedEvent);
    }
}