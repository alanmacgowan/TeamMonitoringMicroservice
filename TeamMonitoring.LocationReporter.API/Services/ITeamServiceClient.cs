using System;
using System.Threading.Tasks;

namespace TeamMonitoring.LocationReporter.API.Services
{
    public interface ITeamServiceClient
    {
        Task<Guid> GetTeamForMember(Guid memberId);
    }
}