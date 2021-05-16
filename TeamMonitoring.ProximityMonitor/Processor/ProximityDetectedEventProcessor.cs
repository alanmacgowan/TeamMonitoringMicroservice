using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using TeamMonitoring.Common.Queues;
using TeamMonitoring.ProximityMonitor.Realtime;
using TeamMonitoring.ProximityMonitor.TeamService;
using TeamMonitoring.Common.Processor;
using TeamMonitoring.Events;

namespace TeamMonitoring.ProximityMonitor.Processor
{
    public class ProximityDetectedEventProcessor : BackgroundService, IEventProcessor
    {
        private ILogger _logger;
        private ITeamServiceClient _teamClient;
        private IEventSubscriber<ProximityDetectedEvent> _eventSubscriber;
        private IHubContext<TeamMonitoringHub, ITeamMonitoringHub> _notificationgHub;

        public ProximityDetectedEventProcessor(ILogger<ProximityDetectedEventProcessor> logger,
                                               IEventSubscriber<ProximityDetectedEvent> eventSubscriber,
                                               ITeamServiceClient teamClient,
                                               IHubContext<TeamMonitoringHub, ITeamMonitoringHub> notificationgHub)
        {
            _notificationgHub = notificationgHub;
            _logger = logger;
            _eventSubscriber = eventSubscriber;
            _teamClient = teamClient;
            _logger.LogInformation("Created Proximity Event Processor.");

            _eventSubscriber.EventReceived += async (pde) =>
            {
                await NotifyProximityDetectedRealtimeEvent(pde);
            };

            Start();
        }

        private async Task NotifyProximityDetectedRealtimeEvent(ProximityDetectedEvent pde)
        {
            var team = await _teamClient.GetTeam(pde.TeamID);
            var sourceMember = await _teamClient.GetMember(pde.TeamID, pde.SourceMemberID);
            var targetMember = await _teamClient.GetMember(pde.TeamID, pde.TargetMemberID);

            var outEvent = new ProximityDetectedRealtimeEvent
            {
                TargetMemberID = pde.TargetMemberID,
                SourceMemberID = pde.SourceMemberID,
                DetectionTime = pde.DetectionTime,
                SourceMemberLocation = pde.SourceMemberLocation,
                TargetMemberLocation = pde.TargetMemberLocation,
                MemberDistance = pde.MemberDistance,
                TeamID = pde.TeamID,
                TeamName = team.Name,
                SourceMemberName = $"{sourceMember.FirstName} {sourceMember.LastName}",
                TargetMemberName = $"{targetMember.FirstName} {targetMember.LastName}"
            };
            await _notificationgHub.Clients.All.ProximityDetectedNotification(outEvent);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(1000, stoppingToken);
            }
        }

        public void Start()
        {
            _eventSubscriber.Subscribe();
        }

        public void Stop()
        {
            _eventSubscriber.Unsubscribe();
        }
    }
}