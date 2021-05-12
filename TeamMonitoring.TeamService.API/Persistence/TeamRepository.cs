using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeamMonitoring.TeamService.TeamService.API.Models;
using TeamMonitoring.TeamService.TeamService.API.Persistence;

namespace TeamMonitoring.TeamService.API.Persistence
{
    public class TeamRepository : ITeamRepository
    {

        private TeamDbContext context;

        public TeamRepository(TeamDbContext context)
        {
            this.context = context;
        }

        public Team Add(Team team)
        {
            this.context.Add(team);
            this.context.SaveChanges();
            return team;
        }

        public Team Delete(Guid id)
        {
            Team team = this.Get(id);
            this.context.Remove(team);
            this.context.SaveChanges();
            return team;
        }

        public Team Get(Guid id)
        {
            return this.context.Teams.Include(x => x.Members).FirstOrDefault(lr => lr.ID == id);
        }

        public IEnumerable<Team> List()
        {
            return this.context.Teams.ToList();
        }

        public Team Update(Team team)
        {
            this.context.Entry(team).State = EntityState.Modified;
            this.context.SaveChanges();
            return team;
        }
    }
}
