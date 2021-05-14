using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamMonitoring.ProximityMonitor.Realtime
{
    public interface ITeamMonitoringHub
    {
        Task ProximityDetectedNotification(ProximityDetectedRealtimeEvent eventMessage);
    }
}
