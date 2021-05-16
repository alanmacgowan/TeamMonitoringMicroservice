namespace TeamMonitoring.Common.Processor
{
    public interface IEventProcessor
    {
        void Start();
        void Stop();
    }
}