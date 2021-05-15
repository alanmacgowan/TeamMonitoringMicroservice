namespace TeamMonitoring.Common.Queues
{
    public interface IEvent<T>
    {
        string toJson();

        T FromJson(string jsonBody);
    }
}
