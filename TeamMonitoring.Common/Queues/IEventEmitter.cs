namespace TeamMonitoring.Common.Queues
{
    public interface IEventEmitter<T>
    {
        void EmitEvent(T emmitEvent);
    }
}
