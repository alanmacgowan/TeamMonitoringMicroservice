using System;
using System.Threading.Tasks;

namespace TeamMonitoring.ProximityMonitor.TeamService
{
    public interface ITeamServiceClient
    {
        Task<Team> GetTeam(Guid teamId);
        Task<Member> GetMember(Guid teamId, Guid memberId);
    }
}