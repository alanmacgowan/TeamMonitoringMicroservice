using System;
using System.Collections.Generic;
using TeamMonitoring.TeamService.TeamService.API.Models;

namespace TeamMonitoring.TeamService.TeamService.API.Persistence
{
    public interface ITeamRepository
    {
        IEnumerable<Team> List();
        Team Get(Guid id);
        Team Add(Team team);
        Team Update(Team team);
        Team Delete(Guid id);
    }
}