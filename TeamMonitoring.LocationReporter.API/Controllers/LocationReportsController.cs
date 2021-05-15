using Microsoft.AspNetCore.Mvc;
using System;
using TeamMonitoring.Common.Queues;
using TeamMonitoring.LocationReporter.API.Events;
using TeamMonitoring.LocationReporter.API.Models;
using TeamMonitoring.LocationReporter.API.Services;

namespace TeamMonitoring.LocationReporter.API.Controllers
{
    [Route("/api/members/{memberId}/locationreports")]
    public class LocationReportsController : ControllerBase
    {
        private ICommandEventConverter converter;
        private IEventEmitter<MemberLocationRecordedEvent> eventEmitter;
        private ITeamServiceClient teamServiceClient;


        public LocationReportsController(ICommandEventConverter converter,
            IEventEmitter<MemberLocationRecordedEvent> eventEmitter,
            ITeamServiceClient teamServiceClient)
        {
            this.converter = converter;
            this.eventEmitter = eventEmitter;
            this.teamServiceClient = teamServiceClient;
        }

        [HttpPost]
        public ActionResult PostLocationReport(Guid memberId, [FromBody] LocationReport locationReport)
        {
            MemberLocationRecordedEvent locationRecordedEvent = converter.CommandToEvent(locationReport);
            locationRecordedEvent.TeamID = teamServiceClient.GetTeamForMember(locationReport.MemberID);
            eventEmitter.EmitEvent(locationRecordedEvent);

            return this.Created($"/api/members/{memberId}/locationreports/{locationReport.ReportID}", locationReport);
        }
    }
}