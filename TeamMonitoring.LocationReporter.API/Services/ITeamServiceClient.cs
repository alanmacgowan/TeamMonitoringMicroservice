using System;

namespace TeamMonitoring.LocationReporter.API.Services
{
    public interface ITeamServiceClient
    {
        Guid GetTeamForMember(Guid memberId);
    }
}