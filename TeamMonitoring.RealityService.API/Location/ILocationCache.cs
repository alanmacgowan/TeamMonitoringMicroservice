using System;
using System.Collections.Generic;

namespace TeamMonitoring.RealityService.Location
{
    public interface ILocationCache
    {
        IList<MemberLocation> GetMemberLocations(Guid teamId);
        void Put(Guid teamId, MemberLocation memberLocation);
        MemberLocation Get(Guid teamId, Guid memberId);
    }
}