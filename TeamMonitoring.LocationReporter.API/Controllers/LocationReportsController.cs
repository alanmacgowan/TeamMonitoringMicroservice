using Microsoft.AspNetCore.Mvc;
using System;
using TeamMonitoring.Common.Queues;
using TeamMonitoring.Events;
using TeamMonitoring.LocationReporter.API.Models;
using TeamMonitoring.LocationReporter.API.Services;
using System.Threading.Tasks;

namespace TeamMonitoring.LocationReporter.API.Controllers
{
    [Route("/api/members/{memberId}/locationreports")]
    public class LocationReportsController : ControllerBase
    {
        private ICommandEventConverter _eventConverter;
        private IEventPublisher<MemberLocationRecordedEvent> _eventPublisher;
        private ITeamServiceClient _teamServiceClient;


        public LocationReportsController(ICommandEventConverter eventConverter,
                                         IEventPublisher<MemberLocationRecordedEvent> eventPublisher,
                                         ITeamServiceClient teamServiceClient)
        {
            _eventConverter = eventConverter;
            _eventPublisher = eventPublisher;
            _teamServiceClient = teamServiceClient;
        }

        [HttpPost]
        public async Task<ActionResult> PostLocationReport(Guid memberId, [FromBody] LocationReport locationReport)
        {
            MemberLocationRecordedEvent locationRecordedEvent = _eventConverter.CommandToEvent(locationReport);
            locationRecordedEvent.TeamID = await _teamServiceClient.GetTeamForMember(locationReport.MemberID);
            _eventPublisher.PublishEvent(locationRecordedEvent);

            return Created($"/api/members/{memberId}/locationreports/{locationReport.ReportID}", locationReport);
        }
    }
}