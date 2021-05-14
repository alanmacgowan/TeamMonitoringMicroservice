using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace TeamMonitoring.ProximityMonitor.Realtime
{
    public class TeamMonitoringHub : Hub<ITeamMonitoringHub>
    {
        public async Task NotifyProximityDetectedEvent(ProximityDetectedRealtimeEvent eventMessage)
        {
            await Clients.All.ProximityDetectedNotification(eventMessage);
        }
    }
}
