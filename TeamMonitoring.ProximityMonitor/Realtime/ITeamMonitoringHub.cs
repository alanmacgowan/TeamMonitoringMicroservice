using System.Threading.Tasks;

namespace TeamMonitoring.ProximityMonitor.Realtime
{
    public interface ITeamMonitoringHub
    {
        Task ProximityDetectedNotification(ProximityDetectedRealtimeEvent eventMessage);
    }
}
