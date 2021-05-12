using Microsoft.EntityFrameworkCore;
using TeamMonitoring.TeamService.TeamService.API.Models;

namespace TeamMonitoring.TeamService.API.Persistence
{
    public class TeamDbContext : DbContext
    {
        public TeamDbContext(DbContextOptions<TeamDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Team> Teams { get; set; }
        public DbSet<Member> Members { get; set; }
    }
}