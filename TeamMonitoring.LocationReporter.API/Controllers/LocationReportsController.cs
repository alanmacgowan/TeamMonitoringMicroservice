using Microsoft.AspNetCore.Mvc;
using System;
using TeamMonitoring.Common.Queues;
using TeamMonitoring.Events;
using TeamMonitoring.LocationReporter.API.Models;
using TeamMonitoring.LocationReporter.API.Services;
using System.Threading.Tasks;
using Prometheus;
using Microsoft.Extensions.Logging;

namespace TeamMonitoring.LocationReporter.API.Controllers
{
    [Route("/api/members/{memberId}/locationreports")]
    public class LocationReportsController : ControllerBase
    {
        private ICommandEventConverter _eventConverter;
        private IEventPublisher<MemberLocationRecordedEvent> _eventPublisher;
        private ITeamServiceClient _teamServiceClient;
        private static readonly Counter PublishedEventsCount = Metrics.CreateCounter("memberlocation_recorded_events_processed_total", "Number of MemberLocationRecordedEvent events published.");
        private readonly ILogger<LocationReportsController> _logger;

        public LocationReportsController(ICommandEventConverter eventConverter,
                                         IEventPublisher<MemberLocationRecordedEvent> eventPublisher,
                                         ITeamServiceClient teamServiceClient,
                                         ILogger<LocationReportsController> logger)
        {
            _eventConverter = eventConverter;
            _eventPublisher = eventPublisher;
            _teamServiceClient = teamServiceClient;
            _logger = logger;
        }

        [HttpPost]
        public async Task<ActionResult> PostLocationReport(Guid memberId, [FromBody] LocationReport locationReport)
        {
            MemberLocationRecordedEvent locationRecordedEvent = _eventConverter.CommandToEvent(locationReport);
            locationRecordedEvent.TeamID = await _teamServiceClient.GetTeamForMember(locationReport.MemberID);
            _eventPublisher.PublishEvent(locationRecordedEvent);
            
            PublishedEventsCount.Inc();
            _logger.LogInformation($"MemberLocationRecordedEvent Published for MemberId: {locationRecordedEvent.MemberID}");

            return Created($"/api/members/{memberId}/locationreports/{locationReport.ReportID}", locationReport);
        }
    }
}