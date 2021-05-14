using System;

namespace TeamMonitoring.ProximityMonitor.TeamService
{
    public interface ITeamServiceClient
    {
        Team GetTeam(Guid teamId);
        Member GetMember(Guid teamId, Guid memberId);
    }
}