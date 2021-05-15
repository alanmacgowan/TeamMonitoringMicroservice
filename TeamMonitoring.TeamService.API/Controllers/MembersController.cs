using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using TeamMonitoring.TeamService.TeamService.API.Models;
using TeamMonitoring.TeamService.TeamService.API.Persistence;

namespace TeamMonitoring.TeamService.API.Controllers
{
    [Route("/teams/{teamId}/[controller]")]
    public class MembersController : ControllerBase
    {
        protected readonly ITeamRepository _repository;

        public MembersController(ITeamRepository repo)
        {
            _repository = repo;
        }

        [HttpGet]
        [Route("/teams/{teamID}/[controller]/members")]
        public virtual IActionResult GetMembers(Guid teamID)
        {
            Team team = _repository.Get(teamID);

            if (team == null)
            {
                return this.NotFound();
            }
            else
            {
                return this.Ok(team.Members);
            }
        }


        [HttpGet]
        [Route("/teams/{teamID}/[controller]/{memberId}")]
        public virtual IActionResult GetMember(Guid teamID, Guid memberId)
        {
            Team team = _repository.Get(teamID);

            if (team == null)
            {
                return this.NotFound();
            }
            else
            {
                var q = team.Members.Where(m => m.ID == memberId);

                if (q.Count() < 1)
                {
                    return this.NotFound();
                }
                else
                {
                    return this.Ok(q.First());
                }
            }
        }

        [HttpPut]
        [Route("/teams/{teamID}/[controller]/{memberId}")]
        public virtual IActionResult UpdateMember([FromBody] Member updatedMember, Guid teamID, Guid memberId)
        {
            Team team = _repository.Get(teamID);

            if (team == null)
            {
                return this.NotFound();
            }
            else
            {
                var q = team.Members.Where(m => m.ID == memberId);

                if (q.Count() < 1)
                {
                    return this.NotFound();
                }
                else
                {
                    team.Members.Remove(q.First());
                    team.Members.Add(updatedMember);
                    return this.Ok();
                }
            }
        }

        [HttpPost]
        public virtual IActionResult CreateMember([FromBody] Member newMember, Guid teamID)
        {
            Team team = _repository.Get(teamID);

            if (team == null)
            {
                return this.NotFound();
            }
            else
            {
                team.Members.Add(newMember);
                var teamMember = new { TeamID = team.ID, MemberID = newMember.ID };
                return this.Created($"/teams/{teamMember.TeamID}/[controller]/{teamMember.MemberID}", teamMember);
            }
        }

        [HttpGet]
        [Route("/members/{memberId}/team")]
        public IActionResult GetTeamForMember(Guid memberId)
        {
            var teamId = GetTeamIdForMember(memberId);
            if (teamId != Guid.Empty)
            {
                return this.Ok(new
                {
                    TeamID = teamId
                });
            }
            else
            {
                return this.NotFound();
            }
        }

        private Guid GetTeamIdForMember(Guid memberId)
        {
            foreach (var team in _repository.List())
            {
                var member = team.Members.FirstOrDefault(m => m.ID == memberId);
                if (member != null)
                {
                    return team.ID;
                }
            }
            return Guid.Empty;
        }
    }
}