namespace TeamMonitoring.ProximityMonitor.Events
{
    public interface IEventProcessor
    {
        void Start();
        void Stop();
    }
}