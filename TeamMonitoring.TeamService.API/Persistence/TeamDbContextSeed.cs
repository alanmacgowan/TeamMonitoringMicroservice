using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TeamMonitoring.TeamService.TeamService.API.Models;

namespace TeamMonitoring.TeamService.API.Persistence
{
    public class TeamDbContextSeed
    {
        public async Task SeedAsync(TeamDbContext context, ILogger<TeamDbContextSeed> logger)
        {

            var policy = CreatePolicy(logger, nameof(TeamDbContextSeed));

            await policy.ExecuteAsync(async () =>
            {

                if (context.Teams.Any())
                {
                    return;   // DB has been seeded
                }

                var teams = new List<Team>()
                {
                    new Team() { Name = "Team 1", Members = new List<Member> {
                                new Member {FirstName = "Juan", LastName = "Perez" },
                                new Member { FirstName = "Martin", LastName = "Macgowan" },
                                new Member { FirstName = "Carlos", LastName = "Gonzalez" } 
                            } 
                    }
                };

                await context.Teams.AddRangeAsync(teams);
                await context.SaveChangesAsync();

            });
        }

        private AsyncRetryPolicy CreatePolicy(ILogger<TeamDbContextSeed> logger, string prefix, int retries = 3)
        {
            return Policy.Handle<SqlException>().
                WaitAndRetryAsync(
                    retryCount: retries,
                    sleepDurationProvider: retry => TimeSpan.FromSeconds(5),
                    onRetry: (exception, timeSpan, retry, ctx) =>
                    {
                        logger.LogWarning(exception, "[{prefix}] Exception {ExceptionType} with message {Message} detected on attempt {retry} of {retries}", prefix, exception.GetType().Name, exception.Message, retry, retries);
                    }
                );
        }
    }
}
