using Microsoft.AspNetCore.Mvc;
using System;
using TeamMonitoring.TeamService.TeamService.API.Models;
using TeamMonitoring.TeamService.TeamService.API.Persistence;

namespace TeamMonitoring.TeamService.API.Controllers
{
    [Route("[controller]")]
    public class TeamsController : ControllerBase
    {
        protected readonly ITeamRepository _repository;

        public TeamsController(ITeamRepository repo)
        {
            _repository = repo;
        }

        [HttpGet]
        public virtual IActionResult GetAllTeams()
        {
            return this.Ok(_repository.List());
        }

        [HttpGet("{id}")]
        public IActionResult GetTeam(Guid id)
        {
            Team team = _repository.Get(id);

            if (team != null)	  
            {
                return this.Ok(team);
            }
            else
            {
                return this.NotFound();
            }
        }

        [HttpPost]
        public virtual IActionResult CreateTeam([FromBody] Team newTeam)
        {
            _repository.Add(newTeam);

            return this.Created($"/teams/{newTeam.ID}", newTeam);
        }

        [HttpPut("{id}")]
        public virtual IActionResult UpdateTeam([FromBody] Team team, Guid id)
        {
            team.ID = id;

            if (_repository.Update(team) == null)
            {
                return this.NotFound();
            }
            else
            {
                return this.Ok(team);
            }
        }

        [HttpDelete("{id}")]
        public virtual IActionResult DeleteTeam(Guid id)
        {
            Team team = _repository.Delete(id);

            if (team == null)
            {
                return this.NotFound();
            }
            else
            {
                return this.Ok(team.ID);
            }
        }
    }
}
