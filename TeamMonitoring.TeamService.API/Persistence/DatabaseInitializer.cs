using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeamMonitoring.TeamService.TeamService.API.Models;

namespace TeamMonitoring.TeamService.API.Persistence
{
    public static class DatabaseInitializer
    {
        public static void Initialize(TeamDbContext context)
        {
            context.Database.EnsureCreated();

            if (context.Teams.Any())
            {
                return;   // DB has been seeded
            }

            //for the challenge we assume there is only one Room
            var teams = new List<Team>()
            {
                new Team() { Name = "Team 1", Members = new List<Member> { 
                    new Member {FirstName = "Juan", LastName = "Perez" },
                    new Member { FirstName = "Martin", LastName = "Macgowan" },
                    new Member { FirstName = "Carlos", LastName = "Gonzalez" } } }
            };

            context.Teams.AddRange(teams);
            context.SaveChanges();
        }
    }
}
